using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExpressiveSharp.EntityFrameworkCore.CodeFixers;

/// <summary>
/// Provides code fixes for migrating from EntityFrameworkCore.Projectables to ExpressiveSharp.
/// Handles all three migration diagnostics: attribute rename, method call replacement, and namespace updates.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MigrationCodeFixProvider))]
[Shared]
public sealed class MigrationCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create("EXP1001", "EXP1002", "EXP1003");

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            switch (diagnostic.Id)
            {
                case "EXP1001":
                    RegisterAttributeFix(context, root, diagnostic);
                    break;
                case "EXP1002":
                    RegisterUseProjectablesFix(context, root, diagnostic);
                    break;
                case "EXP1003":
                    RegisterUsingFix(context, root, diagnostic);
                    break;
            }
        }
    }

    private static void RegisterAttributeFix(CodeFixContext context, SyntaxNode root, Diagnostic diagnostic)
    {
        var node = root.FindNode(diagnostic.Location.SourceSpan);
        var attribute = node.AncestorsAndSelf().OfType<AttributeSyntax>().FirstOrDefault();
        if (attribute is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Replace [Projectable] with [Expressive]",
                createChangedDocument: ct => ReplaceProjectableAttributeAsync(context.Document, attribute, ct),
                equivalenceKey: "EXP1001_ReplaceAttribute"),
            diagnostic);
    }

    private static void RegisterUseProjectablesFix(CodeFixContext context, SyntaxNode root, Diagnostic diagnostic)
    {
        var node = root.FindNode(diagnostic.Location.SourceSpan);
        var invocation = node.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
        if (invocation is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Replace UseProjectables() with UseExpressives()",
                createChangedDocument: ct => ReplaceUseProjectablesAsync(context.Document, invocation, ct),
                equivalenceKey: "EXP1002_ReplaceMethod"),
            diagnostic);
    }

    private static void RegisterUsingFix(CodeFixContext context, SyntaxNode root, Diagnostic diagnostic)
    {
        var node = root.FindNode(diagnostic.Location.SourceSpan);
        var usingDirective = node.AncestorsAndSelf().OfType<UsingDirectiveSyntax>().FirstOrDefault();
        if (usingDirective is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Replace with ExpressiveSharp namespace",
                createChangedDocument: ct => ReplaceUsingDirectiveAsync(context.Document, usingDirective, ct),
                equivalenceKey: "EXP1003_ReplaceUsing"),
            diagnostic);
    }

    /// <summary>
    /// Replaces <c>[Projectable(...)]</c> with <c>[Expressive]</c>, keeping only the <c>Transformers</c>
    /// property if present. All other properties are removed as they have no equivalent.
    /// </summary>
    private static async Task<Document> ReplaceProjectableAttributeAsync(
        Document document,
        AttributeSyntax attribute,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        // Build new attribute name
        var oldName = attribute.Name.ToString();
        var newName = oldName == "ProjectableAttribute" ? "ExpressiveAttribute" : "Expressive";
        var newAttributeName = SyntaxFactory.IdentifierName(newName)
            .WithTriviaFrom(attribute.Name);

        // Filter arguments: keep only Transformers
        AttributeArgumentListSyntax? newArgList = null;
        if (attribute.ArgumentList is { Arguments.Count: > 0 })
        {
            var keptArgs = attribute.ArgumentList.Arguments
                .Where(a => a.NameEquals?.Name.Identifier.Text == "Transformers")
                .ToArray();

            if (keptArgs.Length > 0)
            {
                newArgList = SyntaxFactory.AttributeArgumentList(
                    SyntaxFactory.SeparatedList(keptArgs));
            }
        }

        var newAttribute = SyntaxFactory.Attribute(newAttributeName, newArgList)
            .WithTriviaFrom(attribute);

        var newRoot = root.ReplaceNode(attribute, newAttribute);
        return document.WithSyntaxRoot(newRoot);
    }

    /// <summary>
    /// Replaces <c>UseProjectables(...)</c> with <c>UseExpressives()</c>,
    /// removing any configuration callback argument.
    /// </summary>
    private static async Task<Document> ReplaceUseProjectablesAsync(
        Document document,
        InvocationExpressionSyntax invocation,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
            return document;
        }

        // Replace method name
        var newName = SyntaxFactory.IdentifierName("UseExpressives")
            .WithTriviaFrom(memberAccess.Name);
        var newMemberAccess = memberAccess.WithName(newName);

        // Remove all arguments (the callback is no longer needed)
        var newArgList = SyntaxFactory.ArgumentList()
            .WithOpenParenToken(invocation.ArgumentList.OpenParenToken)
            .WithCloseParenToken(invocation.ArgumentList.CloseParenToken);

        var newInvocation = invocation
            .WithExpression(newMemberAccess)
            .WithArgumentList(newArgList);

        var newRoot = root.ReplaceNode(invocation, newInvocation);
        return document.WithSyntaxRoot(newRoot);
    }

    /// <summary>
    /// Replaces <c>using EntityFrameworkCore.Projectables*</c> with the corresponding
    /// <c>using ExpressiveSharp*</c> namespace.
    /// </summary>
    private static async Task<Document> ReplaceUsingDirectiveAsync(
        Document document,
        UsingDirectiveSyntax usingDirective,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null || usingDirective.Name is null)
        {
            return document;
        }

        var oldNamespace = usingDirective.Name.ToString();
        var newNamespace = MapNamespace(oldNamespace);

        if (newNamespace is null)
        {
            // No mapping — remove the using directive entirely
            var newRoot = root.RemoveNode(usingDirective, SyntaxRemoveOptions.KeepLeadingTrivia);
            return document.WithSyntaxRoot(newRoot!);
        }

        var newName = SyntaxFactory.ParseName(newNamespace)
            .WithTriviaFrom(usingDirective.Name);
        var newUsing = usingDirective.WithName(newName);
        var updatedRoot = root.ReplaceNode(usingDirective, newUsing);
        return document.WithSyntaxRoot(updatedRoot);
    }

    /// <summary>
    /// Maps old EntityFrameworkCore.Projectables namespaces to ExpressiveSharp equivalents.
    /// Returns <c>null</c> if the using should be removed (no equivalent exists).
    /// </summary>
    private static string? MapNamespace(string oldNamespace) => oldNamespace switch
    {
        "EntityFrameworkCore.Projectables" => "ExpressiveSharp",
        "EntityFrameworkCore.Projectables.Extensions" => "ExpressiveSharp",
        // Infrastructure namespace (CompatibilityMode, ProjectableOptionsBuilder) has no equivalent
        "EntityFrameworkCore.Projectables.Infrastructure" => null,
        // Any other sub-namespace — map the root
        _ when oldNamespace.StartsWith("EntityFrameworkCore.Projectables.") =>
            "ExpressiveSharp" + oldNamespace.Substring("EntityFrameworkCore.Projectables".Length),
        _ => "ExpressiveSharp",
    };
}
