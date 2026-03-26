using ExpressiveSharp.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;
using ExpressiveSharp.Generator.Infrastructure;
using ExpressiveSharp.Generator.Comparers;
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

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Snapshot global MSBuild defaults once per generator run.
        var globalOptions = context.AnalyzerConfigOptionsProvider
            .Select(static (opts, _) => new ExpressiveGlobalOptions(opts.GlobalOptions));

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

        // Flatten (Member, Attribute) + GlobalOptions into a single named tuple.
        var memberDeclarationsWithGlobalOptions = memberDeclarations
            .Combine(globalOptions)
            .Select(static (pair, _) => (
                Member: pair.Left.Member,
                Attribute: pair.Left.Attribute,
                GlobalOptions: pair.Right
            ));

        var compilationAndMemberPairs = memberDeclarationsWithGlobalOptions
            .Combine(context.CompilationProvider)
            .WithComparer(new MemberDeclarationSyntaxAndCompilationEqualityComparer());

        context.RegisterSourceOutput(compilationAndMemberPairs,
            static (spc, source) =>
            {
                var ((member, attribute, globalOptions), compilation) = source;
                var semanticModel = compilation.GetSemanticModel(member.SyntaxTree);
                var memberSymbol = semanticModel.GetDeclaredSymbol(member);

                if (memberSymbol is null)
                {
                    return;
                }

                Execute(member, semanticModel, memberSymbol, attribute, globalOptions, compilation, spc);
            });

        // Build the expression registry: collect all entries and emit a single registry file
        var registryEntries = compilationAndMemberPairs.Select(
            static (source, cancellationToken) => {
                var ((member, _, _), compilation) = source;

                var semanticModel = compilation.GetSemanticModel(member.SyntaxTree);
                var memberSymbol = semanticModel.GetDeclaredSymbol(member, cancellationToken);

                if (memberSymbol is null)
                {
                    return null;
                }

                return ExtractRegistryEntry(memberSymbol);
            });

        // ── [ExpressiveFor] / [ExpressiveForConstructor] pipelines ──────────────

        var expressiveForDeclarations = CreateExpressiveForPipeline(
            context, globalOptions, ExpressiveForAttributeName, ExpressiveForMemberKind.MethodOrProperty);

        var expressiveForConstructorDeclarations = CreateExpressiveForPipeline(
            context, globalOptions, ExpressiveForConstructorAttributeName, ExpressiveForMemberKind.Constructor);

        // Collect registry entries from [ExpressiveFor] pipelines
        var expressiveForRegistryEntries = expressiveForDeclarations.Select(
            static (source, _) => ExtractRegistryEntryForExternal(source));
        var expressiveForConstructorRegistryEntries = expressiveForConstructorDeclarations.Select(
            static (source, _) => ExtractRegistryEntryForExternal(source));

        // ── Merged registry ─────────────────────────────────────────────────────

        var allRegistryEntries = registryEntries.Collect()
            .Combine(expressiveForRegistryEntries.Collect())
            .Combine(expressiveForConstructorRegistryEntries.Collect())
            .Select(static (pair, _) =>
            {
                var ((expressiveEntries, forEntries), forCtorEntries) = pair;
                var builder = ImmutableArray.CreateBuilder<ExpressionRegistryEntry?>(
                    expressiveEntries.Length + forEntries.Length + forCtorEntries.Length);
                builder.AddRange(expressiveEntries);
                builder.AddRange(forEntries);
                builder.AddRange(forCtorEntries);
                return builder.ToImmutable();
            });

        // Delegate registry file emission to the dedicated ExpressionRegistryEmitter,
        // which uses a string-based CodeWriter instead of SyntaxFactory.
        context.RegisterImplementationSourceOutput(
            allRegistryEntries,
            static (spc, entries) => ExpressionRegistryEmitter.Emit(entries, spc));
    }

    /// <summary>
    /// Creates the incremental pipeline for an [ExpressiveFor*] attribute type.
    /// Discovers, interprets, emits expression factory source, and returns the pipeline
    /// for registry entry extraction.
    /// </summary>
    private static IncrementalValuesProvider<((MethodDeclarationSyntax Method, ExpressiveForAttributeData Attribute, ExpressiveGlobalOptions GlobalOptions), Compilation)>
        CreateExpressiveForPipeline(
            IncrementalGeneratorInitializationContext context,
            IncrementalValueProvider<ExpressiveGlobalOptions> globalOptions,
            string attributeFullName,
            ExpressiveForMemberKind memberKind)
    {
        var declarations = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                attributeFullName,
                predicate: static (s, _) => s is MethodDeclarationSyntax,
                transform: (c, _) => (
                    Method: (MethodDeclarationSyntax)c.TargetNode,
                    Attribute: new ExpressiveForAttributeData(c.Attributes[0], memberKind)
                ));

        var declarationsWithGlobalOptions = declarations
            .Combine(globalOptions)
            .Select(static (pair, _) => (
                Method: pair.Left.Method,
                Attribute: pair.Left.Attribute,
                GlobalOptions: pair.Right
            ));

        var compilationAndPairs = declarationsWithGlobalOptions
            .Combine(context.CompilationProvider)
            .WithComparer(new ExpressiveForMemberCompilationEqualityComparer());

        context.RegisterSourceOutput(compilationAndPairs,
            static (spc, source) =>
            {
                var ((method, attribute, globalOptions), compilation) = source;
                var semanticModel = compilation.GetSemanticModel(method.SyntaxTree);
                var stubSymbol = semanticModel.GetDeclaredSymbol(method) as IMethodSymbol;

                if (stubSymbol is null)
                {
                    return;
                }

                ExecuteFor(method, semanticModel, stubSymbol, attribute, globalOptions, compilation, spc);
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

        // Report EXP0012 when an [Expressive] method is a factory that could be a constructor.
        if (member is MethodDeclarationSyntax factoryCandidate && SyntaxHelpers.TryGetFactoryMethodPattern(factoryCandidate, out _))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Infrastructure.Diagnostics.FactoryMethodShouldBeConstructor,
                factoryCandidate.Identifier.GetLocation(),
                factoryCandidate.Identifier.Text));
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
    /// Emits the generated source file using raw text when <see cref="Emitter.EmitResult"/> is available.
    /// Each file declares the same <c>static partial class</c> — one per declaring type — and adds
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

        // Usings
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

        // Type parameters
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

        // Source comment showing the original C# member
        var sourceText = member.NormalizeWhitespace().ToFullString();
        foreach (var line in sourceText.Split('\n'))
        {
            var trimmed = line.TrimEnd('\r');
            sb.AppendLine($"        // {trimmed}");
        }

        // {methodSuffix}_Expression() method
        sb.AppendLine($"        static {returnType} {methodSuffix}_Expression{methodTypeParamList}() {methodConstraintClauses}");
        sb.AppendLine("        {");
        sb.Append(emission.Body);
        sb.AppendLine("        }");

        // Transformers (when declared via attribute)
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

    /// <summary>
    /// Extracts a <see cref="ExpressionRegistryEntry"/> from a member declaration.
    /// Returns null when the member does not have [Expressive], is an extension member,
    /// or cannot be represented in the registry (e.g. a generic class member or generic method).
    /// </summary>
    private static ExpressionRegistryEntry? ExtractRegistryEntry(ISymbol memberSymbol)
    {
        var containingType = memberSymbol.ContainingType;

        // Determine whether this entry is metadata-only (excluded from runtime registry
        // but still used for [EditorBrowsable] attribute-only partial file emission).
        var isMetadataOnly = false;
        string? classTypeParameters = null;

        // C# 14 extension type members — metadata-only (fall back to reflection at runtime)
        if (containingType is { IsExtension: true })
        {
            isMetadataOnly = true;
        }

        // Generic classes — metadata-only (registry can't represent open generic types)
        if (containingType.TypeParameters.Length > 0)
        {
            isMetadataOnly = true;
            classTypeParameters = "<" + string.Join(", ", containingType.TypeParameters.Select(tp => tp.Name)) + ">";
        }

        // Determine member kind and lookup name
        ExpressionRegistryMemberType memberKind;
        string memberLookupName;
        var parameterTypeNames = ImmutableArray<string>.Empty;

        if (memberSymbol is IMethodSymbol methodSymbol)
        {
            // Generic methods — metadata-only (same reason as generic classes)
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

        // Build the generated class name and method name using the same logic as Execute
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

    /// <summary>
    /// Processes an [ExpressiveFor] / [ExpressiveForConstructor] stub: resolves the target member,
    /// validates the stub, and emits the expression tree factory source file.
    /// </summary>
    private static void ExecuteFor(
        MethodDeclarationSyntax stubMethod,
        SemanticModel semanticModel,
        IMethodSymbol stubSymbol,
        ExpressiveForAttributeData attributeData,
        ExpressiveGlobalOptions globalOptions,
        Compilation compilation,
        SourceProductionContext context)
    {
        var descriptor = ExpressiveForInterpreter.GetDescriptor(
            semanticModel, stubMethod, stubSymbol, attributeData, globalOptions, context, compilation);

        if (descriptor is null)
            return;

        if (descriptor.MemberName is null)
            throw new InvalidOperationException("Expected a memberName here");

        var generatedClassName = ExpressionClassNameGenerator.GenerateName(
            descriptor.ClassNamespace, descriptor.NestedInClassNames,
            descriptor.MemberName, descriptor.ParameterTypeNames);
        var generatedFileName = $"{generatedClassName}.g.cs";

        if (descriptor.ExpressionTreeEmission is null)
            throw new InvalidOperationException("ExpressionTreeEmission must be set");

        EmitExpressionTreeSource(descriptor, generatedClassName, generatedFileName, stubMethod, compilation, context);
    }

    /// <summary>
    /// Extracts a <see cref="ExpressionRegistryEntry"/> for an [ExpressiveFor] stub.
    /// The entry points to the external target member, not the stub itself.
    /// </summary>
    private static ExpressionRegistryEntry? ExtractRegistryEntryForExternal(
        ((MethodDeclarationSyntax Method, ExpressiveForAttributeData Attribute, ExpressiveGlobalOptions GlobalOptions), Compilation) source)
    {
        var ((method, attribute, globalOptions), compilation) = source;
        var semanticModel = compilation.GetSemanticModel(method.SyntaxTree);
        var stubSymbol = semanticModel.GetDeclaredSymbol(method) as IMethodSymbol;

        if (stubSymbol is null)
            return null;

        // Resolve target type
        var targetType = attribute.TargetTypeMetadataName is not null
            ? compilation.GetTypeByMetadataName(attribute.TargetTypeMetadataName)
            : null;

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

            // Constructor params match stub params directly
            parameterTypeNames = [
                ..stubSymbol.Parameters.Select(p => p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            ];
        }
        else
        {
            var memberName = attribute.MemberName;
            if (memberName is null)
                return null;

            // Determine if this maps to a property or method
            var isProperty = targetType.GetMembers(memberName).OfType<IPropertySymbol>().Any();
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

                // Find the matching target method to get its parameter types (not the stub's)
                var targetMethod = targetType.GetMembers(memberName).OfType<IMethodSymbol>()
                    .Where(m => m.MethodKind is not (MethodKind.PropertyGet or MethodKind.PropertySet))
                    .FirstOrDefault(m =>
                    {
                        var expectedParamCount = m.IsStatic ? m.Parameters.Length : m.Parameters.Length + 1;
                        if (stubSymbol.Parameters.Length != expectedParamCount)
                            return false;

                        // For instance methods, validate that the stub's first parameter matches the target type
                        if (!m.IsStatic &&
                            !SymbolEqualityComparer.Default.Equals(stubSymbol.Parameters[0].Type, targetType))
                            return false;

                        var offset = m.IsStatic ? 0 : 1;
                        for (var i = 0; i < m.Parameters.Length; i++)
                        {
                            if (!SymbolEqualityComparer.Default.Equals(
                                m.Parameters[i].Type, stubSymbol.Parameters[i + offset].Type))
                                return false;
                        }
                        return true;
                    });

                if (targetMethod is null)
                    return null;

                // Use the TARGET method's parameter types (not the stub's)
                parameterTypeNames = [
                    ..targetMethod.Parameters.Select(p => p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                ];
            }
        }

        // Build generated class name using the target type's path
        var classNamespace = targetType.ContainingNamespace.IsGlobalNamespace
            ? null
            : targetType.ContainingNamespace.ToDisplayString();

        var nestedTypePath = GetRegistryNestedTypePath(targetType);

        var generatedClassName = ExpressionClassNameGenerator.GenerateName(
            classNamespace, nestedTypePath, memberLookupName,
            parameterTypeNames.IsEmpty ? null : parameterTypeNames);

        var generatedClassFullName = "ExpressiveSharp.Generated." + generatedClassName;

        return new ExpressionRegistryEntry(
            DeclaringTypeFullName: targetTypeFullName,
            MemberKind: memberKind,
            MemberLookupName: memberLookupName,
            GeneratedClassFullName: generatedClassFullName,
            ParameterTypeNames: parameterTypeNames);
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
}
