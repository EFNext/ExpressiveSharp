using System.Collections.Immutable;
using System.Text;
using ExpressiveSharp.Generator.Comparers;
using ExpressiveSharp.Generator.Emitter;
using ExpressiveSharp.Generator.Infrastructure;
using ExpressiveSharp.Generator.Interpretation;
using ExpressiveSharp.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Text;

namespace ExpressiveSharp.Generator;

[Generator]
public class PolyfillInterceptorGenerator : IIncrementalGenerator
{
    private const string IExpressiveQueryableOpenTypeName =
        "ExpressiveSharp.IExpressiveQueryable<T>";

    private const string PolyfillTypeName = "ExpressiveSharp.ExpressionPolyfill";
    private const string PolyfillMethodName = "Create";

    private const string ExpressivePropertyAttributeName = "ExpressiveSharp.Mapping.ExpressivePropertyAttribute";

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

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // One pipeline item per source file; drop files with no lambda-bearing invocations
        // before semantic binding (every intercepted call site has at least one lambda arg).
        var candidateFiles = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) => node is CompilationUnitSyntax,
            transform: static (ctx, ct) =>
            {
                ct.ThrowIfCancellationRequested();
                var unit = (CompilationUnitSyntax)ctx.Node;
                foreach (var descendant in unit.DescendantNodes())
                {
                    if (descendant is InvocationExpressionSyntax inv &&
                        inv.Expression is MemberAccessExpressionSyntax &&
                        HasLambdaArgument(inv))
                        return unit;
                }
                return null;
            })
            .Where(static x => x is not null)
            .Select(static (x, _) => x!);

        // Reference equality on the CompilationUnitSyntax root: Roslyn keeps unchanged files'
        // syntax tree roots as the same object across incremental runs, so editing a noise
        // file leaves all other (file, compilation) pairs equal and skips re-emission.
        var filesWithCompilation = candidateFiles
            .Combine(context.CompilationProvider)
            .WithComparer(CompilationUnitAndCompilationComparer.Instance);

        // Source generators don't see each other's AddSource output, so SemanticModel can't bind
        // references to ExpressiveGenerator's synthesized [ExpressiveProperty] partials. Mirror
        // its synthesis here and augment our local compilation for binding only — never AddSource.
        var synthesizedDeclarations = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                ExpressivePropertyAttributeName,
                predicate: static (s, _) => s is PropertyDeclarationSyntax,
                transform: static (c, _) => (
                    Stub: (PropertyDeclarationSyntax)c.TargetNode,
                    Attribute: new ExpressivePropertyAttributeData(c.Attributes[0])
                ));

        var synthesizedSources = synthesizedDeclarations
            .Combine(context.CompilationProvider)
            .Select(static (pair, ct) =>
            {
                ct.ThrowIfCancellationRequested();
                var ((stub, attribute), compilation) = pair;
                var semanticModel = compilation.GetSemanticModel(stub.SyntaxTree);
                if (semanticModel.GetDeclaredSymbol(stub, ct) is not IPropertySymbol stubSymbol)
                    return default;

                var spec = ExpressivePropertyInterpreter.TryBuildSpec(stub, stubSymbol, attribute);
                if (spec is null) return default;

                var hint = $"{spec.ContainingTypeName}.{spec.PropertyName}.Synthesized.augment.cs";
                var source = SynthesizedPropertyEmitter.BuildSource(spec);
                return (HintName: hint, Source: source);
            })
            .Where(static t => t.Source is not null)
            .Collect()
            .WithComparer(SynthesizedSourceArrayComparer.Instance);

        var filesWithCompilationAndSynth = filesWithCompilation
            .Combine(synthesizedSources)
            .WithComparer(FileAndSynthesizedSourcesComparer.Instance);

        context.RegisterSourceOutput(filesWithCompilationAndSynth,
            static (spc, pair) => ProcessFileAndEmit(pair.Left.Left, pair.Left.Right, pair.Right, spc));
    }

    private static void ProcessFileAndEmit(
        CompilationUnitSyntax unit,
        Compilation compilation,
        ImmutableArray<(string HintName, string Source)> synthesizedSources,
        SourceProductionContext spc)
    {
        var ct = spc.CancellationToken;

        // Augment in-memory only so SemanticModel can bind to synthesized [ExpressiveProperty]
        // members — the augmented Compilation never escapes via AddSource.
        var modelCompilation = compilation;
        if (synthesizedSources.Length > 0)
        {
            var parseOptions = compilation.SyntaxTrees.FirstOrDefault()?.Options as CSharpParseOptions
                ?? CSharpParseOptions.Default;
            var trees = new SyntaxTree[synthesizedSources.Length];
            for (var i = 0; i < synthesizedSources.Length; i++)
            {
                trees[i] = CSharpSyntaxTree.ParseText(
                    synthesizedSources[i].Source,
                    parseOptions,
                    cancellationToken: ct);
            }
            modelCompilation = compilation.AddSyntaxTrees(trees);
        }

        var model = modelCompilation.GetSemanticModel(unit.SyntaxTree);
        var sourcePath = unit.SyntaxTree.FilePath;
        var fileTag = GetFileTag(sourcePath);

        var methodCodes = new List<string>();
        var needsClosureHelper = false;

        foreach (var descendant in unit.DescendantNodes())
        {
            if (descendant is not InvocationExpressionSyntax inv) continue;
            if (inv.Expression is not MemberAccessExpressionSyntax ma) continue;
            if (!HasLambdaArgument(inv)) continue;

            ct.ThrowIfCancellationRequested();
            try
            {
                // Method-name token position (not invocation span start, which collides on chained calls).
                var nameLineSpan = ma.Name.GetLocation().GetLineSpan();
                var line = nameLineSpan.StartLinePosition.Line;
                var col  = nameLineSpan.StartLinePosition.Character;

                if (model.GetSymbolInfo(inv).Symbol is not IMethodSymbol method) continue;

                string? methodCode;
                if (ma.Name.Identifier.Text == PolyfillMethodName &&
                    method.ContainingType?.ToDisplayString() == PolyfillTypeName)
                {
                    methodCode = TryEmitPolyfill(inv, model, method, line, col, fileTag, spc);
                }
                else
                {
                    methodCode = TryEmit(inv, ma, model, method, line, col, fileTag, spc);
                }

                if (methodCode is null) continue;

                methodCodes.Add(methodCode);
                if (methodCode.Contains("__ClosureHelper"))
                    needsClosureHelper = true;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.InterceptorEmissionFailed,
                    inv.GetLocation(),
                    ex.GetType().Name + ": " + ex.Message));
            }
        }

        if (methodCodes.Count == 0) return;

        var allMethods = string.Concat(methodCodes);
        var closureHelper = needsClosureHelper ? ClosureHelperSource : "";
        var source = $$"""
            // <auto-generated/>
            #nullable disable

            namespace ExpressiveSharp.Generated.Interceptors
            {
                internal static partial class PolyfillInterceptors
                {
            {{allMethods}}    }{{closureHelper}}
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

        spc.AddSource(ComputeOutputFileName(sourcePath),
            SourceText.From(source, Encoding.UTF8));
    }

    private static string ComputeOutputFileName(string sourcePath)
    {
        unchecked
        {
            var hash = 2166136261u;
            for (var i = 0; i < sourcePath.Length; i++)
            {
                hash ^= (uint)sourcePath[i];
                hash *= 16777619u;
            }
            return "PolyfillInterceptors_" + hash.ToString("x8") + ".g.cs";
        }
    }

    /// <summary>4-char hex tag from source path hash; disambiguates same-line/col call sites across files.</summary>
    private static string GetFileTag(string sourcePath)
    {
        unchecked
        {
            var hash = 2166136261u;
            for (var i = 0; i < sourcePath.Length; i++)
            {
                hash ^= (uint)sourcePath[i];
                hash *= 16777619u;
            }
            return (hash & 0xFFFFu).ToString("x4");
        }
    }

    private static string? TryEmitPolyfill(InvocationExpressionSyntax inv,
        SemanticModel model,
        IMethodSymbol method,
        int line,
        int col,
        string fileTag,
        SourceProductionContext spc)
    {
        if (method.TypeArguments.Length != 1)
            return null;

        // First arg is the lambda; optional second is params transformers.
        if (inv.ArgumentList.Arguments.Count < 1)
            return null;
        if (inv.ArgumentList.Arguments[0].Expression is not LambdaExpressionSyntax lam)
            return null;
        var hasTransformers = method.Parameters.Length == 2;

        var interceptableLocation = model.GetInterceptableLocation(inv, spc.CancellationToken);
        if (interceptableLocation is null)
            return null;
        var interceptAttr = interceptableLocation.GetInterceptsLocationAttributeSyntax();

        var delegateType = method.TypeArguments[0];
        var delegateFqn = delegateType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        if (delegateType is not INamedTypeSymbol delegateNamed || delegateNamed.TypeArguments.IsEmpty)
            return null;

        var elemType = delegateNamed.TypeArguments[0];
        if (elemType is not INamedTypeSymbol elemSymbol)
            return null;

        var emitResult = EmitLambdaBody(lam, elemSymbol, model, spc, delegateFqn,
            varPrefix: $"i{fileTag}{line}c{col}_", delegateVarName: "__func");
        if (emitResult is null)
            return null;

        if (hasTransformers)
        {
            return $$"""
                    {{interceptAttr}}
                    internal static global::System.Linq.Expressions.Expression<{{delegateFqn}}> {{MethodId("Create", fileTag, line, col)}}(
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
                internal static global::System.Linq.Expressions.Expression<{{delegateFqn}}> {{MethodId("Create", fileTag, line, col)}}(
                    {{delegateFqn}} __func)
                {
        {{emitResult.Body}}            return __lambda;
                }

        """;
    }

    private static string? TryEmit(InvocationExpressionSyntax inv,
        MemberAccessExpressionSyntax ma,
        SemanticModel model,
        IMethodSymbol method,
        int line,
        int col,
        string fileTag,
        SourceProductionContext spc)
    {
        if (model.GetTypeInfo(ma.Expression).Type is not INamedTypeSymbol receiverType)
            return null;

        if (!IsExpressiveQueryable(receiverType))
            return null;

        // Stub convention: at least one Func<> param distinguishes our stubs from regular IQueryable
        // extensions. The Func<> may not be Parameters[0] (e.g. Join/Zip/ExceptBy).
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

        var interceptableLocation = model.GetInterceptableLocation(inv, spc.CancellationToken);

        if (interceptableLocation is null)
            return null;

        var interceptAttr = interceptableLocation.GetInterceptsLocationAttributeSyntax();

        var rewritableInterface = GetExpressiveQueryableInterface(receiverType);
        if (rewritableInterface is null)
            return null;
        var elementType = rewritableInterface.TypeArguments[0];
        if (elementType is not INamedTypeSymbol elementSymbol)
            return null;
        var elementFqn = elementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var methodName = ma.Name.Identifier.Text;

        // Defaults to System.Linq.Queryable unless overridden by [PolyfillTarget(typeof(...))].
        var targetTypeFqn = "global::System.Linq.Queryable";
        var polyfillAttr = method.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "PolyfillTargetAttribute");
        if (polyfillAttr?.ConstructorArguments.Length > 0 &&
            polyfillAttr.ConstructorArguments[0].Value is INamedTypeSymbol targetType)
        {
            targetTypeFqn = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        return EmitGenericLambda(inv, model, spc, interceptAttr, line, col, fileTag,
            methodName, elementSymbol, elementFqn, method, funcParamIndices, targetTypeFqn);
    }

    private static Emitter.EmitResult? EmitLambdaBody(
        LambdaExpressionSyntax lambda,
        INamedTypeSymbol elementSymbol,
        SemanticModel model,
        SourceProductionContext spc,
        string delegateTypeFqn,
        string assignToVariable = "__lambda",
        string varPrefix = "",
        IReadOnlyDictionary<ITypeSymbol, string>? typeAliases = null,
        string? delegateVarName = null)
    {
        var bodyNode = lambda.Body is ExpressionSyntax expr ? (SyntaxNode)expr : lambda.Body;
        if (bodyNode is null) return null;

        // If binding failed (e.g. a synthesized member invisible to this generator), the IOperation
        // tree has IInvalidOperation nodes and emission would produce garbage; surface a diagnostic
        // instead. Null IOperation is fine — that's transparent syntax the emitter unwraps separately.
        var bodyOperation = model.GetOperation(bodyNode);
        if (bodyOperation is not null && ContainsInvalidOperation(bodyOperation))
        {
            spc.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.InterceptorEmissionFailed,
                lambda.GetLocation(),
                "lambda body contains unresolvable references — the original delegate stub will be used at runtime"));
            return null;
        }

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

        // Return type from delegate args, resolved through aliases for anonymous types.
        var returnTypeFqn = "object";
        if (elementSymbol.ContainingNamespace is not null)
        {
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

        // Prepend the original lambda source as a comment in the generated output.
        var sourceText = lambda.NormalizeWhitespace().ToFullString()
            .Replace("\r", "").Replace("\n", " ");
        var commentLine = $"            // Source: {sourceText}\n";
        return new Emitter.EmitResult(commentLine + result.Body);
    }

    private static bool IsAnonymousType(ITypeSymbol type)
        => type is INamedTypeSymbol { IsAnonymousType: true };

    private static bool ContainsInvalidOperation(IOperation op)
    {
        if (op is IInvalidOperation) return true;
        foreach (var child in op.ChildOperations)
        {
            if (ContainsInvalidOperation(child)) return true;
        }
        return false;
    }

    private static bool HasLambdaArgument(InvocationExpressionSyntax inv)
    {
        var args = inv.ArgumentList.Arguments;
        for (var i = 0; i < args.Count; i++)
        {
            if (args[i].Expression is LambdaExpressionSyntax)
                return true;
        }
        return false;
    }

    private static string MethodId(string op, string fileTag, int line, int col)
        => $"__Polyfill_{op}_{fileTag}_{line}_{col}";

    private static string? EmitGenericLambda(
        InvocationExpressionSyntax inv, SemanticModel model, SourceProductionContext spc,
        string interceptAttr, int line, int col, string fileTag,
        string methodName, INamedTypeSymbol elemSym, string elemFqn,
        IMethodSymbol method, List<int> funcParamIndices, string targetTypeFqn)
    {
        var lambdas = new List<LambdaExpressionSyntax>(funcParamIndices.Count);
        for (int i = 0; i < funcParamIndices.Count; i++)
        {
            if (inv.ArgumentList.Arguments[funcParamIndices[i]].Expression is not LambdaExpressionSyntax lam)
                return null;
            lambdas.Add(lam);
        }

        bool single = funcParamIndices.Count == 1;

        var hasAnyAnon = elemSym.IsAnonymousType;
        for (int i = 0; i < funcParamIndices.Count; i++)
        {
            var fta = ((INamedTypeSymbol)method.Parameters[funcParamIndices[i]].Type).TypeArguments;
            for (int j = 0; j < fta.Length; j++)
                hasAnyAnon = hasAnyAnon || IsAnonymousType(fta[j]);
        }

        // Non-Func params can also be anonymous (e.g. AggregateBy seed).
        for (int i = 0; i < method.Parameters.Length; i++)
        {
            if (!funcParamIndices.Contains(i))
                hasAnyAnon = hasAnyAnon || IsAnonymousType(method.Parameters[i].Type);
        }

        var isRewritableReturn = method.ReturnType is INamedTypeSymbol rqType
            && rqType.ConstructedFrom.ToDisplayString() == IExpressiveQueryableOpenTypeName;

        ITypeSymbol? returnElemType = null;
        if (isRewritableReturn)
        {
            returnElemType = ((INamedTypeSymbol)method.ReturnType).TypeArguments[0];
            hasAnyAnon = hasAnyAnon || IsAnonymousType(returnElemType);
        }

        var scalarReturnFqn = method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        // ThenBy/ThenByDescending require IOrderedQueryable<T>.
        var isOrdered = methodName is "ThenBy" or "ThenByDescending";

        var methodTypeArgs = method.TypeArguments;
        var typeAliases = new Dictionary<ITypeSymbol, string>(SymbolEqualityComparer.Default);
        var delegateFqns = new string[funcParamIndices.Count];
        string elemRef;
        string castFqn;
        string typeParams;
        string returnRef;
        string interceptorParamList;
        string queryableArgList;

        if (hasAnyAnon)
        {
            // All method type args become generic params (T0, T1, …) so the C# compiler can infer
            // anonymous types — they have no nameable form in the interceptor signature.
            var typeParamNames = new string[methodTypeArgs.Length];
            for (int i = 0; i < methodTypeArgs.Length; i++)
                typeParamNames[i] = $"T{i}";
            typeParams = $"<{string.Join(", ", typeParamNames)}>";

            if (!typeAliases.ContainsKey(elemSym))
                typeAliases[elemSym] = typeParamNames[0];
            for (int i = 0; i < methodTypeArgs.Length; i++)
            {
                if (!typeAliases.ContainsKey(methodTypeArgs[i]))
                    typeAliases[methodTypeArgs[i]] = typeParamNames[i];
            }

            if (typeAliases.TryGetValue(elemSym, out var ep))
                elemRef = ep;
            else
                elemRef = elemFqn;

            var funcFqnGenerics = new string[funcParamIndices.Count];
            for (int fi = 0; fi < funcParamIndices.Count; fi++)
            {
                var funcTypeArgs = ((INamedTypeSymbol)method.Parameters[funcParamIndices[fi]].Type).TypeArguments;
                var sigParts = new string[funcTypeArgs.Length];
                for (int i = 0; i < funcTypeArgs.Length; i++)
                {
                    if (typeAliases.TryGetValue(funcTypeArgs[i], out var gp))
                        sigParts[i] = gp;
                    else
                        sigParts[i] = funcTypeArgs[i].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                }
                funcFqnGenerics[fi] = "global::System.Func<" + string.Join(", ", sigParts) + ">";
                delegateFqns[fi] = funcFqnGenerics[fi];
            }

            castFqn = isOrdered
                ? $"global::System.Linq.IOrderedQueryable<{elemRef}>"
                : $"global::System.Linq.IQueryable<{elemRef}>";

            if (isRewritableReturn)
            {
                if (typeAliases.TryGetValue(returnElemType!, out var retParam))
                    returnRef = retParam;
                else
                    // Composite return types like IGrouping<TKey, AnonType> need alias substitution
                    // (anonymous types have no nameable form in C# source).
                    returnRef = ResolveTypeFqn(returnElemType!, typeAliases);
            }
            else
            {
                returnRef = scalarReturnFqn;
            }

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

        // For multi-Func methods (e.g. AggregateBy seedSelector Func<TKey, TAcc>), the source
        // element is NOT necessarily the first type arg of every Func<>; derive per-lambda.
        var emitBodies = new List<string>(funcParamIndices.Count);
        for (int j = 0; j < funcParamIndices.Count; j++)
        {
            var lambdaVar = single ? "__lambda" : $"__lambda{j + 1}";
            var prefix = single ? $"i{fileTag}{line}c{col}_" : $"i{fileTag}{line}c{col}{(char)('a' + j)}_";
            var delegateName = single ? "__func" : $"__func{j + 1}";
            var funcType = (INamedTypeSymbol)method.Parameters[funcParamIndices[j]].Type;
            var lambdaElemSym = funcType.TypeArguments[0] as INamedTypeSymbol ?? elemSym;
            var emitResult = EmitLambdaBody(lambdas[j], lambdaElemSym, model, spc,
                delegateFqns[j], lambdaVar, varPrefix: prefix,
                typeAliases: hasAnyAnon ? typeAliases : null,
                delegateVarName: delegateName);
            if (emitResult is null) return null;
            emitBodies.Add(emitResult.Body);
        }
        var allBodies = string.Concat(emitBodies);

        if (!hasAnyAnon)
        {
            if (isRewritableReturn)
            {
                return $$"""
                        {{interceptAttr}}
                        internal static global::ExpressiveSharp.IExpressiveQueryable<{{returnRef}}> {{MethodId(methodName, fileTag, line, col)}}(
                            this global::ExpressiveSharp.IExpressiveQueryable<{{elemFqn}}> source,
                            {{interceptorParamList}})
                        {
                {{allBodies}}            return global::ExpressiveSharp.ExpressiveQueryableExtensions.AsExpressive(
                                {{targetTypeFqn}}.{{methodName}}(
                                    ({{castFqn}})source,
                                    {{queryableArgList}}));
                        }

                """;
            }

            return $$"""
                    {{interceptAttr}}
                    internal static {{returnRef}} {{MethodId(methodName, fileTag, line, col)}}(
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
                    internal static global::ExpressiveSharp.IExpressiveQueryable<{{returnRef}}> {{MethodId(methodName, fileTag, line, col)}}{{typeParams}}(
                        this global::ExpressiveSharp.IExpressiveQueryable<{{elemRef}}> source,
                        {{interceptorParamList}})
                    {
            {{allBodies}}            return (global::ExpressiveSharp.IExpressiveQueryable<{{returnRef}}>)(object)
                            global::ExpressiveSharp.ExpressiveQueryableExtensions.AsExpressive(
                                {{targetTypeFqn}}.{{methodName}}(
                                    ({{castFqn}})(object)source,
                                    {{queryableArgList}}));
                    }

            """;
        }

        return $$"""
                {{interceptAttr}}
                internal static {{returnRef}} {{MethodId(methodName, fileTag, line, col)}}{{typeParams}}(
                    this global::ExpressiveSharp.IExpressiveQueryable<{{elemRef}}> source,
                    {{interceptorParamList}})
                {
        {{allBodies}}            return {{targetTypeFqn}}.{{methodName}}(
                            ({{castFqn}})(object)source,
                            {{queryableArgList}});
                }

        """;
    }

    private static bool IsExpressiveQueryable(INamedTypeSymbol type)
    {
        if (type.ConstructedFrom.ToDisplayString() == IExpressiveQueryableOpenTypeName)
            return true;

        return type.AllInterfaces.Any(i =>
            i.ConstructedFrom.ToDisplayString() == IExpressiveQueryableOpenTypeName);
    }

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
        if (typeAliases.TryGetValue(type, out var alias))
            return alias;

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
                var idx = openType.LastIndexOf('<');
                if (idx >= 0)
                    openType = openType.Substring(0, idx);
                return openType + "<" + string.Join(", ", resolvedArgs) + ">";
            }
        }

        return type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    /// <summary>
    /// Reference equality on the CompilationUnitSyntax root, ignoring Compilation.
    /// Roslyn keeps unchanged files' syntax tree roots as the same object across incremental
    /// runs, so noise-file edits leave untouched (file, compilation) pairs equal and skip
    /// re-emission — O(1) incremental cost.
    /// </summary>
    private sealed class CompilationUnitAndCompilationComparer
        : IEqualityComparer<(CompilationUnitSyntax Left, Compilation Right)>
    {
        public readonly static CompilationUnitAndCompilationComparer Instance
            = new CompilationUnitAndCompilationComparer();

        private CompilationUnitAndCompilationComparer() { }

        public bool Equals(
            (CompilationUnitSyntax Left, Compilation Right) x,
            (CompilationUnitSyntax Left, Compilation Right) y)
            => ReferenceEquals(x.Left, y.Left);

        public int GetHashCode((CompilationUnitSyntax Left, Compilation Right) obj)
            => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj.Left);
    }

    // ── FileAndSynthesizedSourcesComparer ──────────────────────────────────────

    /// <summary>
    /// ANDs <see cref="CompilationUnitAndCompilationComparer"/> with sequence equality on the
    /// synthesized array — editing an [ExpressiveProperty] attribute correctly re-emits all
    /// files since synthesized binding can affect any file's lambdas.
    /// </summary>
    private sealed class FileAndSynthesizedSourcesComparer
        : IEqualityComparer<((CompilationUnitSyntax Left, Compilation Right) Left, ImmutableArray<(string HintName, string Source)> Right)>
    {
        public readonly static FileAndSynthesizedSourcesComparer Instance = new();

        private FileAndSynthesizedSourcesComparer() { }

        public bool Equals(
            ((CompilationUnitSyntax Left, Compilation Right) Left, ImmutableArray<(string HintName, string Source)> Right) x,
            ((CompilationUnitSyntax Left, Compilation Right) Left, ImmutableArray<(string HintName, string Source)> Right) y)
            => CompilationUnitAndCompilationComparer.Instance.Equals(x.Left, y.Left)
                && SynthesizedSourceArrayComparer.Instance.Equals(x.Right, y.Right);

        public int GetHashCode(
            ((CompilationUnitSyntax Left, Compilation Right) Left, ImmutableArray<(string HintName, string Source)> Right) obj)
        {
            unchecked
            {
                return CompilationUnitAndCompilationComparer.Instance.GetHashCode(obj.Left) * 31
                    + SynthesizedSourceArrayComparer.Instance.GetHashCode(obj.Right);
            }
        }
    }
}
