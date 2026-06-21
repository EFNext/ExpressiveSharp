using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using ExpressiveSharp.Extensions;

namespace ExpressiveSharp.Services;

/// <summary>
/// Replaces calls to <see cref="ExpressiveAttribute"/>-marked members with their generated
/// expression trees. EF Core-specific subclasses override <see cref="VisitMethodCallCore"/>
/// and <see cref="VisitExtension"/> to add query-provider awareness.
/// </summary>
public class ExpressiveReplacer : ExpressionVisitor
{
    private readonly IExpressiveResolver _resolver;
    private readonly bool _polymorphicDispatch;
    private readonly ExpressionArgumentReplacer _expressionArgumentReplacer = new();
    private readonly Dictionary<MemberInfo, LambdaExpression?> _memberCache = new();
    // Guards against infinite expansion when an [Expressive] member references itself
    // (directly or via mutual recursion). Currently-expanding members are left as plain
    // calls instead of being inlined again — the runtime then dispatches normally on
    // .Compile()/.Invoke().
    private readonly HashSet<MemberInfo> _expandingMembers = new();

    private static readonly ConditionalWeakTable<Type, StrongBox<bool>> _compilerGeneratedClosureCache = new();

    // Cached derived-override discovery for polymorphic dispatch (keyed by receiver type + base
    // member). Discovery scans every loaded assembly; the cache is dropped when the assembly count
    // changes (mirrors ExpressiveResolver) so runtime-compiled assemblies are picked up.
    private static readonly ConcurrentDictionary<(Type RootType, MemberInfo BaseMember), PolymorphicPlan> _polymorphicPlanCache = new();
    private static int _polymorphicPlanAssemblyCount;

    internal static void ClearCachesForMetadataUpdate()
    {
        _compilerGeneratedClosureCache.Clear();
        _polymorphicPlanCache.Clear();
        Volatile.Write(ref _polymorphicPlanAssemblyCount, 0);
    }

    public ExpressiveReplacer(IExpressiveResolver resolver, bool polymorphicDispatch = true)
    {
        _resolver = resolver;
        _polymorphicDispatch = polymorphicDispatch;
    }

    protected bool TryGetReflectedExpression(MemberInfo memberInfo, [NotNullWhen(true)] out LambdaExpression? reflectedExpression)
    {
        if (!_memberCache.TryGetValue(memberInfo, out reflectedExpression))
        {
            var attribute = memberInfo.GetCustomAttribute<ExpressiveAttribute>(false);

            reflectedExpression = attribute is not null
                ? _resolver.FindGeneratedExpression(memberInfo, attribute)
                : _resolver.FindExternalExpression(memberInfo);

            _memberCache.Add(memberInfo, reflectedExpression);
        }

        return reflectedExpression is not null;
    }

    [return: NotNullIfNotNull(nameof(node))]
    public virtual Expression? Replace(Expression? node) => Visit(node);

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        // Replace MethodGroup arguments with their reflected expressions.
        Expression[]? updatedArgs = null;
        for (var i = 0; i < node.Arguments.Count; i++)
        {
            if (node.Arguments[i] is UnaryExpression {
                    NodeType: ExpressionType.Convert,
                    Operand: MethodCallExpression {
                        NodeType: ExpressionType.Call,
                        Method: { Name: nameof(MethodInfo.CreateDelegate), DeclaringType.Name: nameof(MethodInfo) },
                        Object: ConstantExpression { Value: MethodInfo capturedMethodInfo }
                    }
                } && TryGetReflectedExpression(capturedMethodInfo, out var expressionArg))
            {
                (updatedArgs ??= [.. node.Arguments])[i] = expressionArg;
            }
        }
        if (updatedArgs is not null)
        {
            node = node.Update(node.Object, updatedArgs);
        }

        VisitMethodCallCore(node);

        if (_polymorphicDispatch
            && node.Object is { } callInstance
            && node.Method is { DeclaringType.IsInterface: false, IsVirtual: true, IsFinal: false }
            && Attribute.IsDefined(node.Method, typeof(ExpressiveAttribute), inherit: false)
            && TryExpandPolymorphic(node.Method, callInstance, node.Arguments, node.Type, out var polyCall))
        {
            return polyCall;
        }

        var methodInfo = node.Method.DeclaringType?.IsInterface == true
            ? (node.Object?.Type.GetConcreteMethod(node.Method) ?? node.Method)
            : node.Method;

        if (!_expandingMembers.Contains(methodInfo) &&
            TryGetReflectedExpression(methodInfo, out var reflectedExpression))
        {
            _expandingMembers.Add(methodInfo);
            try
            {
                for (var parameterIndex = 0; parameterIndex < reflectedExpression.Parameters.Count; parameterIndex++)
                {
                    var parameterExpression = reflectedExpression.Parameters[parameterIndex];
                    var mappedArgumentExpression = (parameterIndex, node.Object) switch {
                        (0, not null) => node.Object,
                        (_, not null) => node.Arguments[parameterIndex - 1],
                        (_, null) => node.Arguments.Count > parameterIndex ? node.Arguments[parameterIndex] : null
                    };

                    if (mappedArgumentExpression is not null)
                    {
                        _expressionArgumentReplacer.ParameterArgumentMapping.Add(parameterExpression, mappedArgumentExpression);
                    }
                }

                var updatedBody = _expressionArgumentReplacer.Visit(reflectedExpression.Body);
                return base.Visit(updatedBody);
            }
            finally
            {
                _expressionArgumentReplacer.ParameterArgumentMapping.Clear();
                _expandingMembers.Remove(methodInfo);
            }
        }

        return base.VisitMethodCall(node);
    }

    /// <summary>
    /// Called during <see cref="VisitMethodCall"/> before expression replacement.
    /// Override to add tracking detection or other method-level hooks.
    /// </summary>
    protected virtual void VisitMethodCallCore(MethodCallExpression node) { }

    protected override Expression VisitNew(NewExpression node)
    {
        var constructor = node.Constructor;
        if (constructor is not null &&
            !_expandingMembers.Contains(constructor) &&
            TryGetReflectedExpression(constructor, out var reflectedExpression))
        {
            _expandingMembers.Add(constructor);
            try
            {
                for (var parameterIndex = 0; parameterIndex < reflectedExpression.Parameters.Count; parameterIndex++)
                {
                    var parameterExpression = reflectedExpression.Parameters[parameterIndex];
                    if (parameterIndex < node.Arguments.Count)
                    {
                        _expressionArgumentReplacer.ParameterArgumentMapping.Add(parameterExpression, node.Arguments[parameterIndex]);
                    }
                }

                var updatedBody = _expressionArgumentReplacer.Visit(reflectedExpression.Body);
                return base.Visit(updatedBody);
            }
            finally
            {
                _expressionArgumentReplacer.ParameterArgumentMapping.Clear();
                _expandingMembers.Remove(constructor);
            }
        }

        return base.VisitNew(node);
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        var nodeExpression = node.Expression switch {
            UnaryExpression { NodeType: ExpressionType.Convert, Type: { IsInterface: true } type, Operand: { } operand }
                when type.IsAssignableFrom(operand.Type)
                => operand,
            _ => node.Expression
        };
        var nodeMember = node.Member switch {
            PropertyInfo property when nodeExpression is not null && property.DeclaringType?.IsInterface == true
                => nodeExpression.Type.GetConcreteProperty(property),
            _ => node.Member
        };

        if (_polymorphicDispatch
            && node.Expression is { } memberInstance
            && node.Member is PropertyInfo { DeclaringType.IsInterface: false, GetMethod: { IsVirtual: true, IsFinal: false } }
            && Attribute.IsDefined(node.Member, typeof(ExpressiveAttribute), inherit: false)
            && TryExpandPolymorphic(node.Member, memberInstance, methodArgs: null, node.Type, out var polyMember))
        {
            return polyMember;
        }

        if (!_expandingMembers.Contains(nodeMember) &&
            TryGetReflectedExpression(nodeMember, out var reflectedExpression))
        {
            _expandingMembers.Add(nodeMember);
            try
            {
                if (nodeExpression is not null)
                {
                    _expressionArgumentReplacer.ParameterArgumentMapping.Add(reflectedExpression.Parameters[0], nodeExpression);
                    var updatedBody = _expressionArgumentReplacer.Visit(reflectedExpression.Body);
                    return base.Visit(updatedBody);
                }

                return base.Visit(reflectedExpression.Body);
            }
            finally
            {
                _expressionArgumentReplacer.ParameterArgumentMapping.Clear();
                _expandingMembers.Remove(nodeMember);
            }
        }

        return base.VisitMember(node);
    }

    /// <summary>
    /// Expands a virtual/override [Expressive] member into a runtime type-test chain
    /// (<c>e is Dog ? ((Dog)e).body : baseBody</c>) so each value uses its runtime type's body;
    /// EF Core translates the <c>is</c> tests to a TPH discriminator <c>CASE</c>. Returns
    /// <c>false</c> (caller falls back to the static path) when there is nothing polymorphic to do.
    /// </summary>
    private bool TryExpandPolymorphic(MemberInfo baseMember, Expression instance,
        IReadOnlyList<Expression>? methodArgs, Type resultType, [NotNullWhen(true)] out Expression? result)
    {
        result = null;

        // Self-referential member already mid-expansion: let the legacy path emit a plain access.
        if (_expandingMembers.Contains(baseMember))
        {
            return false;
        }

        var rootType = instance.Type;
        if (rootType.IsInterface)
        {
            return false;
        }

        var plan = GetOrBuildPolymorphicPlan(rootType, baseMember);

        if (plan.Arms.Length == 0)
        {
            // No derived overrides: diverge from the static path only when the receiver's own type
            // overrides the base slot (so `d.Score` uses ScoreDerived's body, not ScoreBase's).
            if (!plan.RootRegistered
                || SameSlot(plan.RootMember, baseMember)
                || _expandingMembers.Contains(plan.RootMember))
            {
                return false;
            }

            result = ExpandPolymorphicBody(plan.RootMember, instance, convertTo: null, methodArgs, resultType);
            return true;
        }

        // Else branch: the receiver type's own body, or a throw when the base slot is abstract
        // (unreachable in a closed TPH hierarchy where every concrete row matches an arm).
        Expression acc = plan.RootRegistered
            ? ExpandPolymorphicBody(plan.RootMember, instance, convertTo: null, methodArgs, resultType)
            : Expression.Throw(
                Expression.New(
                    typeof(InvalidOperationException).GetConstructor([typeof(string)])!,
                    Expression.Constant(
                        $"No polymorphic [Expressive] override matched the runtime type for member '{plan.RootMember.Name}'.")),
                resultType);

        // Ascending depth → folding leaves the most-derived test outermost.
        foreach (var arm in plan.Arms)
        {
            var thenExpr = ExpandPolymorphicBody(arm.Member, instance, convertTo: arm.TestType, methodArgs, resultType);
            acc = Expression.Condition(Expression.TypeIs(instance, arm.TestType), thenExpr, acc, resultType);
        }

        result = acc;
        return true;
    }

    private static bool SameSlot(MemberInfo a, MemberInfo b) => a.DeclaringType == b.DeclaringType;

    /// <summary>
    /// Binds <paramref name="instance"/> (cast to <paramref name="convertTo"/> for a derived arm)
    /// into the member's body and recursively expands it. A member already on the expansion stack
    /// (e.g. a <c>base.X</c> reference inside its own override) is left as a plain access.
    /// </summary>
    private Expression ExpandPolymorphicBody(MemberInfo member, Expression instance, Type? convertTo,
        IReadOnlyList<Expression>? methodArgs, Type resultType)
    {
        var boundInstance = convertTo is null ? instance : Expression.Convert(instance, convertTo);

        if (_expandingMembers.Contains(member) || !TryGetReflectedExpressionSafe(member, out var body))
        {
            return Coerce(PlainAccess(member, boundInstance, methodArgs), resultType);
        }

        _expandingMembers.Add(member);
        var added = new List<ParameterExpression>(body.Parameters.Count);
        var map = _expressionArgumentReplacer.ParameterArgumentMapping;
        try
        {
            map[body.Parameters[0]] = boundInstance;
            added.Add(body.Parameters[0]);
            for (var i = 1; i < body.Parameters.Count; i++)
            {
                if (methodArgs is not null && methodArgs.Count >= i)
                {
                    map[body.Parameters[i]] = methodArgs[i - 1];
                    added.Add(body.Parameters[i]);
                }
            }

            // Substitution is eager, so remove only our own keys (never Clear the shared map,
            // which sibling branches and nested expansions also use).
            var substituted = _expressionArgumentReplacer.Visit(body.Body);
            return Coerce(base.Visit(substituted), resultType);
        }
        finally
        {
            foreach (var addedParameter in added)
            {
                map.Remove(addedParameter);
            }
            _expandingMembers.Remove(member);
        }
    }

    private static Expression Coerce(Expression expression, Type type)
        => expression.Type == type ? expression : Expression.Convert(expression, type);

    private static Expression PlainAccess(MemberInfo member, Expression instance, IReadOnlyList<Expression>? methodArgs)
        => member switch
        {
            PropertyInfo property => Expression.Property(instance, property),
            MethodInfo method => Expression.Call(instance, method, methodArgs ?? (IReadOnlyList<Expression>)Array.Empty<Expression>()),
            _ => throw new InvalidOperationException($"Cannot build a plain access for member '{member}'.")
        };

    private PolymorphicPlan GetOrBuildPolymorphicPlan(Type rootType, MemberInfo baseMember)
    {
        var count = AppDomain.CurrentDomain.GetAssemblies().Length;
        if (count != Volatile.Read(ref _polymorphicPlanAssemblyCount))
        {
            _polymorphicPlanCache.Clear();
            Volatile.Write(ref _polymorphicPlanAssemblyCount, count);
        }

        return _polymorphicPlanCache.GetOrAdd((rootType, baseMember),
            key => BuildPolymorphicPlan(key.RootType, key.BaseMember));
    }

    private PolymorphicPlan BuildPolymorphicPlan(Type rootType, MemberInfo baseMember)
    {
        var rootMember = ResolveConcreteMember(rootType, baseMember) ?? baseMember;
        var rootRegistered = TryGetReflectedExpressionSafe(rootMember, out _);

        var arms = new List<PolymorphicArm>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic)
            {
                continue;
            }

            Type?[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types;
            }
            catch
            {
                continue;
            }

            foreach (var candidate in types)
            {
                // Open generic definitions (Box<T>) can't be used in Expression.TypeIs/Convert and
                // are never a runtime type; only closed constructions reach a query.
                if (candidate is null || candidate.IsInterface || candidate == rootType
                    || candidate.ContainsGenericParameters
                    || !rootType.IsAssignableFrom(candidate))
                {
                    continue;
                }

                var concrete = ResolveConcreteMember(candidate, baseMember);

                // One arm per declaring type: a type that only inherits an override is covered by
                // its declaring ancestor's `is` test. Skip plain overrides (no registered body) —
                // EXP0032 flags those.
                if (concrete is null || concrete.DeclaringType != candidate
                    || !TryGetReflectedExpressionSafe(concrete, out _))
                {
                    continue;
                }

                arms.Add(new PolymorphicArm(candidate, concrete, InheritanceDepth(rootType, candidate)));
            }
        }

        arms.Sort(static (a, b) => a.Depth.CompareTo(b.Depth));
        return new PolymorphicPlan(rootMember, rootRegistered, arms.ToArray());
    }

    private static MemberInfo? ResolveConcreteMember(Type type, MemberInfo baseMember)
    {
        try
        {
            return baseMember switch
            {
                PropertyInfo property => type.GetConcreteProperty(property),
                MethodInfo method => type.GetConcreteMethod(method),
                _ => null
            };
        }
        catch
        {
            return null;
        }
    }

    private static int InheritanceDepth(Type root, Type type)
    {
        var depth = 0;
        for (var current = type; current is not null && current != root; current = current.BaseType)
        {
            depth++;
        }

        return depth;
    }

    private bool TryGetReflectedExpressionSafe(MemberInfo memberInfo, [NotNullWhen(true)] out LambdaExpression? reflectedExpression)
    {
        if (IsAbstractMember(memberInfo))
        {
            reflectedExpression = null;
            return false;
        }

        return TryGetReflectedExpression(memberInfo, out reflectedExpression);
    }

    private static bool IsAbstractMember(MemberInfo memberInfo) => memberInfo switch
    {
        MethodInfo method => method.IsAbstract,
        PropertyInfo property => (property.GetMethod ?? property.SetMethod)?.IsAbstract ?? false,
        _ => false
    };

    private sealed record PolymorphicArm(Type TestType, MemberInfo Member, int Depth);

    private sealed class PolymorphicPlan(MemberInfo rootMember, bool rootRegistered, PolymorphicArm[] arms)
    {
        public MemberInfo RootMember { get; } = rootMember;
        public bool RootRegistered { get; } = rootRegistered;
        public PolymorphicArm[] Arms { get; } = arms;
    }

    protected static bool IsCompilerGeneratedClosure(Type type) =>
        type.Attributes.HasFlag(System.Reflection.TypeAttributes.NestedPrivate) &&
        _compilerGeneratedClosureCache.GetValue(type, static t =>
            new StrongBox<bool>(Attribute.IsDefined(t, typeof(CompilerGeneratedAttribute), inherit: true))).Value;
}
