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

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Snapshot global MSBuild defaults once per generator run.
        var globalOptions = context.AnalyzerConfigOptionsProvider
            .Select(static (opts, _) => new ExpressiveGlobalOptions(opts.GlobalOptions));

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

        // Delegate registry file emission to the dedicated ExpressionRegistryEmitter,
        // which uses a string-based CodeWriter instead of SyntaxFactory.
        context.RegisterImplementationSourceOutput(
            registryEntries.Collect(),
            static (spc, entries) => ExpressionRegistryEmitter.Emit(entries, spc));
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
