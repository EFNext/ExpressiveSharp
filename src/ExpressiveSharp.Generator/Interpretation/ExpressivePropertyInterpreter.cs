using ExpressiveSharp.Generator.Emitter;
using ExpressiveSharp.Generator.Infrastructure;
using ExpressiveSharp.Generator.Models;
using ExpressiveSharp.Generator.SyntaxRewriters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExpressiveSharp.Generator.Interpretation;

/// <summary>
/// Interprets <c>[ExpressiveProperty]</c> stubs: validates placement rules, builds an
/// <see cref="ExpressiveDescriptor"/> keyed on the synthesized target property, and attaches a
/// <see cref="SynthesizedPropertySpec"/> so the generator emits the partial-class declaration.
/// </summary>
/// <remarks>
/// Rules (v1):
/// <list type="number">
///   <item>Stub must be a property with a top-level expression body (<c>=&gt; expr</c>).</item>
///   <item>Stub must be an instance member (static stubs rejected).</item>
///   <item>Target property name must be supplied explicitly as a string literal.</item>
/// </list>
/// </remarks>
static internal class ExpressivePropertyInterpreter
{
    public static (ExpressiveDescriptor Descriptor, SynthesizedPropertySpec Spec)? GetDescriptor(
        SemanticModel semanticModel,
        PropertyDeclarationSyntax stubProperty,
        IPropertySymbol stubSymbol,
        ExpressivePropertyAttributeData attributeData,
        SourceProductionContext context)
    {
        var stubLocation = stubProperty.Identifier.GetLocation();
        var containingType = stubSymbol.ContainingType;
        var targetName = attributeData.TargetName;

        if (string.IsNullOrWhiteSpace(targetName))
        {
            // Constructor guarantees non-null in the attribute, so a null here means the attribute
            // was unparseable (e.g. a literal null argument). Silently ignore — the C# compiler
            // has already surfaced its own diagnostic for the null literal.
            return null;
        }

        // Rule 2: instance only.
        if (stubSymbol.IsStatic)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.ExpressivePropertyInstanceOnly,
                stubLocation,
                stubSymbol.Name));
            return null;
        }

        // Rule 1: expression body required (top-level `=> expr` form). Reject any accessor list —
        // even a `{ get => expr; }` that's semantically equivalent — to keep the surface minimal.
        if (stubProperty.ExpressionBody is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.ExpressivePropertyRequiresExpressionBody,
                stubLocation,
                stubSymbol.Name));
            return null;
        }

        // Containing type must be partial (class / struct / record / record struct).
        if (!IsPartialType(containingType))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.ExpressivePropertyRequiresPartial,
                stubLocation,
                containingType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            return null;
        }

        // Target name must not collide with an existing declared or inherited member on the type.
        // Declared members first (EXP0031) because that's the more common mistake and deserves the
        // dedicated "use [ExpressiveFor] instead" steering.
        if (containingType.GetMembers(targetName!).Any(m =>
            m is IPropertySymbol or IMethodSymbol or IFieldSymbol or IEventSymbol))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.ExpressivePropertyTargetExists,
                stubLocation,
                targetName,
                containingType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            return null;
        }

        // Inherited-member shadowing gets its own diagnostic — silent hiding is a footgun.
        if (FindInheritedMember(containingType, targetName!) is { } inheritedFrom)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.ExpressivePropertyShadowsInherited,
                stubLocation,
                targetName,
                inheritedFrom.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            return null;
        }

        // Build the descriptor from the stub's expression body.
        var descriptor = BuildDescriptor(
            semanticModel, context, stubProperty, stubSymbol,
            attributeData, containingType, targetName!);
        if (descriptor is null) return null;

        // Synthesis spec for the partial-class emitter.
        var returnType = stubSymbol.Type;
        var useTernary = IsNullablePropertyType(returnType);
        var propertyTypeFqn = returnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var backingFieldTypeFqn = useTernary ? propertyTypeFqn : MakeNullableTypeFqn(returnType);

        var spec = new SynthesizedPropertySpec
        {
            PropertyTypeFqn = propertyTypeFqn,
            PropertyName = targetName!,
            StubMemberName = stubSymbol.Name,
            StubIsMethod = false, // rule 1 enforces property stub
            UseTernaryShape = useTernary,
            BackingFieldTypeFqn = backingFieldTypeFqn,
            ContainingTypeName = containingType.Name,
            ContainingTypeNamespace = containingType.ContainingNamespace.IsGlobalNamespace
                ? null
                : containingType.ContainingNamespace.ToDisplayString(),
            ContainingTypePath = GetNestedInClassPath(containingType).ToList(),
            ContainingTypeKeyword = GetTypeKeyword(containingType),
        };

        descriptor.SynthesisSpec = spec;
        return (descriptor, spec);
    }

    private static ExpressiveDescriptor? BuildDescriptor(
        SemanticModel semanticModel,
        SourceProductionContext context,
        PropertyDeclarationSyntax stubProperty,
        IPropertySymbol stubSymbol,
        ExpressivePropertyAttributeData attributeData,
        INamedTypeSymbol containingType,
        string targetName)
    {
        // Rule 1 guaranteed us an expression body.
        var bodySyntax = stubProperty.ExpressionBody!.Expression;

        var rewriter = new DeclarationSyntaxRewriter(semanticModel);
        var returnTypeName = rewriter.Visit(stubProperty.Type).ToString();

        var containingNamespace = containingType.ContainingNamespace.IsGlobalNamespace
            ? null
            : containingType.ContainingNamespace.ToDisplayString();

        var descriptor = new ExpressiveDescriptor
        {
            UsingDirectives = stubProperty.SyntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>(),
            ClassName = containingType.Name,
            ClassNamespace = containingNamespace,
            MemberName = targetName,
            NestedInClassNames = GetNestedInClassPath(containingType),
            TargetClassNamespace = containingNamespace,
            TargetNestedInClassNames = GetNestedInClassPath(containingType),
            ParametersList = SyntaxFactory.ParameterList(),
            ReturnTypeName = returnTypeName,
        };

        foreach (var transformerTypeName in attributeData.TransformerTypeNames)
            descriptor.DeclaredTransformerTypeNames.Add(transformerTypeName);

        // Instance stub → prepend @this so IInstanceReferenceOperation binds correctly.
        var thisTypeFqn = containingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        descriptor.ParametersList = descriptor.ParametersList.AddParameters(
            SyntaxFactory.Parameter(SyntaxFactory.Identifier("@this"))
                .WithType(SyntaxFactory.ParseTypeName(thisTypeFqn)));

        var emitterParams = new List<EmitterParameter>
        {
            new EmitterParameter("@this", thisTypeFqn, isThis: true)
        };

        var delegateTypeFqn = $"global::System.Func<{thisTypeFqn}, {returnTypeName}>";
        var emitter = new ExpressionTreeEmitter(semanticModel, context);
        descriptor.ExpressionTreeEmission = emitter.Emit(bodySyntax, emitterParams,
            returnTypeName, delegateTypeFqn);

        return descriptor;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static bool IsNullablePropertyType(ITypeSymbol type)
    {
        if (type.NullableAnnotation == NullableAnnotation.Annotated) return true;
        if (type is INamedTypeSymbol named
            && named.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
        {
            return true;
        }
        return false;
    }

    private static string MakeNullableTypeFqn(ITypeSymbol type)
    {
        var fqn = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        if (type.IsValueType)
            return $"global::System.Nullable<{fqn}>";
        return fqn.EndsWith("?") ? fqn : fqn + "?";
    }

    private static bool IsPartialType(INamedTypeSymbol type)
    {
        foreach (var reference in type.DeclaringSyntaxReferences)
        {
            if (reference.GetSyntax() is TypeDeclarationSyntax typeDecl
                && typeDecl.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                return true;
            }
        }
        return false;
    }

    private static string GetTypeKeyword(INamedTypeSymbol type)
    {
        foreach (var reference in type.DeclaringSyntaxReferences)
        {
            switch (reference.GetSyntax())
            {
                case ClassDeclarationSyntax: return "class";
                case RecordDeclarationSyntax rec:
                    return rec.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword)
                        ? "record struct"
                        : "record";
                case StructDeclarationSyntax: return "struct";
                case InterfaceDeclarationSyntax: return "interface";
            }
        }
        return type.TypeKind switch
        {
            TypeKind.Struct => "struct",
            TypeKind.Interface => "interface",
            _ => "class",
        };
    }

    private static IEnumerable<string> GetNestedInClassPath(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.ContainingType is not null)
        {
            foreach (var name in GetNestedInClassPath(typeSymbol.ContainingType))
                yield return name;
        }
        yield return typeSymbol.Name;
    }

    private static INamedTypeSymbol? FindInheritedMember(INamedTypeSymbol type, string memberName)
    {
        // Walk the base chain — System.Object itself is not interesting for user-defined collisions
        // but we include it for completeness (e.g. `ToString` as a target name).
        var current = type.BaseType;
        while (current is not null)
        {
            if (current.GetMembers(memberName).Any(m =>
                m is IPropertySymbol or IMethodSymbol or IFieldSymbol or IEventSymbol))
            {
                return current;
            }
            current = current.BaseType;
        }
        return null;
    }
}
