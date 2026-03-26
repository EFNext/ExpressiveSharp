using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using ExpressiveSharp.Extensions;

namespace ExpressiveSharp.Services;

/// <summary>
/// Expression visitor that replaces calls to members marked with <see cref="ExpressiveAttribute"/>
/// with their generated expression tree equivalents. This is the generic base class; EF Core-specific
/// subclasses can override <see cref="VisitMethodCallCore"/> and <see cref="VisitExtension"/> to add
/// query provider awareness.
/// </summary>
public class ExpressiveReplacer : ExpressionVisitor
{
    private readonly IExpressiveResolver _resolver;
    private readonly ExpressionArgumentReplacer _expressionArgumentReplacer = new();
    private readonly Dictionary<MemberInfo, LambdaExpression?> _memberCache = new();
    private readonly HashSet<ConstructorInfo> _expandingConstructors = new();

    private static readonly ConditionalWeakTable<Type, StrongBox<bool>> _compilerGeneratedClosureCache = new();

    public ExpressiveReplacer(IExpressiveResolver resolver)
    {
        _resolver = resolver;
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

        // Allow subclasses to hook into method call processing (e.g., tracking detection).
        VisitMethodCallCore(node);

        var methodInfo = node.Object?.Type.GetConcreteMethod(node.Method) ?? node.Method;

        if (TryGetReflectedExpression(methodInfo, out var reflectedExpression))
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
            _expressionArgumentReplacer.ParameterArgumentMapping.Clear();

            return base.Visit(updatedBody);
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
            !_expandingConstructors.Contains(constructor) &&
            TryGetReflectedExpression(constructor, out var reflectedExpression))
        {
            _expandingConstructors.Add(constructor);
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
                _expandingConstructors.Remove(constructor);
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
            PropertyInfo property when nodeExpression is not null
                => nodeExpression.Type.GetConcreteProperty(property),
            _ => node.Member
        };

        if (TryGetReflectedExpression(nodeMember, out var reflectedExpression))
        {
            if (nodeExpression is not null)
            {
                _expressionArgumentReplacer.ParameterArgumentMapping.Add(reflectedExpression.Parameters[0], nodeExpression);
                var updatedBody = _expressionArgumentReplacer.Visit(reflectedExpression.Body);
                _expressionArgumentReplacer.ParameterArgumentMapping.Clear();

                return base.Visit(updatedBody);
            }

            return base.Visit(reflectedExpression.Body);
        }

        return base.VisitMember(node);
    }

    protected static bool IsCompilerGeneratedClosure(Type type) =>
        type.Attributes.HasFlag(System.Reflection.TypeAttributes.NestedPrivate) &&
        _compilerGeneratedClosureCache.GetValue(type, static t =>
            new StrongBox<bool>(Attribute.IsDefined(t, typeof(CompilerGeneratedAttribute), inherit: true))).Value;
}
