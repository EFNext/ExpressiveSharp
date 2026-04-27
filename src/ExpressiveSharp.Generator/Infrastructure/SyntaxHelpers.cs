using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExpressiveSharp.Generator.Infrastructure;

static internal class SyntaxHelpers
{
    // Matches: static method with expression body `=> new ContainingType { ... }`
    // (object initializer only, no constructor arguments) whose return type name
    // equals the containing type name.
    static internal bool TryGetFactoryMethodPattern(
        MethodDeclarationSyntax method,
        out TypeDeclarationSyntax? containingType)
    {
        containingType = null;

        if (method.Parent is not TypeDeclarationSyntax parentType)
            return false;

        if (method.ExpressionBody is null)
            return false;

        if (method.ExpressionBody.Expression is not BaseObjectCreationExpressionSyntax creation)
            return false;

        if (creation.ArgumentList?.Arguments.Count > 0)
            return false;

        if (creation.Initializer is null)
            return false;

        if (creation.Initializer.Expressions.Any(
            e => e is not AssignmentExpressionSyntax { Right: not InitializerExpressionSyntax }))
            return false;

        if (!method.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)))
            return false;

        if (creation is ObjectCreationExpressionSyntax { Type: var createdType })
        {
            var createdTypeName = createdType switch
            {
                IdentifierNameSyntax id => id.Identifier.Text,
                GenericNameSyntax generic => generic.Identifier.Text,
                _ => null
            };

            if (createdTypeName is null || createdTypeName != parentType.Identifier.Text)
                return false;
        }

        var returnTypeName = method.ReturnType switch
        {
            IdentifierNameSyntax id => id.Identifier.Text,
            QualifiedNameSyntax { Right: IdentifierNameSyntax right } => right.Identifier.Text,
            GenericNameSyntax generic => generic.Identifier.Text,
            _ => null
        };

        if (returnTypeName is null || returnTypeName != parentType.Identifier.Text)
            return false;

        containingType = parentType;
        return true;
    }
}
