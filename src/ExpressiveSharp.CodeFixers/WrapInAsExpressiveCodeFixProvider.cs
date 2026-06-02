using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExpressiveSharp.CodeFixers;

/// <summary>
/// Provides a code fix for EXP0028 that wraps the chain root with <c>.AsExpressive()</c>
/// (and adds <c>using ExpressiveSharp;</c> if needed) so that subsequent LINQ methods
/// flow through the ExpressiveSharp delegate-based overloads.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(WrapInAsExpressiveCodeFixProvider))]
[Shared]
public sealed class WrapInAsExpressiveCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create("EXP0028");

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document
            .GetSyntaxRootAsync(context.CancellationToken)
            .ConfigureAwait(false);
        if (root is null)
            return;

        foreach (var diagnostic in context.Diagnostics)
        {
            var node = root.FindNode(diagnostic.Location.SourceSpan);
            var invocation = node.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            if (invocation is null)
                continue;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Wrap source with .AsExpressive()",
                    createChangedDocument: ct => WrapWithAsExpressiveAsync(context.Document, invocation, ct),
                    equivalenceKey: "EXP0028_WrapAsExpressive"),
                diagnostic);
        }
    }

    private static async Task<Document> WrapWithAsExpressiveAsync(
        Document document,
        InvocationExpressionSyntax diagnosticInvocation,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
            return document;

        var chainRoot = FindChainRoot(diagnosticInvocation);
        if (chainRoot is null)
            return document;

        var wrapped = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                chainRoot.WithoutTrailingTrivia(),
                SyntaxFactory.IdentifierName("AsExpressive")))
            .WithTriviaFrom(chainRoot);

        var newRoot = root.ReplaceNode(chainRoot, wrapped);
        newRoot = EnsureUsingDirective(newRoot);

        return document.WithSyntaxRoot(newRoot);
    }

    /// <summary>
    /// Walks back through chained <c>X.Method(...).Method(...)</c> invocations to find the leftmost
    /// non-invocation expression — the source of the LINQ chain.
    /// </summary>
    private static ExpressionSyntax? FindChainRoot(InvocationExpressionSyntax invocation)
    {
        ExpressionSyntax current = invocation;

        while (true)
        {
            if (current is InvocationExpressionSyntax inv)
            {
                if (inv.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    current = memberAccess.Expression;
                    continue;
                }

                // Cannot walk further (e.g., delegate invocation, not a fluent chain).
                return null;
            }

            return current;
        }
    }

    private static SyntaxNode EnsureUsingDirective(SyntaxNode root)
    {
        const string requiredNamespace = "ExpressiveSharp";

        if (root is CompilationUnitSyntax compilationUnit)
        {
            if (compilationUnit.Usings.Any(u => u.Name?.ToString() == requiredNamespace))
                return root;

            var newUsing = SyntaxFactory.UsingDirective(
                SyntaxFactory.ParseName(requiredNamespace))
                .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

            return compilationUnit.WithUsings(compilationUnit.Usings.Add(newUsing));
        }

        return root;
    }
}
