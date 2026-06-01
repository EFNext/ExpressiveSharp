using ExpressiveSharp.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using ExpressiveSharp.Generator.Comparers;
using ExpressiveSharp.Generator.Emitter;
using ExpressiveSharp.Generator.Infrastructure;
using ExpressiveSharp.Generator.Interpretation;
using ExpressiveSharp.Generator.Models;
using ExpressiveSharp.Generator.Registry;

namespace ExpressiveSharp.Generator;

[Generator]
public class ExpressiveGenerator : IIncrementalGenerator
{
    private const string ExpressiveAttributeName = "ExpressiveSharp.ExpressiveAttribute";
    private const string ExpressiveForAttributeName = "ExpressiveSharp.Mapping.ExpressiveForAttribute";
    private const string ExpressiveForConstructorAttributeName = "ExpressiveSharp.Mapping.ExpressiveForConstructorAttribute";
    private const string ExpressivePropertyAttributeName = "ExpressiveSharp.Mapping.ExpressivePropertyAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var globalOptions = context.AnalyzerConfigOptionsProvider
            .Select(static (opts, _) => new ExpressiveGlobalOptions(opts.GlobalOptions));

        // [ExpressiveProperty] synthesizes new property declarations into separate generated
        // files. Source generators don't see each other's (or their own pipelines') AddSource
        // output, so the SemanticModel built from the input compilation can't bind references
        // from one [ExpressiveProperty] body to another's synthesized target. Mirror the
        // synthesis here and augment our local compilation for binding only — never AddSource.
        var synthesizedSources = BuildSynthesizedSourcesPipeline(context);

        // Augment the compilation once per (compilation, synth) pair. The result is reused
        // across every member of every pipeline, so we don't re-parse synthesized partials
        // and rebuild a Compilation per [Expressive] / [ExpressiveFor] / [ExpressiveProperty]
        // member.
        var bindingCompilationProvider = context.CompilationProvider
            .Combine(synthesizedSources)
            .Select(static (pair, ct) => AugmentCompilation(pair.Left, pair.Right, ct));

        // ── [Expressive] pipeline ──────────────────────────────────────────────

        // Extract only pure stable data from the attribute in the transform.
        // No live Roslyn objects (no AttributeData, SemanticModel, Compilation, ISymbol) —
        // those are always new instances and defeat incremental caching entirely.
        var memberDeclarations = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                ExpressiveAttributeName,
                predicate: static (s, _) => s is MemberDeclarationSyntax,
                transform: static (c, _) => (
                    Member: (MemberDeclarationSyntax)c.TargetNode,
                    Attribute: new ExpressiveAttributeData(c.Attributes[0])
                ));

        var memberDeclarationsWithGlobalOptions = memberDeclarations
            .Combine(globalOptions)
            .Select(static (pair, _) => (
                Member: pair.Left.Member,
                Attribute: pair.Left.Attribute,
                GlobalOptions: pair.Right
            ));

        // Combine with the augmented compilation directly: validation has no synthesized-sibling
        // conflicts in this pipeline (only [ExpressiveProperty] does), so binding-against-augmented
        // is correct everywhere.
        var compilationAndMemberPairs = memberDeclarationsWithGlobalOptions
            .Combine(bindingCompilationProvider)
            .WithComparer(new MemberDeclarationSyntaxAndCompilationEqualityComparer());

        context.RegisterImplementationSourceOutput(compilationAndMemberPairs,
            static (spc, source) =>
            {
                var ((member, attribute, globalOptions), bindingCompilation) = source;
                var semanticModel = bindingCompilation.GetSemanticModel(member.SyntaxTree);
                var memberSymbol = semanticModel.GetDeclaredSymbol(member);

                if (memberSymbol is null)
                {
                    return;
                }

                Execute(member, semanticModel, memberSymbol, attribute, globalOptions, bindingCompilation, spc);
            });

        var registryEntries = compilationAndMemberPairs.Select(
            static (source, cancellationToken) => {
                var ((member, _, _), bindingCompilation) = source;

                var semanticModel = bindingCompilation.GetSemanticModel(member.SyntaxTree);
                var memberSymbol = semanticModel.GetDeclaredSymbol(member, cancellationToken);

                if (memberSymbol is null)
                {
                    return null;
                }

                return ExtractRegistryEntry(memberSymbol);
            });

        var expressiveForDeclarations = CreateExpressiveForPipeline(
            context, globalOptions, bindingCompilationProvider, ExpressiveForAttributeName, ExpressiveForMemberKind.MethodOrProperty);

        var expressiveForConstructorDeclarations = CreateExpressiveForPipeline(
            context, globalOptions, bindingCompilationProvider, ExpressiveForConstructorAttributeName, ExpressiveForMemberKind.Constructor);

        var expressiveForRegistryEntries = expressiveForDeclarations.Select(
            static (source, _) => ExtractRegistryEntryForExternal(source));
        var expressiveForConstructorRegistryEntries = expressiveForConstructorDeclarations.Select(
            static (source, _) => ExtractRegistryEntryForExternal(source));

        // ── [ExpressiveProperty] pipeline ───────────────────────────────────────

        var expressivePropertyDeclarations = CreateExpressivePropertyPipeline(context, bindingCompilationProvider);

        var expressivePropertyRegistryEntries = expressivePropertyDeclarations.Select(
            static (source, _) => ExtractRegistryEntryForExpressiveProperty(source));

        var allRegistryEntries = registryEntries.Collect()
            .Combine(expressiveForRegistryEntries.Collect())
            .Combine(expressiveForConstructorRegistryEntries.Collect())
            .Combine(expressivePropertyRegistryEntries.Collect())
            .Select(static (pair, _) =>
            {
                var (((expressiveEntries, forEntries), forCtorEntries), propEntries) = pair;
                var builder = ImmutableArray.CreateBuilder<ExpressionRegistryEntry?>(
                    expressiveEntries.Length + forEntries.Length + forCtorEntries.Length + propEntries.Length);
                builder.AddRange(expressiveEntries);
                builder.AddRange(forEntries);
                builder.AddRange(forCtorEntries);
                builder.AddRange(propEntries);
                return builder.ToImmutable();
            });

        context.RegisterImplementationSourceOutput(
            allRegistryEntries,
            static (spc, entries) => ExpressionRegistryEmitter.Emit(entries, spc));
    }

    private static IncrementalValuesProvider<((MemberDeclarationSyntax Member, ExpressiveForAttributeData Attribute, ExpressiveGlobalOptions GlobalOptions), Compilation)>
        CreateExpressiveForPipeline(
            IncrementalGeneratorInitializationContext context,
            IncrementalValueProvider<ExpressiveGlobalOptions> globalOptions,
            IncrementalValueProvider<Compilation> bindingCompilationProvider,
            string attributeFullName,
            ExpressiveForMemberKind memberKind)
    {
        var declarations = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                attributeFullName,
                predicate: static (s, _) => s is MethodDeclarationSyntax or PropertyDeclarationSyntax,
                transform: (c, _) => (
                    Member: (MemberDeclarationSyntax)c.TargetNode,
                    Attribute: new ExpressiveForAttributeData(c.Attributes[0], memberKind)
                ));

        var declarationsWithGlobalOptions = declarations
            .Combine(globalOptions)
            .Select(static (pair, _) => (
                Member: pair.Left.Member,
                Attribute: pair.Left.Attribute,
                GlobalOptions: pair.Right
            ));

        // Combine with the augmented compilation directly. The augmented compilation is
        // shared across the entire run via bindingCompilationProvider's Select cache, so
        // we don't re-parse synthesized partials per member.
        var compilationAndPairs = declarationsWithGlobalOptions
            .Combine(bindingCompilationProvider)
            .WithComparer(new ExpressiveForMemberCompilationEqualityComparer());

        // Collect all items and emit in a single batch to detect duplicates before AddSource.
        // Per-item emission would crash the generator on duplicate hint names (Roslyn deduplicates
        // after all per-item callbacks, not at the AddSource call site).
        context.RegisterImplementationSourceOutput(compilationAndPairs.Collect(),
            static (spc, items) =>
            {
                var emittedFileNames = new HashSet<string>();

                foreach (var source in items)
                {
                    var ((member, attribute, globalOptions), bindingCompilation) = source;
                    var semanticModel = bindingCompilation.GetSemanticModel(member.SyntaxTree);
                    var stubSymbol = semanticModel.GetDeclaredSymbol(member);

                    if (stubSymbol is not (IMethodSymbol or IPropertySymbol))
                        continue;

                    ExecuteFor(member, semanticModel, stubSymbol, attribute, globalOptions,
                        bindingCompilation, spc, emittedFileNames);
                }
            });

        return compilationAndPairs;
    }

    private static void Execute(
        MemberDeclarationSyntax member,
        SemanticModel semanticModel,
        ISymbol memberSymbol,
        ExpressiveAttributeData expressiveAttribute,
        ExpressiveGlobalOptions globalOptions,
        Compilation? compilation,
        SourceProductionContext context)
    {
        var expressive = ExpressiveInterpreter.GetDescriptor(
            semanticModel, member, memberSymbol, expressiveAttribute, globalOptions, context, compilation);

        if (expressive is null)
        {
            return;
        }

        if (expressive.MemberName is null)
        {
            throw new InvalidOperationException("Expected a memberName here");
        }

        // EXP0012: factory method that could be a constructor.
        if (member is MethodDeclarationSyntax factoryCandidate && SyntaxHelpers.TryGetFactoryMethodPattern(factoryCandidate, out _))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Infrastructure.Diagnostics.FactoryMethodShouldBeConstructor,
                factoryCandidate.Identifier.GetLocation(),
                factoryCandidate.Identifier.Text));
        }

        // EXP0038: virtual/abstract/override members are expanded using the static (declared)
        // type. Once the body is inlined into an expression tree (EF Core, MongoDB, ...), C#
        // virtual dispatch is lost, so an overridden body in a derived type is never used.
        if (memberSymbol.IsVirtual || memberSymbol.IsAbstract || memberSymbol.IsOverride)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Infrastructure.Diagnostics.VirtualMemberDispatchedStatically,
                memberSymbol.Locations.Length > 0 ? memberSymbol.Locations[0] : Location.None,
                memberSymbol.Name));
        }

        var generatedClassName = ExpressionClassNameGenerator.GenerateClassName(expressive.ClassNamespace, expressive.NestedInClassNames);
        var methodSuffix = ExpressionClassNameGenerator.GenerateMethodSuffix(expressive.MemberName, expressive.ParameterTypeNames);
        var generatedFileName = expressive.ClassTypeParameterList is not null
            ? $"{generatedClassName}-{expressive.ClassTypeParameterList.Parameters.Count}.{methodSuffix}.g.cs"
            : $"{generatedClassName}.{methodSuffix}.g.cs";

        if (expressive.ExpressionTreeEmission is null)
        {
            throw new InvalidOperationException("ExpressionTreeEmission must be set");
        }

        EmitExpressionTreeSource(expressive, generatedClassName, methodSuffix, generatedFileName, member, compilation, context);
    }

    /// <summary>
    /// Each file declares the same <c>static partial class</c> (one per declaring type) and adds
    /// a uniquely-named <c>{methodSuffix}_Expression()</c> method for this member.
    /// </summary>
    private static void EmitExpressionTreeSource(
        ExpressiveDescriptor expressive,
        string generatedClassName,
        string methodSuffix,
        string generatedFileName,
        MemberDeclarationSyntax member,
        Compilation? compilation,
        SourceProductionContext context)
    {
        var emission = expressive.ExpressionTreeEmission!;
        var sb = new System.Text.StringBuilder();

        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable disable");
        sb.AppendLine();

        foreach (var usingDirective in expressive.UsingDirectives!)
        {
            sb.AppendLine(usingDirective.NormalizeWhitespace().ToFullString());
        }

        if (expressive.ClassNamespace is not null)
        {
            sb.AppendLine($"using {expressive.ClassNamespace};");
        }

        sb.AppendLine();
        sb.AppendLine("namespace ExpressiveSharp.Generated");
        sb.AppendLine("{");

        // Build return type string: Expression<Func<ParamTypes..., ReturnType>>
        var paramTypesList = expressive.ParametersList?.Parameters
            .Where(p => p.Type is not null)
            .Select(p => p.Type!.ToString())
            .ToList() ?? new List<string>();
        if (expressive.ReturnTypeName is not null)
        {
            paramTypesList.Add(expressive.ReturnTypeName);
        }

        var funcType = $"global::System.Func<{string.Join(", ", paramTypesList)}>";
        var returnType = $"global::System.Linq.Expressions.Expression<{funcType}>";

        var typeParamList = expressive.ClassTypeParameterList?.NormalizeWhitespace().ToFullString() ?? "";
        var constraintClauses = expressive.ClassConstraintClauses is not null
            ? string.Join(" ", expressive.ClassConstraintClauses.Value.Select(c => c.NormalizeWhitespace().ToFullString()))
            : "";

        var methodTypeParamList = expressive.TypeParameterList?.NormalizeWhitespace().ToFullString() ?? "";
        var methodConstraintClauses = expressive.ConstraintClauses is not null
            ? string.Join(" ", expressive.ConstraintClauses.Value.Select(c => c.NormalizeWhitespace().ToFullString()))
            : "";

        sb.AppendLine($"    static partial class {generatedClassName}{typeParamList} {constraintClauses}");
        sb.AppendLine("    {");

        // Emit the original C# member as a comment in the generated file for readability.
        var sourceText = member.NormalizeWhitespace().ToFullString();
        foreach (var line in sourceText.Split('\n'))
        {
            var trimmed = line.TrimEnd('\r');
            sb.AppendLine($"        // {trimmed}");
        }

        sb.AppendLine($"        static {returnType} {methodSuffix}_Expression{methodTypeParamList}() {methodConstraintClauses}");
        sb.AppendLine("        {");
        sb.Append(emission.Body);
        sb.AppendLine("        }");

        if (expressive.DeclaredTransformerTypeNames.Count > 0)
        {
            sb.AppendLine();
            var transformerInstances = string.Join(", ",
                expressive.DeclaredTransformerTypeNames.Select(t => $"new {t}()"));
            sb.AppendLine($"        static global::ExpressiveSharp.IExpressionTreeTransformer[] {methodSuffix}_Transformers() => [{transformerInstances}];");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        context.AddSource(generatedFileName, SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private static ExpressionRegistryEntry? ExtractRegistryEntry(ISymbol memberSymbol)
    {
        var containingType = memberSymbol.ContainingType;

        // Metadata-only entries are excluded from the runtime registry but still emit
        // [EditorBrowsable] attribute-only partial files.
        var isMetadataOnly = false;
        string? classTypeParameters = null;

        // C# 14 extension type members — fall back to reflection at runtime.
        if (containingType is { IsExtension: true })
        {
            isMetadataOnly = true;
        }

        // Generic classes — registry can't represent open generic types.
        if (containingType.TypeParameters.Length > 0)
        {
            isMetadataOnly = true;
            classTypeParameters = "<" + string.Join(", ", containingType.TypeParameters.Select(tp => tp.Name)) + ">";
        }

        ExpressionRegistryMemberType memberKind;
        string memberLookupName;
        var parameterTypeNames = ImmutableArray<string>.Empty;

        if (memberSymbol is IMethodSymbol methodSymbol)
        {
            // Generic methods — same reason as generic classes.
            if (methodSymbol.TypeParameters.Length > 0)
            {
                isMetadataOnly = true;
            }

            if (methodSymbol.MethodKind is MethodKind.Constructor or MethodKind.StaticConstructor)
            {
                memberKind = ExpressionRegistryMemberType.Constructor;
                memberLookupName = "_ctor";
            }
            else
            {
                memberKind = ExpressionRegistryMemberType.Method;
                memberLookupName = memberSymbol.Name;
            }

            parameterTypeNames = [
                ..methodSymbol.Parameters.Select(p => p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            ];
        }
        else
        {
            memberKind = ExpressionRegistryMemberType.Property;
            memberLookupName = memberSymbol.Name;
        }

        var classNamespace = containingType.ContainingNamespace.IsGlobalNamespace
            ? null
            : containingType.ContainingNamespace.ToDisplayString();

        var nestedTypePath = GetRegistryNestedTypePath(containingType);

        var generatedClassFullName = ExpressionClassNameGenerator.GenerateClassFullName(
            classNamespace,
            nestedTypePath);

        var methodSuffix = ExpressionClassNameGenerator.GenerateMethodSuffix(
            memberLookupName,
            parameterTypeNames.IsEmpty ? null : parameterTypeNames);

        var expressionMethodName = methodSuffix + "_Expression";

        var declaringTypeFullName = containingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        return new ExpressionRegistryEntry(
            DeclaringTypeFullName: declaringTypeFullName,
            MemberKind: memberKind,
            MemberLookupName: memberLookupName,
            GeneratedClassFullName: generatedClassFullName,
            ExpressionMethodName: expressionMethodName,
            ParameterTypeNames: parameterTypeNames,
            IsMetadataOnly: isMetadataOnly,
            ClassTypeParameters: classTypeParameters);
    }

    private static void ExecuteFor(
        MemberDeclarationSyntax stubMember,
        SemanticModel semanticModel,
        ISymbol stubSymbol,
        ExpressiveForAttributeData attributeData,
        ExpressiveGlobalOptions globalOptions,
        Compilation compilation,
        SourceProductionContext context,
        HashSet<string>? emittedFileNames = null)
    {
        var descriptor = ExpressiveForInterpreter.GetDescriptor(
            semanticModel, stubMember, stubSymbol, attributeData, globalOptions, context, compilation);

        if (descriptor is null)
            return;

        if (descriptor.MemberName is null)
            throw new InvalidOperationException("Expected a memberName here");

        var generatedClassName = ExpressionClassNameGenerator.GenerateClassName(
            descriptor.ClassNamespace, descriptor.NestedInClassNames);
        var methodSuffix = ExpressionClassNameGenerator.GenerateMethodSuffix(
            descriptor.MemberName, descriptor.ParameterTypeNames);
        var generatedFileName = $"{generatedClassName}.{methodSuffix}.g.cs";

        // Skip duplicate emissions — EXP0020 is reported via the registry duplicate check
        if (emittedFileNames is not null && !emittedFileNames.Add(generatedFileName))
            return;

        if (descriptor.ExpressionTreeEmission is null)
            throw new InvalidOperationException("ExpressionTreeEmission must be set");

        EmitExpressionTreeSource(descriptor, generatedClassName, methodSuffix, generatedFileName, stubMember, compilation, context);
    }

    /// <summary>
    /// The entry points to the external target member, not the stub itself.
    /// </summary>
    private static ExpressionRegistryEntry? ExtractRegistryEntryForExternal(
        ((MemberDeclarationSyntax Member, ExpressiveForAttributeData Attribute, ExpressiveGlobalOptions GlobalOptions), Compilation) source)
    {
        var ((member, attribute, _), compilation) = source;
        var semanticModel = compilation.GetSemanticModel(member.SyntaxTree);
        var rawStubSymbol = semanticModel.GetDeclaredSymbol(member);

        if (rawStubSymbol is not (IMethodSymbol or IPropertySymbol))
            return null;

        var stubIsProperty = rawStubSymbol is IPropertySymbol;
        var stubIsStatic = rawStubSymbol.IsStatic;
        var stubContainingType = rawStubSymbol.ContainingType;
        var stubMethodSymbol = rawStubSymbol as IMethodSymbol;

        // Property stubs cannot map to constructors (no parameter list).
        if (stubIsProperty && attribute.MemberKind == ExpressiveForMemberKind.Constructor)
            return null;

        // Resolve target type. Two cases (mirrors ExpressiveForInterpreter):
        //  - Two-arg form: resolve from metadata name.
        //  - Single-arg form: default to the stub's containing type.
        var targetType = attribute.TargetTypeMetadataName is not null
            ? compilation.GetTypeByMetadataName(attribute.TargetTypeMetadataName)
            : stubContainingType;

        if (targetType is null)
            return null;

        // Skip generic target types (registry only supports closed constructed types)
        if (targetType.TypeParameters.Length > 0)
            return null;

        var targetTypeFullName = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        ExpressionRegistryMemberType memberKind;
        string memberLookupName;
        var parameterTypeNames = ImmutableArray<string>.Empty;

        if (attribute.MemberKind == ExpressiveForMemberKind.Constructor)
        {
            memberKind = ExpressionRegistryMemberType.Constructor;
            memberLookupName = "_ctor";

            // Constructor params match stub params directly (method stubs only — guarded above).
            parameterTypeNames = [
                ..stubMethodSymbol!.Parameters.Select(p => p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            ];
        }
        else
        {
            var memberName = attribute.MemberName;
            if (memberName is null)
                return null;

            // Property stubs can only target properties; method stubs may target either.
            var isProperty = stubIsProperty
                || targetType.GetMembers(memberName).OfType<IPropertySymbol>().Any();

            if (isProperty)
            {
                memberKind = ExpressionRegistryMemberType.Property;
                memberLookupName = memberName;
                // Properties have no parameter types in the registry
            }
            else
            {
                memberKind = ExpressionRegistryMemberType.Method;
                memberLookupName = memberName;

                // Use the shared matcher so the registry entry can never disagree with
                // what ExpressiveForInterpreter accepted.
                var stubParams = stubMethodSymbol!.Parameters;
                var targetMethod = targetType.GetMembers(memberName).OfType<IMethodSymbol>()
                    .Where(m => m.MethodKind is not (MethodKind.PropertyGet or MethodKind.PropertySet))
                    .FirstOrDefault(m => Interpretation.ExpressiveForSignatureMatcher
                        .MatchesMethodSignature(m, targetType, stubIsStatic, stubContainingType, stubParams));

                if (targetMethod is null)
                    return null;

                // Use the TARGET method's parameter types (not the stub's)
                parameterTypeNames = [
                    ..targetMethod.Parameters.Select(p => p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                ];
            }
        }

        var classNamespace = targetType.ContainingNamespace.IsGlobalNamespace
            ? null
            : targetType.ContainingNamespace.ToDisplayString();

        var nestedTypePath = GetRegistryNestedTypePath(targetType);

        var generatedClassFullName = ExpressionClassNameGenerator.GenerateClassFullName(
            classNamespace, nestedTypePath);

        var methodSuffix = ExpressionClassNameGenerator.GenerateMethodSuffix(
            memberLookupName,
            parameterTypeNames.IsEmpty ? null : parameterTypeNames);

        var expressionMethodName = methodSuffix + "_Expression";

        var stubLocation = member switch
        {
            MethodDeclarationSyntax m => m.Identifier.GetLocation(),
            PropertyDeclarationSyntax p => p.Identifier.GetLocation(),
            _ => member.GetLocation()
        };
        var stubLineSpan = stubLocation.GetLineSpan();

        return new ExpressionRegistryEntry(
            DeclaringTypeFullName: targetTypeFullName,
            MemberKind: memberKind,
            MemberLookupName: memberLookupName,
            GeneratedClassFullName: generatedClassFullName,
            ExpressionMethodName: expressionMethodName,
            ParameterTypeNames: parameterTypeNames,
            StubLocation: new SourceLocation(stubLineSpan.Path, stubLocation.SourceSpan, stubLineSpan.Span));
    }

    private static IEnumerable<string> GetRegistryNestedTypePath(INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol.ContainingType is not null)
        {
            foreach (var name in GetRegistryNestedTypePath(typeSymbol.ContainingType))
            {
                yield return name;
            }
        }
        yield return typeSymbol.Name;
    }

    private static IncrementalValueProvider<ImmutableArray<(string HintName, string Source)>>
        BuildSynthesizedSourcesPipeline(IncrementalGeneratorInitializationContext context)
    {
        var augmentations = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                ExpressivePropertyAttributeName,
                predicate: static (s, _) => s is PropertyDeclarationSyntax,
                transform: static (c, _) => (
                    Stub: (PropertyDeclarationSyntax)c.TargetNode,
                    Attribute: new ExpressivePropertyAttributeData(c.Attributes[0])))
            .Combine(context.CompilationProvider)
            .Select(static (pair, ct) =>
            {
                ct.ThrowIfCancellationRequested();
                var ((stub, attribute), compilation) = pair;
                var sm = compilation.GetSemanticModel(stub.SyntaxTree);
                if (sm.GetDeclaredSymbol(stub, ct) is not IPropertySymbol stubSymbol)
                    return default((string HintName, string Source, string DedupKey));
                var spec = ExpressivePropertyInterpreter.TryBuildSpec(stub, stubSymbol, attribute);
                if (spec is null) return default;
                var hint = $"{spec.ContainingTypeName}.{spec.PropertyName}.Synthesized.augment.cs";
                var dedupKey = BuildDedupKey(spec);
                return (HintName: hint, Source: SynthesizedPropertyEmitter.BuildSource(spec), DedupKey: dedupKey);
            })
            .Where(static t => t.Source is not null);

        return augmentations
            .Collect()
            .Select(static (arr, _) =>
            {
                if (arr.Length == 0)
                    return ImmutableArray<(string HintName, string Source)>.Empty;
                // Two stubs targeting the same (containing type, property name) is an EXP-error
                // case but both pass TryBuildSpec. Dedup so the augmented compilation doesn't
                // get duplicate-member binding errors that would mask the real diagnostic.
                var seen = new HashSet<string>();
                var builder = ImmutableArray.CreateBuilder<(string HintName, string Source)>(arr.Length);
                foreach (var item in arr.Where(item => seen.Add(item.DedupKey)))
                    builder.Add((item.HintName, item.Source));
                return builder.Count == arr.Length ? builder.MoveToImmutable() : builder.ToImmutable();
            })
            .WithComparer(SynthesizedSourceArrayComparer.Instance);
    }

    private static string BuildDedupKey(SynthesizedPropertySpec spec)
    {
        var sb = new StringBuilder();
        if (spec.ContainingTypeNamespace is not null)
        {
            sb.Append(spec.ContainingTypeNamespace);
            sb.Append('.');
        }
        for (var i = 0; i < spec.ContainingTypePath.Count; i++)
        {
            if (i > 0) sb.Append('+');
            sb.Append(spec.ContainingTypePath[i]);
        }
        sb.Append('.');
        sb.Append(spec.PropertyName);
        return sb.ToString();
    }

    private static Compilation AugmentCompilation(
        Compilation compilation,
        ImmutableArray<(string HintName, string Source)> synthesizedSources,
        CancellationToken ct)
    {
        if (synthesizedSources.IsDefaultOrEmpty)
            return compilation;
        var parseOptions = compilation.SyntaxTrees.FirstOrDefault()?.Options as CSharpParseOptions
            ?? CSharpParseOptions.Default;
        var trees = new SyntaxTree[synthesizedSources.Length];
        for (var i = 0; i < trees.Length; i++)
        {
            trees[i] = CSharpSyntaxTree.ParseText(
                synthesizedSources[i].Source,
                parseOptions,
                cancellationToken: ct);
        }
        return compilation.AddSyntaxTrees(trees);
    }

    /// <summary>
    /// Incremental pipeline for <c>[ExpressiveProperty]</c>. Discovers property stubs, runs the
    /// interpreter, and emits both the expression-tree factory and the synthesized partial-class
    /// declaration.
    /// </summary>
    private static IncrementalValuesProvider<((PropertyDeclarationSyntax Stub, ExpressivePropertyAttributeData Attribute), Compilation)>
        CreateExpressivePropertyPipeline(
            IncrementalGeneratorInitializationContext context,
            IncrementalValueProvider<Compilation> bindingCompilationProvider)
    {
        var declarations = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                ExpressivePropertyAttributeName,
                predicate: static (s, _) => s is PropertyDeclarationSyntax,
                transform: static (c, _) => (
                    Stub: (PropertyDeclarationSyntax)c.TargetNode,
                    Attribute: new ExpressivePropertyAttributeData(c.Attributes[0])
                ));

        var compilationAndPairs = declarations.Combine(context.CompilationProvider);

        // Pair every (decl, originalCompilation) with the augmented compilation. Validation
        // and ChooseBackingNames must use the original compilation (augmentation would otherwise
        // false-positive EXP0031 against synthesized siblings); body-binding uses the augmented
        // SemanticModel so cross-references between [ExpressiveProperty] stubs resolve.
        var compilationAndPairsWithBinding = compilationAndPairs.Combine(bindingCompilationProvider);

        context.RegisterSourceOutput(compilationAndPairsWithBinding.Collect(),
            static (spc, items) =>
            {
                var emittedFileNames = new HashSet<string>();
                foreach (var source in items)
                {
                    var (((stub, attribute), compilation), bindingCompilation) = source;
                    var semanticModel = compilation.GetSemanticModel(stub.SyntaxTree);
                    if (semanticModel.GetDeclaredSymbol(stub) is not IPropertySymbol stubSymbol)
                        continue;

                    var bindingSemanticModel = ReferenceEquals(bindingCompilation, compilation)
                        ? semanticModel
                        : bindingCompilation.GetSemanticModel(stub.SyntaxTree);

                    ExecuteExpressiveProperty(stub, stubSymbol, semanticModel, bindingSemanticModel,
                        attribute, spc, emittedFileNames);
                }
            });

        return compilationAndPairs;
    }

    private static void ExecuteExpressiveProperty(
        PropertyDeclarationSyntax stub,
        IPropertySymbol stubSymbol,
        SemanticModel semanticModel,
        SemanticModel bodyBindingSemanticModel,
        ExpressivePropertyAttributeData attribute,
        SourceProductionContext context,
        HashSet<string> emittedFileNames)
    {
        var result = ExpressivePropertyInterpreter.GetDescriptor(
            semanticModel, stub, stubSymbol, attribute, context,
            bodyBindingSemanticModel: bodyBindingSemanticModel);
        if (result is null) return;

        var (descriptor, synthesisSpec) = result.Value;

        if (descriptor.MemberName is null)
            throw new InvalidOperationException("Expected a memberName here");
        if (descriptor.ExpressionTreeEmission is null)
            throw new InvalidOperationException("ExpressionTreeEmission must be set");

        var generatedClassName = ExpressionClassNameGenerator.GenerateClassName(
            descriptor.ClassNamespace, descriptor.NestedInClassNames);
        var methodSuffix = ExpressionClassNameGenerator.GenerateMethodSuffix(
            descriptor.MemberName, descriptor.ParameterTypeNames);
        var generatedFileName = $"{generatedClassName}.{methodSuffix}.g.cs";

        if (!emittedFileNames.Add(generatedFileName))
            return;

        EmitExpressionTreeSource(descriptor, generatedClassName, methodSuffix, generatedFileName,
            stub, compilation: null, context);

        var synthesizedFileName = $"{generatedClassName}.{methodSuffix}.Synthesized.g.cs";
        Emitter.SynthesizedPropertyEmitter.Emit(synthesisSpec, synthesizedFileName, context);
    }

    /// <summary>
    /// Keyed on the synthesized target name, which doesn't exist on the target type yet —
    /// the synthesized partial declaration fills it in.
    /// </summary>
    private static ExpressionRegistryEntry? ExtractRegistryEntryForExpressiveProperty(
        ((PropertyDeclarationSyntax Stub, ExpressivePropertyAttributeData Attribute), Compilation) source)
    {
        var ((stub, attribute), compilation) = source;
        if (attribute.TargetName is null || string.IsNullOrWhiteSpace(attribute.TargetName))
            return null;

        var semanticModel = compilation.GetSemanticModel(stub.SyntaxTree);
        if (semanticModel.GetDeclaredSymbol(stub) is not IPropertySymbol stubSymbol)
            return null;

        var containingType = stubSymbol.ContainingType;
        if (containingType.TypeParameters.Length > 0)
            return null;

        var classNamespace = containingType.ContainingNamespace.IsGlobalNamespace
            ? null
            : containingType.ContainingNamespace.ToDisplayString();
        var nestedTypePath = GetRegistryNestedTypePath(containingType);

        var generatedClassFullName = ExpressionClassNameGenerator.GenerateClassFullName(
            classNamespace, nestedTypePath);
        var methodSuffix = ExpressionClassNameGenerator.GenerateMethodSuffix(
            attribute.TargetName, parameterTypeNames: null);
        var expressionMethodName = methodSuffix + "_Expression";

        var stubLocation = stub.Identifier.GetLocation();
        var stubLineSpan = stubLocation.GetLineSpan();

        return new ExpressionRegistryEntry(
            DeclaringTypeFullName: containingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            MemberKind: ExpressionRegistryMemberType.Property,
            MemberLookupName: attribute.TargetName,
            GeneratedClassFullName: generatedClassFullName,
            ExpressionMethodName: expressionMethodName,
            ParameterTypeNames: ImmutableArray<string>.Empty,
            StubLocation: new SourceLocation(stubLineSpan.Path, stubLocation.SourceSpan, stubLineSpan.Span));
    }
}
