using ExpressiveSharp.Generator.Infrastructure;
using ExpressiveSharp.Generator.Models;
using ExpressiveSharp.Generator.SyntaxRewriters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExpressiveSharp.Generator.Interpretation;

static internal partial class ExpressiveInterpreter
{
    public static ExpressiveDescriptor? GetDescriptor(
        SemanticModel semanticModel,
        MemberDeclarationSyntax member,
        ISymbol memberSymbol,
        ExpressiveAttributeData expressiveAttribute,
        ExpressiveGlobalOptions globalOptions,
        SourceProductionContext context,
        Compilation? compilation = null)
    {
        // Detect C# 14 extension member context
        var isExtensionMember = memberSymbol.ContainingType is { IsExtension: true };
        IParameterSymbol? extensionParameter = null;
        ITypeSymbol? extensionReceiverType = null;

        if (isExtensionMember && memberSymbol.ContainingType is { } extensionType)
        {
            extensionParameter = extensionType.ExtensionParameter;
            extensionReceiverType = extensionParameter?.Type;
        }

        var declarationSyntaxRewriter = new DeclarationSyntaxRewriter(semanticModel);

        // Build base descriptor (class names, namespaces, @this parameter, target class)
        var methodSymbol = memberSymbol as IMethodSymbol;
        var descriptor = BuildBaseDescriptor(
            member, memberSymbol, methodSymbol,
            isExtensionMember, extensionParameter, extensionReceiverType);

        // Populate declared transformers from attribute
        foreach (var typeName in expressiveAttribute.TransformerTypeNames)
            descriptor.DeclaredTransformerTypeNames.Add(typeName);

        // Fill descriptor from the body
        var success = member switch
        {
            MethodDeclarationSyntax methodDecl =>
                TryApplyMethodBody(methodDecl, memberSymbol, semanticModel,
                    declarationSyntaxRewriter, context, descriptor),

            PropertyDeclarationSyntax propDecl =>
                TryApplyPropertyBody(propDecl, memberSymbol, semanticModel,
                    declarationSyntaxRewriter, context, descriptor),

            ConstructorDeclarationSyntax ctorDecl =>
                TryApplyConstructorBody(ctorDecl, memberSymbol, semanticModel,
                    declarationSyntaxRewriter, context, compilation, descriptor),

            _ => false
        };

        return success ? descriptor : null;
    }

    /// <summary>
    /// Builds a <see cref="ExpressiveDescriptor"/> with all fields populated except
    /// <see cref="ExpressiveDescriptor.ReturnTypeName"/> and
    /// <see cref="ExpressiveDescriptor.ExpressionTreeEmission"/>
    /// (those are filled by the body processors).
    /// </summary>
    private static ExpressiveDescriptor BuildBaseDescriptor(
        MemberDeclarationSyntax member,
        ISymbol memberSymbol,
        IMethodSymbol? methodSymbol,
        bool isExtensionMember,
        IParameterSymbol? extensionParameter,
        ITypeSymbol? extensionReceiverType)
    {
        // For extension members, use the outer class for naming
        var classForNaming = isExtensionMember && memberSymbol.ContainingType.ContainingType is not null
            ? memberSymbol.ContainingType.ContainingType
            : memberSymbol.ContainingType;

        // Sanitize constructor name (.ctor / .cctor are not valid C# identifiers, use _ctor)
        var memberName = methodSymbol?.MethodKind is MethodKind.Constructor or MethodKind.StaticConstructor
            ? "_ctor"
            : memberSymbol.Name;

        var descriptor = new ExpressiveDescriptor
        {
            UsingDirectives = member.SyntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>(),
            ClassName = classForNaming.Name,
            ClassNamespace = classForNaming.ContainingNamespace.IsGlobalNamespace
                ? null
                : classForNaming.ContainingNamespace.ToDisplayString(),
            MemberName = memberName,
            NestedInClassNames = isExtensionMember
                ? GetNestedInClassPathForExtensionMember(memberSymbol.ContainingType)
                : GetNestedInClassPath(memberSymbol.ContainingType),
            ParametersList = SyntaxFactory.ParameterList()
        };

        // Collect parameter type names for method overload disambiguation
        if (methodSymbol is not null)
        {
            var parameterTypeNames = methodSymbol.Parameters
                .Select(p => p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                .ToList();

            // For extension members, prepend the extension receiver type to match how the
            // runtime sees the method (receiver is the first implicit parameter).
            if (isExtensionMember && extensionReceiverType is not null)
            {
                parameterTypeNames.Insert(0,
                    extensionReceiverType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            }

            descriptor.ParameterTypeNames = parameterTypeNames;
        }

        // Set up generic type parameters and constraints for the containing class
        if (classForNaming is { IsGenericType: true })
        {
            SetupGenericTypeParameters(descriptor, classForNaming);
        }

        // Add the implicit @this parameter
        if (isExtensionMember && extensionReceiverType is not null)
        {
            descriptor.ParametersList = descriptor.ParametersList.AddParameters(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("@this"))
                    .WithType(SyntaxFactory.ParseTypeName(
                        extensionReceiverType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))));
        }
        else if (!member.Modifiers.Any(SyntaxKind.StaticKeyword) && member is not ConstructorDeclarationSyntax)
        {
            descriptor.ParametersList = descriptor.ParametersList.AddParameters(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("@this"))
                    .WithType(SyntaxFactory.ParseTypeName(
                        memberSymbol.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))));
        }

        // Resolve target class info (used by the registry to associate the projection)
        if (isExtensionMember && extensionReceiverType is not null)
        {
            descriptor.TargetClassNamespace = extensionReceiverType.ContainingNamespace.IsGlobalNamespace
                ? null
                : extensionReceiverType.ContainingNamespace.ToDisplayString();
            descriptor.TargetNestedInClassNames = GetNestedInClassPath(extensionReceiverType);
        }
        else if (methodSymbol is { IsExtensionMethod: true })
        {
            var targetTypeSymbol = methodSymbol.Parameters.First().Type;
            descriptor.TargetClassNamespace = targetTypeSymbol.ContainingNamespace.IsGlobalNamespace
                ? null
                : targetTypeSymbol.ContainingNamespace.ToDisplayString();
            descriptor.TargetNestedInClassNames = GetNestedInClassPath(targetTypeSymbol);
        }
        else
        {
            descriptor.TargetClassNamespace = descriptor.ClassNamespace;
            descriptor.TargetNestedInClassNames = descriptor.NestedInClassNames;
        }

        return descriptor;
    }

    private static IEnumerable<string> GetNestedInClassPath(ITypeSymbol namedTypeSymbol)
    {
        if (namedTypeSymbol.ContainingType is not null)
        {
            foreach (var nestedInClassName in GetNestedInClassPath(namedTypeSymbol.ContainingType))
            {
                yield return nestedInClassName;
            }
        }

        yield return namedTypeSymbol.Name;
    }

    private static IEnumerable<string> GetNestedInClassPathForExtensionMember(ITypeSymbol extensionType)
    {
        var outerType = extensionType.ContainingType;

        if (outerType is not null)
        {
            return GetNestedInClassPath(outerType);
        }

        return [];
    }

    private static TypeConstraintSyntax MakeTypeConstraint(string constraint) =>
        SyntaxFactory.TypeConstraint(SyntaxFactory.IdentifierName(constraint));

    private static void SetupGenericTypeParameters(ExpressiveDescriptor descriptor, INamedTypeSymbol classForNaming)
    {
        descriptor.ClassTypeParameterList = SyntaxFactory.TypeParameterList();

        foreach (var tp in classForNaming.TypeParameters)
        {
            descriptor.ClassTypeParameterList = descriptor.ClassTypeParameterList.AddParameters(
                SyntaxFactory.TypeParameter(tp.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));

            if (tp.ConstraintTypes.IsDefaultOrEmpty)
            {
                continue;
            }

            descriptor.ClassConstraintClauses ??= SyntaxFactory.List<TypeParameterConstraintClauseSyntax>();

            var constraints = new List<TypeConstraintSyntax>();

            if (tp.HasReferenceTypeConstraint)
            {
                constraints.Add(MakeTypeConstraint("class"));
            }

            if (tp.HasValueTypeConstraint)
            {
                constraints.Add(MakeTypeConstraint("struct"));
            }

            if (tp.HasNotNullConstraint)
            {
                constraints.Add(MakeTypeConstraint("notnull"));
            }

            constraints.AddRange(tp.ConstraintTypes
                .Select(c => MakeTypeConstraint(c.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))));

            if (tp.HasConstructorConstraint)
            {
                constraints.Add(MakeTypeConstraint("new()"));
            }

            descriptor.ClassConstraintClauses = descriptor.ClassConstraintClauses.Value.Add(
                SyntaxFactory.TypeParameterConstraintClause(
                    SyntaxFactory.IdentifierName(tp.Name),
                    SyntaxFactory.SeparatedList<TypeParameterConstraintSyntax>(constraints)));
        }
    }
}
