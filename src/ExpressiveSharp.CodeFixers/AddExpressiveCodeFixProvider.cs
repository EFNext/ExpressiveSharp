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
/// Provides a code fix for EXP0013 that adds [Expressive] to the referenced member's declaration.
/// The diagnostic's additional location (index 0) points to the member declaration.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddExpressiveCodeFixProvider))]
[Shared]
public sealed class AddExpressiveCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create("EXP0013");

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        foreach (var diagnostic in context.Diagnostics)
        {
            if (diagnostic.AdditionalLocations.Count == 0)
                continue;

            var declLocation = diagnostic.AdditionalLocations[0];
            var declDocument = context.Document.Project.Solution.GetDocument(declLocation.SourceTree);
            if (declDocument is null)
                continue;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Add [Expressive] attribute",
                    createChangedSolution: ct => AddExpressiveAttributeAsync(declDocument, declLocation, ct),
                    equivalenceKey: "EXP0013_AddExpressive"),
                diagnostic);
        }
    }

    private static async Task<Solution> AddExpressiveAttributeAsync(
        Document declDocument,
        Location declLocation,
        CancellationToken cancellationToken)
    {
        var root = await declDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
            return declDocument.Project.Solution;

        var node = root.FindNode(declLocation.SourceSpan);
        var memberDecl = node.AncestorsAndSelf().OfType<MemberDeclarationSyntax>().FirstOrDefault();
        if (memberDecl is null)
            return declDocument.Project.Solution;

        var expressiveAttribute = SyntaxFactory.Attribute(
            SyntaxFactory.IdentifierName("Expressive"));

        var attributeList = SyntaxFactory.AttributeList(
            SyntaxFactory.SingletonSeparatedList(expressiveAttribute))
            .WithTrailingTrivia(SyntaxFactory.ElasticMarker);

        var newMemberDecl = memberDecl.WithAttributeLists(
            memberDecl.AttributeLists.Add(attributeList));

        var newRoot = root.ReplaceNode(memberDecl, newMemberDecl);

        // Add using ExpressiveSharp if not already present
        newRoot = EnsureUsingDirective(newRoot);

        return declDocument.Project.Solution.WithDocumentSyntaxRoot(declDocument.Id, newRoot);
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
