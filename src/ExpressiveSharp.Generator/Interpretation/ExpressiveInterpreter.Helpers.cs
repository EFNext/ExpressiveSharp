using ExpressiveSharp.Generator.Infrastructure;
using ExpressiveSharp.Generator.Models;
using ExpressiveSharp.Generator.SyntaxRewriters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExpressiveSharp.Generator.Interpretation;

static internal partial class ExpressiveInterpreter
{
    private static void ApplyParameterList(
        ParameterListSyntax parameterList,
        DeclarationSyntaxRewriter rewriter,
        ExpressiveDescriptor descriptor)
    {
        foreach (var p in ((ParameterListSyntax)rewriter.Visit(parameterList)).Parameters)
        {
            descriptor.ParametersList = descriptor.ParametersList!.AddParameters(p);
        }
    }

    private static void ApplyTypeParameters(
        MethodDeclarationSyntax methodDecl,
        DeclarationSyntaxRewriter rewriter,
        ExpressiveDescriptor descriptor)
    {
        if (methodDecl.TypeParameterList is not null)
        {
            descriptor.TypeParameterList = SyntaxFactory.TypeParameterList();
            foreach (var tp in ((TypeParameterListSyntax)rewriter.Visit(methodDecl.TypeParameterList)).Parameters)
            {
                descriptor.TypeParameterList = descriptor.TypeParameterList.AddParameters(tp);
            }
        }

        if (methodDecl.ConstraintClauses.Any())
        {
            descriptor.ConstraintClauses = SyntaxFactory.List(
                methodDecl.ConstraintClauses
                    .Select(x => (TypeParameterConstraintClauseSyntax)rewriter.Visit(x)));
        }
    }

    // Tries in order: property-level expression body, getter's expression body, then the
    // first `return` in a block-bodied getter.
    private static ExpressionSyntax? TryGetPropertyGetterExpression(PropertyDeclarationSyntax prop)
    {
        if (prop.ExpressionBody?.Expression is { } exprBody)
        {
            return exprBody;
        }

        if (prop.AccessorList is not null)
        {
            var getter = prop.AccessorList.Accessors
                .FirstOrDefault(a => a.IsKind(SyntaxKind.GetAccessorDeclaration));

            if (getter?.ExpressionBody?.Expression is { } getterExpr)
            {
                return getterExpr;
            }

            if (getter?.Body?.Statements.OfType<ReturnStatementSyntax>().FirstOrDefault()?.Expression is { } returnExpr)
            {
                return returnExpr;
            }
        }

        return null;
    }

    private static bool ReportRequiresBodyAndFail(
        SourceProductionContext context,
        SyntaxNode node,
        string memberName)
    {
        context.ReportDiagnostic(Diagnostic.Create(
            Diagnostics.RequiresBodyDefinition,
            node.GetLocation(),
            memberName));
        return false;
    }
}
