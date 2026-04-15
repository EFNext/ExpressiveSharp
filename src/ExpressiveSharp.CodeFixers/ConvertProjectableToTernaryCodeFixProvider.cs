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
using Microsoft.CodeAnalysis.Formatting;

namespace ExpressiveSharp.CodeFixers;

/// <summary>
/// Code fix for EXP0024: the <c>??</c> projectable pattern cannot be applied to a nullable
/// property type. Rewrites the getter and setter to the ternary <c>_hasFoo ? field : formula</c>
/// pattern and inserts a private <c>bool _hasFoo</c> flag field into the containing type.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConvertProjectableToTernaryCodeFixProvider))]
[Shared]
public sealed class ConvertProjectableToTernaryCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("EXP0024");

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
            return;

        foreach (var diagnostic in context.Diagnostics)
        {
            var node = root.FindNode(diagnostic.Location.SourceSpan);
            var property = node.FirstAncestorOrSelf<PropertyDeclarationSyntax>();
            if (property is null)
                continue;

            if (!TryExtractCoalesceParts(property, out _, out _, out _, out _))
                continue;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Convert to ternary 'has-value flag' pattern",
                    createChangedDocument: ct => ApplyFixAsync(context.Document, property, ct),
                    equivalenceKey: "EXP0024_ConvertToTernary"),
                diagnostic);
        }
    }

    private static async Task<Document> ApplyFixAsync(
        Document document,
        PropertyDeclarationSyntax property,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
            return document;

        if (!TryExtractCoalesceParts(property, out var getAccessor, out var setAccessor, out var backingFieldRef, out var formula))
            return document;

        var containingType = property.FirstAncestorOrSelf<TypeDeclarationSyntax>();
        if (containingType is null)
            return document;

        var flagName = $"_has{property.Identifier.Text}";

        var flagIdent = SyntaxFactory.IdentifierName(flagName);

        // Build the new get accessor: `flagName ? <backingFieldRef> : (<formula>)`.
        var ternary = SyntaxFactory.ConditionalExpression(
            flagIdent,
            backingFieldRef!,
            SyntaxFactory.ParenthesizedExpression(formula!));

        var newGetAccessor = getAccessor!
            .WithBody(null)
            .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(ternary))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        // Build the new set/init accessor body: `{ _hasFoo = true; <backingFieldRef> = value; }`.
        var flagAssignment = SyntaxFactory.ExpressionStatement(
            SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                flagIdent,
                SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)));

        var valueAssignment = SyntaxFactory.ExpressionStatement(
            SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                backingFieldRef!,
                SyntaxFactory.IdentifierName("value")));

        var newSetAccessor = setAccessor!
            .WithExpressionBody(null)
            .WithSemicolonToken(default)
            .WithBody(SyntaxFactory.Block(flagAssignment, valueAssignment));

        // Replace accessors inside the property's accessor list.
        var accessorList = property.AccessorList!;
        var newAccessorList = accessorList.ReplaceNodes(
            new AccessorDeclarationSyntax[] { getAccessor, setAccessor },
            (original, _) => ReferenceEquals(original, getAccessor) ? (SyntaxNode)newGetAccessor : newSetAccessor);

        var newProperty = property.WithAccessorList((AccessorListSyntax)newAccessorList);

        // Build the flag field declaration: `private bool _hasFoo;`.
        var flagField = SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                    SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(flagName))))
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)))
            .WithAdditionalAnnotations(Formatter.Annotation);

        // Insert the flag field immediately before the property declaration so related state
        // stays visually grouped.
        var members = containingType.Members;
        var propertyIndex = members.IndexOf(property);
        var newMembers = members
            .RemoveAt(propertyIndex)
            .Insert(propertyIndex, newProperty)
            .Insert(propertyIndex, flagField);

        var newContainingType = containingType.WithMembers(newMembers);

        var newRoot = root.ReplaceNode(containingType, newContainingType);
        return document.WithSyntaxRoot(newRoot);
    }

    /// <summary>
    /// Inspects the property syntax for the Coalesce-shape Projectable pattern and extracts the
    /// get accessor, set/init accessor, the backing field reference (the left operand of
    /// <c>??</c>), and the formula (the right operand). Returns <c>false</c> if the pattern
    /// doesn't match — in which case the fix is not offered.
    /// </summary>
    private static bool TryExtractCoalesceParts(
        PropertyDeclarationSyntax property,
        out AccessorDeclarationSyntax? getAccessor,
        out AccessorDeclarationSyntax? setAccessor,
        out ExpressionSyntax? backingFieldRef,
        out ExpressionSyntax? formula)
    {
        getAccessor = null;
        setAccessor = null;
        backingFieldRef = null;
        formula = null;

        if (property.AccessorList is null)
            return false;

        foreach (var accessor in property.AccessorList.Accessors)
        {
            if (accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
                getAccessor = accessor;
            else if (accessor.IsKind(SyntaxKind.InitAccessorDeclaration) || accessor.IsKind(SyntaxKind.SetAccessorDeclaration))
                setAccessor = accessor;
        }

        if (getAccessor is null || setAccessor is null)
            return false;

        if (!TryGetSingleExpression(getAccessor, out var getBody))
            return false;

        // getBody must be a coalesce `left ?? right`.
        if (getBody is not BinaryExpressionSyntax { RawKind: (int)SyntaxKind.CoalesceExpression } coalesce)
            return false;

        // Left must be either the C# 14 `field` keyword (parsed as FieldExpressionSyntax) or a
        // manually-declared backing field identifier. The generator's recognizer already
        // validated the symbol, so we only need to match the syntax shape here.
        if (!IsBackingFieldReference(coalesce.Left))
            return false;

        if (!TryGetSingleAssignmentValue(setAccessor, out var setAssignment))
            return false;

        if (!IsBackingFieldReference(setAssignment.Left)
            || !BackingFieldReferencesMatch(coalesce.Left, setAssignment.Left))
            return false;

        backingFieldRef = (ExpressionSyntax)coalesce.Left.WithoutTrivia();
        formula = UnwrapParentheses(coalesce.Right).WithoutTrivia();
        return true;
    }

    private static bool IsBackingFieldReference(ExpressionSyntax expression) =>
        expression is IdentifierNameSyntax
        || expression.IsKind(SyntaxKind.FieldExpression);

    private static bool BackingFieldReferencesMatch(ExpressionSyntax a, ExpressionSyntax b)
    {
        // Both `field` keyword references are always equivalent; for identifiers compare text.
        if (a.IsKind(SyntaxKind.FieldExpression) && b.IsKind(SyntaxKind.FieldExpression))
            return true;
        if (a is IdentifierNameSyntax ai && b is IdentifierNameSyntax bi)
            return ai.Identifier.Text == bi.Identifier.Text;
        return false;
    }

    private static bool TryGetSingleExpression(AccessorDeclarationSyntax accessor, out ExpressionSyntax expression)
    {
        if (accessor.ExpressionBody is not null)
        {
            expression = accessor.ExpressionBody.Expression;
            return true;
        }

        if (accessor.Body is { Statements: { Count: 1 } stmts }
            && stmts[0] is ReturnStatementSyntax { Expression: { } ret })
        {
            expression = ret;
            return true;
        }

        expression = null!;
        return false;
    }

    private static bool TryGetSingleAssignmentValue(AccessorDeclarationSyntax accessor, out AssignmentExpressionSyntax assignment)
    {
        if (accessor.ExpressionBody is { Expression: AssignmentExpressionSyntax a1 })
        {
            assignment = a1;
            return true;
        }

        if (accessor.Body is { Statements: { Count: 1 } stmts }
            && stmts[0] is ExpressionStatementSyntax { Expression: AssignmentExpressionSyntax a2 })
        {
            assignment = a2;
            return true;
        }

        assignment = null!;
        return false;
    }

    private static ExpressionSyntax UnwrapParentheses(ExpressionSyntax expression)
    {
        while (expression is ParenthesizedExpressionSyntax paren)
            expression = paren.Expression;
        return expression;
    }
}
