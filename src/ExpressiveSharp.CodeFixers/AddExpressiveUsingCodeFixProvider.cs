using System.Collections.Immutable;
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
/// Provides a code fix for EXP0021 that adds <c>using ExpressiveSharp;</c> to bring
/// the delegate-based LINQ stubs into scope.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddExpressiveUsingCodeFixProvider))]
public sealed class AddExpressiveUsingCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create("EXP0021");

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
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Add 'using ExpressiveSharp'",
                    createChangedDocument: _ => Task.FromResult(
                        context.Document.WithSyntaxRoot(EnsureUsingDirective(root))),
                    equivalenceKey: "EXP0021_AddUsing"),
                diagnostic);
        }
    }

    private static SyntaxNode EnsureUsingDirective(SyntaxNode root)
    {
        const string requiredNamespace = "ExpressiveSharp";

        if (root is CompilationUnitSyntax compilationUnit)
        {
            foreach (var usingDirective in compilationUnit.Usings)
            {
                if (usingDirective.Name?.ToString() == requiredNamespace)
                    return root;
            }

            var newUsing = SyntaxFactory.UsingDirective(
                SyntaxFactory.ParseName(requiredNamespace))
                .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

            return compilationUnit.WithUsings(compilationUnit.Usings.Add(newUsing));
        }

        return root;
    }
}
