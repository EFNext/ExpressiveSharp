using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using ExpressiveSharp.Generator.Infrastructure;
using ExpressiveSharp.Generator.Models;
using ExpressiveSharp.Generator.SyntaxRewriters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ExpressiveSharp.Generator;

/// <summary>
/// Source generator that intercepts calls to <c>IExpressiveQueryable&lt;T&gt;</c> extension methods
/// that take <c>Func&lt;T,…&gt;</c> delegates (instead of <c>Expression&lt;Func&lt;T,…&gt;&gt;</c>).
/// For each such call-site it emits a <c>[InterceptsLocation]</c> method that rewrites the
/// lambda body through <see cref="ExpressionSyntaxRewriter"/> and forwards the result to the
/// matching <c>Queryable.*</c> overload.
///
/// Any extension method on <c>IExpressiveQueryable&lt;T&gt;</c> that takes a <c>Func&lt;&gt;</c> is
/// intercepted by convention — including library-provided stubs and user-defined ones.
/// </summary>
[Generator]
public class PolyfillInterceptorGenerator : IIncrementalGenerator
{
    private const string IExpressiveQueryableOpenTypeName =
        "ExpressiveSharp.IExpressiveQueryable<T>";

    private const string PolyfillTypeName = "ExpressiveSharp.ExpressionPolyfill";
    private const string PolyfillMethodName = "Create";

    private const string ClosureHelperSource = """

                file static class __ClosureHelper
                {
                    private const global::System.Reflection.BindingFlags F =
                        global::System.Reflection.BindingFlags.Instance |
                        global::System.Reflection.BindingFlags.Public |
                        global::System.Reflection.BindingFlags.NonPublic;

                    internal static global::System.Linq.Expressions.MemberExpression ResolveCapturedThis(
                        global::System.Delegate func, global::System.Type thisType)
                        => FindByType(func.Target, thisType)
                            ?? throw new global::System.InvalidOperationException("Captured 'this' of type '" + thisType.Name + "' not found in closure.");

                    internal static global::System.Linq.Expressions.Expression ResolveCapturedInstanceMember(
                        global::System.Delegate func, global::System.Type thisType, string memberName)
                    {
                        var direct = FindByName(func.Target, memberName);
                        if (direct != null) return direct;
                        // Auto-property backing field: compiler may capture <Name>k__BackingField directly.
                        var backingField = FindByName(func.Target, "<" + memberName + ">k__BackingField");
                        if (backingField != null) return backingField;
                        var thisExpr = FindByType(func.Target, thisType);
                        if (thisExpr != null)
                        {
                            var member = thisType.GetField(memberName, F)
                                      ?? (global::System.Reflection.MemberInfo)thisType.GetProperty(memberName, F);
                            if (member != null)
                                return global::System.Linq.Expressions.Expression.MakeMemberAccess(thisExpr, member);
                        }
                        throw new global::System.InvalidOperationException("Captured member '" + memberName + "' on type '" + thisType.Name + "' not found in closure.");
                    }

                    private static global::System.Linq.Expressions.MemberExpression FindByName(object c, string name)
                    {
                        var field = c.GetType().GetField(name, F);
                        if (field != null)
                            return global::System.Linq.Expressions.Expression.MakeMemberAccess(
                                global::System.Linq.Expressions.Expression.Constant(c), field);
                        foreach (var f in c.GetType().GetFields(F))
                            if (f.FieldType.IsDefined(typeof(global::System.Runtime.CompilerServices.CompilerGeneratedAttribute), true))
                            {
                                var nested = f.GetValue(c);
                                if (nested != null) { var r = FindByName(nested, name); if (r != null) return r; }
                            }
                        return null;
                    }

                    private static global::System.Linq.Expressions.MemberExpression FindByType(object c, global::System.Type t)
                    {
                        foreach (var f in c.GetType().GetFields(F))
                        {
                            if (t.IsAssignableFrom(f.FieldType))
                                return global::System.Linq.Expressions.Expression.MakeMemberAccess(
                                    global::System.Linq.Expressions.Expression.Constant(c), f);
                            if (f.FieldType.IsDefined(typeof(global::System.Runtime.CompilerServices.CompilerGeneratedAttribute), true))
                            {
                                var nested = f.GetValue(c);
                                if (nested != null) { var r = FindByType(nested, t); if (r != null) return r; }
                            }
                        }
                        return null;
                    }
                }
        """;

    // ── IIncrementalGenerator ────────────────────────────────────────────────

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Syntactic pre-filter: any member-access invocation with at least one argument
        // is a candidate. The semantic check in Emit does the real filtering.
        var candidates = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) =>
                node is InvocationExpressionSyntax inv &&
                inv.Expression is MemberAccessExpressionSyntax &&
                inv.ArgumentList.Arguments.Count > 0,
            transform: static (ctx, _) => (InvocationExpressionSyntax)ctx.Node);

        var globalOptions = context.AnalyzerConfigOptionsProvider
            .Select(static (opts, _) => new ExpressiveGlobalOptions(opts.GlobalOptions));

        // Combine with the full compilation so we can do semantic analysis in the output step.
        context.RegisterSourceOutput(
            candidates.Collect().Combine(context.CompilationProvider).Combine(globalOptions),
            static (spc, pair) => Emit(pair.Left.Left, pair.Left.Right, pair.Right, spc));
    }

    // ── Emission ─────────────────────────────────────────────────────────────

    private static void Emit(
        ImmutableArray<InvocationExpressionSyntax> invocations,
        Compilation compilation,
        ExpressiveGlobalOptions globalOptions,
        SourceProductionContext spc)
    {
        var methodsBuilder = new StringBuilder();
        var snippetCount = 0;

        foreach (var inv in invocations)
        {
            spc.CancellationToken.ThrowIfCancellationRequested();

            try
            {
                var snippet = TryEmitPolyfill(inv, compilation, spc, snippetCount, globalOptions)
                           ?? TryEmit(inv, compilation, spc, snippetCount, globalOptions);
                if (snippet is not null)
                {
                    methodsBuilder.Append(snippet);
                    snippetCount++;
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // Report the failure so it doesn't hide bugs silently.
                // The stub body will throw UnreachableException at runtime with a clear message.
                spc.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.InterceptorEmissionFailed,
                    inv.GetLocation(),
                    ex.GetType().Name + ": " + ex.Message));
            }
        }

        if (snippetCount == 0) return;

        var methods = methodsBuilder.ToString();
        var closureHelper = methods.Contains("__ClosureHelper") ? ClosureHelperSource : "";
        var source = $$"""
            // <auto-generated/>
            #nullable disable

            namespace ExpressiveSharp.Generated.Interceptors
            {
                internal static class PolyfillInterceptors
                {
            {{methods}}    }{{closureHelper}}
            }

            namespace System.Runtime.CompilerServices
            {
                [global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = true)]
                file sealed class InterceptsLocationAttribute : global::System.Attribute
                {
                    public InterceptsLocationAttribute(int version, string data) { }
                }
            }
            """;

        spc.AddSource("PolyfillInterceptors.g.cs",
            SourceText.From(source, Encoding.UTF8));
    }

    // ── ExpressionPolyfill.Create() interception ──────────────────────────

    /// <summary>
    /// Detects calls to <c>ExpressionPolyfill.Polyfill&lt;TDelegate&gt;(lambda)</c> and emits
    /// an interceptor that returns a literal <c>Expression&lt;TDelegate&gt;</c> with the
    /// rewritten lambda body.
    /// </summary>
    private static string? TryEmitPolyfill(
        InvocationExpressionSyntax inv,
        Compilation compilation,
        SourceProductionContext spc,
        int index,
        ExpressiveGlobalOptions globalOptions)
    {
        var model = compilation.GetSemanticModel(inv.SyntaxTree);

        if (model.GetSymbolInfo(inv).Symbol is not IMethodSymbol method)
            return null;

        // Must be ExpressionPolyfill.Create<TDelegate>(...)
        if (method.Name != PolyfillMethodName)
            return null;
        if (method.ContainingType?.ToDisplayString() != PolyfillTypeName)
            return null;
        if (method.TypeArguments.Length != 1)
            return null;

        // First argument must be a lambda; optional second is params transformers
        if (inv.ArgumentList.Arguments.Count < 1)
            return null;
        if (inv.ArgumentList.Arguments[0].Expression is not LambdaExpressionSyntax lam)
            return null;
        var hasTransformers = method.Parameters.Length == 2;

        var interceptableLocation = Microsoft.CodeAnalysis.CSharp.CSharpExtensions.GetInterceptableLocation(
            model, inv, spc.CancellationToken);
        if (interceptableLocation is null)
            return null;
        var interceptAttr = Microsoft.CodeAnalysis.CSharp.CSharpExtensions
            .GetInterceptsLocationAttributeSyntax(interceptableLocation);

        // Extract the delegate type — e.g., Func<Order, bool>
        var delegateType = method.TypeArguments[0];
        var delegateFqn = delegateType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        // Default to Rewrite (the whole point of Polyfill is enabling expression tree features),
        // but allow MSBuild global property to override.
        

        // Get the parameter types from the delegate to build Expression<TDelegate>
        // For Func<T1, T2, ..., TResult>, the lambda parameters map to T1..Tn
        if (delegateType is not INamedTypeSymbol delegateNamed || delegateNamed.TypeArguments.IsEmpty)
            return null;

        // Use the first type argument as the "element" type for the rewriter
        var elemType = delegateNamed.TypeArguments[0];
        if (elemType is not INamedTypeSymbol elemSymbol)
            return null;

        // Use ExpressionTreeEmitter to build the expression tree
        var exprTypeFqn = $"global::System.Linq.Expressions.Expression<{delegateFqn}>";
        var emitResult = EmitLambdaBody(lam, elemSymbol, model, spc, globalOptions, delegateFqn,
            varPrefix: $"i{index}_", delegateVarName: "__func");
        if (emitResult is null)
            return null;

        if (hasTransformers)
        {
            return $$"""
                    {{interceptAttr}}
                    internal static global::System.Linq.Expressions.Expression<{{delegateFqn}}> {{MethodId("Create", index)}}(
                        {{delegateFqn}} __func,
                        params global::ExpressiveSharp.IExpressionTreeTransformer[] transformers)
                    {
            {{emitResult.Body}}            global::System.Linq.Expressions.Expression result = __lambda;
                        foreach (var t in transformers) result = t.Transform(result);
                        return (global::System.Linq.Expressions.Expression<{{delegateFqn}}>)result;
                    }

            """;
        }

        return $$"""
                {{interceptAttr}}
                internal static global::System.Linq.Expressions.Expression<{{delegateFqn}}> {{MethodId("Create", index)}}(
                    {{delegateFqn}} __func)
                {
        {{emitResult.Body}}            return __lambda;
                }

        """;
    }

    // ── Per-invocation dispatch (IExpressiveQueryable) ───────────────────────

    private static string? TryEmit(
        InvocationExpressionSyntax inv,
        Compilation compilation,
        SourceProductionContext spc,
        int index,
        ExpressiveGlobalOptions globalOptions)
    {
        var model = compilation.GetSemanticModel(inv.SyntaxTree);
        var ma = (MemberAccessExpressionSyntax)inv.Expression;

        // Receiver must be or implement IExpressiveQueryable<T>.
        if (model.GetTypeInfo(ma.Expression).Type is not INamedTypeSymbol receiverType)
            return null;

        // Check both the type itself and its implemented interfaces
        if (!IsExpressiveQueryable(receiverType))
            return null;

        if (model.GetSymbolInfo(inv).Symbol is not IMethodSymbol method)
            return null;

        // The stub convention: at least one non-receiver parameter must be a Func<> delegate,
        // not an Expression<Func<>>. This distinguishes user/library IExpressiveQueryable<T> stubs
        // from regular IQueryable<T> extension methods.
        // For most methods, Func<> is Parameters[0]. For Join/GroupJoin/Zip/ExceptBy etc.,
        // it may be at a later position (e.g., Parameters[1]) after an IEnumerable<> arg.
        if (method.Parameters.IsEmpty) return null;
        var funcParamIndices = new List<int>();
        for (int i = 0; i < method.Parameters.Length; i++)
        {
            if (method.Parameters[i].Type is INamedTypeSymbol pt &&
                pt.ConstructedFrom.Name == "Func" &&
                pt.ConstructedFrom.ContainingNamespace?.ToDisplayString() == "System")
            {
                funcParamIndices.Add(i);
            }
        }
        if (funcParamIndices.Count == 0) return null;

        // Get the interceptable location using the Roslyn 5.0+ API.
        // This produces the correct [InterceptsLocation(version, data)] attribute text.
        var interceptableLocation = Microsoft.CodeAnalysis.CSharp.CSharpExtensions.GetInterceptableLocation(
            model, inv, spc.CancellationToken);

        if (interceptableLocation is null)
            return null;

        var interceptAttr = Microsoft.CodeAnalysis.CSharp.CSharpExtensions
            .GetInterceptsLocationAttributeSyntax(interceptableLocation);

        // Element type T of IExpressiveQueryable<T>.
        // The receiver may be IExpressiveQueryable<T> directly, or a type that implements it.
        var rewritableInterface = GetExpressiveQueryableInterface(receiverType);
        if (rewritableInterface is null)
            return null;
        var elementType = rewritableInterface.TypeArguments[0];
        if (elementType is not INamedTypeSymbol elementSymbol)
            return null; // Skip when T is itself a type parameter
        var elementFqn = elementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var methodName = ma.Name.Identifier.Text;

        // Per-call AsExpressive() → MSBuild global → hardcoded Ignore fallback.

        // Resolve the target type for the Queryable.* call. Defaults to System.Linq.Queryable
        // unless overridden by [PolyfillTarget(typeof(...))] on the stub method.
        var targetTypeFqn = "global::System.Linq.Queryable";
        var polyfillAttr = method.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "PolyfillTargetAttribute");
        if (polyfillAttr?.ConstructorArguments.Length > 0 &&
            polyfillAttr.ConstructorArguments[0].Value is INamedTypeSymbol targetType)
        {
            targetTypeFqn = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        return EmitGenericLambda(inv, model, spc, interceptAttr, index, methodName, elementSymbol, elementFqn, method, funcParamIndices, targetTypeFqn, globalOptions);
    }

    // ── Lambda helpers ───────────────────────────────────────────────────────

    /// <summary>
    /// Uses <see cref="Emitter.ExpressionTreeEmitter"/> to build the expression tree for a lambda body.
    /// Returns the emitter result containing imperative Expression.* factory code, or null on failure.
    /// </summary>
    private static Emitter.EmitResult? EmitLambdaBody(
        LambdaExpressionSyntax lambda,
        INamedTypeSymbol elementSymbol,

        SemanticModel model,
        SourceProductionContext spc,
        ExpressiveGlobalOptions globalOptions,
        string delegateTypeFqn,
        string assignToVariable = "__lambda",
        string varPrefix = "",
        IReadOnlyDictionary<ITypeSymbol, string>? typeAliases = null,
        string? delegateVarName = null)
    {
        // For expression-bodied lambdas, use the expression directly.
        // For block-bodied lambdas, use the block.
        var bodyNode = lambda.Body is ExpressionSyntax expr ? (SyntaxNode)expr : lambda.Body;
        if (bodyNode is null) return null;

        var emitter = new Emitter.ExpressionTreeEmitter(model, spc, varPrefix: varPrefix, delegateVarName: delegateVarName);

        if (typeAliases is not null)
        {
            foreach (var kvp in typeAliases)
                emitter.RegisterTypeAlias(kvp.Key, kvp.Value);
        }

        var emitterParams = new List<Emitter.EmitterParameter>();
        if (lambda is SimpleLambdaExpressionSyntax simple)
        {
            var paramSymbol = model.GetDeclaredSymbol(simple.Parameter);
            var paramTypeFqn = (typeAliases is not null && typeAliases.TryGetValue(elementSymbol, out var elemAlias))
                ? elemAlias
                : elementSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            emitterParams.Add(new Emitter.EmitterParameter(
                simple.Parameter.Identifier.Text, paramTypeFqn,
                symbol: paramSymbol as IParameterSymbol));
        }
        else if (lambda is ParenthesizedLambdaExpressionSyntax parens)
        {
            foreach (var param in parens.ParameterList.Parameters)
            {
                var paramSymbol = model.GetDeclaredSymbol(param) as IParameterSymbol;
                var paramTypeFqn = paramSymbol?.Type is not null
                    ? (typeAliases is not null && typeAliases.TryGetValue(paramSymbol.Type, out var paramAlias)
                        ? paramAlias
                        : paramSymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                    : "object";
                emitterParams.Add(new Emitter.EmitterParameter(
                    param.Identifier.Text, paramTypeFqn, symbol: paramSymbol));
            }
        }

        // Determine return type from delegate type args, resolving through aliases for anonymous types.
        var returnTypeFqn = "object";
        if (elementSymbol.ContainingNamespace is not null)
        {
            // Try to get the return type from the lambda's actual type info
            var typeInfo = model.GetTypeInfo(lambda);
            if (typeInfo.ConvertedType is INamedTypeSymbol convertedType &&
                convertedType.TypeArguments.Length > 0)
            {
                var returnTypeSymbol = convertedType.TypeArguments[convertedType.TypeArguments.Length - 1];
                if (typeAliases is not null && typeAliases.TryGetValue(returnTypeSymbol, out var aliasedReturn))
                    returnTypeFqn = aliasedReturn;
                else
                    returnTypeFqn = returnTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }
        }

        var result = emitter.Emit(bodyNode, emitterParams, returnTypeFqn, delegateTypeFqn,
            assignToVariable: assignToVariable);

        // Prepend source comment showing the original lambda
        var sourceText = lambda.NormalizeWhitespace().ToFullString()
            .Replace("\r", "").Replace("\n", " ");
        var commentLine = $"            // Source: {sourceText}\n";
        return new Emitter.EmitResult(commentLine + result.Body);
    }

    // ── Type helpers ─────────────────────────────────────────────────────────

    /// <summary>
    /// Returns true if <paramref name="type"/> is an anonymous type, which cannot be
    /// named in C# source and requires a generic type parameter in the interceptor signature.
    /// </summary>
    private static bool IsAnonymousType(ITypeSymbol type)
        => type is INamedTypeSymbol { IsAnonymousType: true };

    // ── Formatting helpers ───────────────────────────────────────────────────

    private static string MethodId(string op, int index) => $"__Polyfill_{op}_{index}";

    /// <summary>
    /// Generic lambda emitter for all LINQ operators and user-defined stubs.
    /// Handles any number of Func&lt;&gt; parameters (single or multi-lambda).
    /// Convention: the interceptor calls <c>Queryable.{methodName}</c> with the same name.
    /// The return type is taken from the stub's declared return type.
    /// Supports methods where the Func&lt;&gt; parameters are not necessarily at position 0
    /// (e.g., ExceptBy, Zip, AggregateBy where non-lambda args are interleaved).
    /// Non-lambda parameters are forwarded directly to the Queryable.* call.
    /// </summary>
    private static string? EmitGenericLambda(
        InvocationExpressionSyntax inv, SemanticModel model,
        SourceProductionContext spc, string interceptAttr, int idx,
        string methodName, INamedTypeSymbol elemSym, string elemFqn,
        IMethodSymbol method, List<int> funcParamIndices, string targetTypeFqn,
        ExpressiveGlobalOptions globalOptions)
    {
        // ── Extract all lambda expressions from the Func<> argument positions ──
        var lambdas = new List<LambdaExpressionSyntax>(funcParamIndices.Count);
        for (int i = 0; i < funcParamIndices.Count; i++)
        {
            if (inv.ArgumentList.Arguments[funcParamIndices[i]].Expression is not LambdaExpressionSyntax lam)
                return null;
            lambdas.Add(lam);
        }

        bool single = funcParamIndices.Count == 1;

        // ── Detect anonymous types across all Func<> type args ──
        var hasAnyAnon = elemSym.IsAnonymousType;
        for (int i = 0; i < funcParamIndices.Count; i++)
        {
            var fta = ((INamedTypeSymbol)method.Parameters[funcParamIndices[i]].Type).TypeArguments;
            for (int j = 0; j < fta.Length; j++)
                hasAnyAnon = hasAnyAnon || IsAnonymousType(fta[j]);
        }

        // Also check non-Func parameter types (e.g., anonymous seed in AggregateBy).
        for (int i = 0; i < method.Parameters.Length; i++)
        {
            if (!funcParamIndices.Contains(i))
                hasAnyAnon = hasAnyAnon || IsAnonymousType(method.Parameters[i].Type);
        }

        // Determine if the stub returns IExpressiveQueryable<X> (queryable) or a scalar type.
        var isRewritableReturn = method.ReturnType is INamedTypeSymbol rqType
            && rqType.ConstructedFrom.ToDisplayString() == IExpressiveQueryableOpenTypeName;

        ITypeSymbol? returnElemType = null;
        if (isRewritableReturn)
        {
            returnElemType = ((INamedTypeSymbol)method.ReturnType).TypeArguments[0];
            hasAnyAnon = hasAnyAnon || IsAnonymousType(returnElemType);
        }

        // For scalar returns, the FQN of the return type itself.
        var scalarReturnFqn = method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        // ThenBy/ThenByDescending require IOrderedQueryable<T>.
        var isOrdered = methodName is "ThenBy" or "ThenByDescending";

        // ── Build type aliases, per-lambda delegate FQNs, param lists ──
        var methodTypeArgs = method.TypeArguments;
        var typeAliases = new Dictionary<ITypeSymbol, string>(SymbolEqualityComparer.Default);
        var delegateFqns = new string[funcParamIndices.Count];
        string elemRef; // element type reference (alias or FQN)
        string castFqn;
        string typeParams;
        string returnRef;
        string interceptorParamList;
        string queryableArgList;

        if (hasAnyAnon)
        {
            // ALL method type args become generic params (T0, T1, …) to match interceptor arity.
            // Anonymous types get aliases for EmitLambdaBody; concrete types still appear as
            // generic params in the interceptor signature so the C# compiler can infer them.
            var typeParamNames = new string[methodTypeArgs.Length];
            for (int i = 0; i < methodTypeArgs.Length; i++)
                typeParamNames[i] = $"T{i}";
            typeParams = $"<{string.Join(", ", typeParamNames)}>";

            // Register ALL method type args as aliases so EmitLambdaBody and the
            // Queryable.* forwarding call use consistent generic param references.
            if (!typeAliases.ContainsKey(elemSym))
                typeAliases[elemSym] = typeParamNames[0];
            for (int i = 0; i < methodTypeArgs.Length; i++)
            {
                if (!typeAliases.ContainsKey(methodTypeArgs[i]))
                    typeAliases[methodTypeArgs[i]] = typeParamNames[i];
            }

            // Element reference: use alias (all method type args are aliased).
            if (typeAliases.TryGetValue(elemSym, out var ep))
                elemRef = ep;
            else
                elemRef = elemFqn;

            // Build per-lambda delegate FQN strings.
            // Interceptor signature form: ALL types as generic params (ensures inference).
            // EmitLambdaBody form: anonymous → alias, concrete → FQN.
            var funcFqnGenerics = new string[funcParamIndices.Count];
            for (int fi = 0; fi < funcParamIndices.Count; fi++)
            {
                var funcTypeArgs = ((INamedTypeSymbol)method.Parameters[funcParamIndices[fi]].Type).TypeArguments;
                var sigParts = new string[funcTypeArgs.Length];
                for (int i = 0; i < funcTypeArgs.Length; i++)
                {
                    // Use alias if the type is registered; concrete FQN otherwise.
                    if (typeAliases.TryGetValue(funcTypeArgs[i], out var gp))
                        sigParts[i] = gp;
                    else
                        sigParts[i] = funcTypeArgs[i].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                }
                funcFqnGenerics[fi] = "global::System.Func<" + string.Join(", ", sigParts) + ">";
                // Use same generic form for EmitLambdaBody so lambda types match the Queryable.* call.
                delegateFqns[fi] = funcFqnGenerics[fi];
            }

            castFqn = isOrdered
                ? $"global::System.Linq.IOrderedQueryable<{elemRef}>"
                : $"global::System.Linq.IQueryable<{elemRef}>";

            // Return type: use alias if available.
            if (isRewritableReturn)
            {
                if (typeAliases.TryGetValue(returnElemType!, out var retParam))
                    returnRef = retParam;
                else
                    returnRef = returnElemType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }
            else
            {
                returnRef = scalarReturnFqn;
            }

            // Build parameter and argument lists, replacing each Func<> position.
            var interceptorParams = new List<string>();
            var queryableArgs = new List<string>();
            int funcOrdinal = 0;
            for (int i = 0; i < method.Parameters.Length; i++)
            {
                if (funcOrdinal < funcParamIndices.Count && i == funcParamIndices[funcOrdinal])
                {
                    var delegateName = single ? "__func" : $"__func{funcOrdinal + 1}";
                    interceptorParams.Add($"{funcFqnGenerics[funcOrdinal]} {delegateName}");
                    queryableArgs.Add(single ? "__lambda" : $"__lambda{funcOrdinal + 1}");
                    funcOrdinal++;
                }
                else
                {
                    var paramType = method.Parameters[i].Type;
                    var paramTypeFqn = ResolveTypeFqn(paramType, typeAliases);
                    var paramName = method.Parameters[i].Name;
                    interceptorParams.Add($"{paramTypeFqn} {paramName}");
                    queryableArgs.Add(paramName);
                }
            }
            interceptorParamList = string.Join(",\n                    ", interceptorParams);
            queryableArgList = string.Join(",\n                        ", queryableArgs);
        }
        else
        {
            typeParams = "";
            elemRef = elemFqn;

            // Build per-lambda delegate FQN strings with concrete types.
            for (int fi = 0; fi < funcParamIndices.Count; fi++)
            {
                var funcTypeArgs = ((INamedTypeSymbol)method.Parameters[funcParamIndices[fi]].Type).TypeArguments;
                delegateFqns[fi] = "global::System.Func<" +
                    string.Join(", ", funcTypeArgs.Select(t =>
                        t.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))) + ">";
            }

            castFqn = isOrdered
                ? $"global::System.Linq.IOrderedQueryable<{elemFqn}>"
                : $"global::System.Linq.IQueryable<{elemFqn}>";

            returnRef = isRewritableReturn
                ? returnElemType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                : scalarReturnFqn;

            // Build parameter and argument lists with concrete Func<> types.
            var interceptorParams = new List<string>();
            var queryableArgs = new List<string>();
            int funcOrdinal = 0;
            for (int i = 0; i < method.Parameters.Length; i++)
            {
                var paramTypeFqn = method.Parameters[i].Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                if (funcOrdinal < funcParamIndices.Count && i == funcParamIndices[funcOrdinal])
                {
                    var delegateName = single ? "__func" : $"__func{funcOrdinal + 1}";
                    interceptorParams.Add($"{paramTypeFqn} {delegateName}");
                    queryableArgs.Add(single ? "__lambda" : $"__lambda{funcOrdinal + 1}");
                    funcOrdinal++;
                }
                else
                {
                    var paramName = method.Parameters[i].Name;
                    interceptorParams.Add($"{paramTypeFqn} {paramName}");
                    queryableArgs.Add(paramName);
                }
            }
            interceptorParamList = string.Join(",\n                    ", interceptorParams);
            queryableArgList = string.Join(",\n                        ", queryableArgs);
        }

        // ── Emit all lambda bodies ──
        // For simple (single-parameter) lambdas, EmitLambdaBody uses the elementSymbol
        // to determine the parameter type. For the first Func<> that's the source element,
        // but for subsequent Func<> params (e.g., AggregateBy's seedSelector Func<TKey, TAcc>)
        // the first type arg is NOT the source element. Derive the correct symbol per-lambda.
        var emitBodies = new List<string>(funcParamIndices.Count);
        for (int j = 0; j < funcParamIndices.Count; j++)
        {
            var lambdaVar = single ? "__lambda" : $"__lambda{j + 1}";
            var prefix = single ? $"i{idx}_" : $"i{idx}{(char)('a' + j)}_";
            var delegateName = single ? "__func" : $"__func{j + 1}";
            var funcType = (INamedTypeSymbol)method.Parameters[funcParamIndices[j]].Type;
            var lambdaElemSym = funcType.TypeArguments[0] as INamedTypeSymbol ?? elemSym;
            var emitResult = EmitLambdaBody(lambdas[j], lambdaElemSym, model, spc, globalOptions,
                delegateFqns[j], lambdaVar, varPrefix: prefix,
                typeAliases: hasAnyAnon ? typeAliases : null,
                delegateVarName: delegateName);
            if (emitResult is null) return null;
            emitBodies.Add(emitResult.Body);
        }
        var allBodies = string.Concat(emitBodies);

        // ── Templates ──
        if (!hasAnyAnon)
        {
            if (isRewritableReturn)
            {
                return $$"""
                        {{interceptAttr}}
                        internal static global::ExpressiveSharp.IExpressiveQueryable<{{returnRef}}> {{MethodId(methodName, idx)}}(
                            this global::ExpressiveSharp.IExpressiveQueryable<{{elemFqn}}> source,
                            {{interceptorParamList}})
                        {
                {{allBodies}}            return global::ExpressiveSharp.Extensions.ExpressiveQueryableExtensions.AsExpressive(
                                {{targetTypeFqn}}.{{methodName}}(
                                    ({{castFqn}})source,
                                    {{queryableArgList}}));
                        }

                """;
            }

            return $$"""
                    {{interceptAttr}}
                    internal static {{returnRef}} {{MethodId(methodName, idx)}}(
                        this global::ExpressiveSharp.IExpressiveQueryable<{{elemFqn}}> source,
                        {{interceptorParamList}})
                    {
            {{allBodies}}            return {{targetTypeFqn}}.{{methodName}}(
                                ({{castFqn}})source,
                                {{queryableArgList}});
                    }

            """;
        }

        if (isRewritableReturn)
        {
            return $$"""
                    {{interceptAttr}}
                    internal static global::ExpressiveSharp.IExpressiveQueryable<{{returnRef}}> {{MethodId(methodName, idx)}}{{typeParams}}(
                        this global::ExpressiveSharp.IExpressiveQueryable<{{elemRef}}> source,
                        {{interceptorParamList}})
                    {
            {{allBodies}}            return (global::ExpressiveSharp.IExpressiveQueryable<{{returnRef}}>)(object)
                            global::ExpressiveSharp.Extensions.ExpressiveQueryableExtensions.AsExpressive(
                                {{targetTypeFqn}}.{{methodName}}(
                                    ({{castFqn}})(object)source,
                                    {{queryableArgList}}));
                    }

            """;
        }

        return $$"""
                {{interceptAttr}}
                internal static {{returnRef}} {{MethodId(methodName, idx)}}{{typeParams}}(
                    this global::ExpressiveSharp.IExpressiveQueryable<{{elemRef}}> source,
                    {{interceptorParamList}})
                {
        {{allBodies}}            return {{targetTypeFqn}}.{{methodName}}(
                            ({{castFqn}})(object)source,
                            {{queryableArgList}});
                }

        """;
    }

    /// <summary>
    /// Returns true if <paramref name="type"/> is or implements <c>IExpressiveQueryable&lt;T&gt;</c>.
    /// </summary>
    private static bool IsExpressiveQueryable(INamedTypeSymbol type)
    {
        if (type.ConstructedFrom.ToDisplayString() == IExpressiveQueryableOpenTypeName)
            return true;

        return type.AllInterfaces.Any(i =>
            i.ConstructedFrom.ToDisplayString() == IExpressiveQueryableOpenTypeName);
    }

    /// <summary>
    /// Finds the <c>IExpressiveQueryable&lt;T&gt;</c> interface on <paramref name="type"/>,
    /// or returns <paramref name="type"/> itself if it is <c>IExpressiveQueryable&lt;T&gt;</c>.
    /// </summary>
    private static INamedTypeSymbol? GetExpressiveQueryableInterface(INamedTypeSymbol type)
    {
        if (type.ConstructedFrom.ToDisplayString() == IExpressiveQueryableOpenTypeName)
            return type;

        return type.AllInterfaces.FirstOrDefault(i =>
            i.ConstructedFrom.ToDisplayString() == IExpressiveQueryableOpenTypeName);
    }

    /// <summary>
    /// Resolves a type's FQN, substituting type arguments through aliases.
    /// For <c>IEnumerable&lt;Customer&gt;</c> where Customer→T1, returns <c>IEnumerable&lt;T1&gt;</c>.
    /// </summary>
    private static string ResolveTypeFqn(ITypeSymbol type, Dictionary<ITypeSymbol, string> typeAliases)
    {
        // Direct match (e.g., the type itself is aliased).
        if (typeAliases.TryGetValue(type, out var alias))
            return alias;

        // Constructed generic type: resolve each type argument.
        if (type is INamedTypeSymbol named && named.TypeArguments.Length > 0)
        {
            bool anyResolved = false;
            var resolvedArgs = new string[named.TypeArguments.Length];
            for (int i = 0; i < named.TypeArguments.Length; i++)
            {
                if (typeAliases.TryGetValue(named.TypeArguments[i], out var argAlias))
                {
                    resolvedArgs[i] = argAlias;
                    anyResolved = true;
                }
                else
                {
                    resolvedArgs[i] = named.TypeArguments[i].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                }
            }
            if (anyResolved)
            {
                var openType = named.ConstructedFrom.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                // Remove the existing type args from the FQN (everything from the last '<').
                var idx = openType.LastIndexOf('<');
                if (idx >= 0)
                    openType = openType.Substring(0, idx);
                return openType + "<" + string.Join(", ", resolvedArgs) + ">";
            }
        }

        return type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

}
