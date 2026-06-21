using System.Collections.Immutable;
using ExpressiveSharp.CodeFixers.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExpressiveSharp.CodeFixers;

/// <summary>
/// Reports EXP0032 when a member overrides an <c>[Expressive]</c> member but is not itself
/// <c>[Expressive]</c>, so instances of its type silently fall back to the base body in
/// expression-tree expansion. Walking <em>up</em> the override chain keeps this cheap and
/// cross-assembly (the derived-side successor to the retired EXP0024).
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class OverrideMissingExpressiveAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor OverrideShouldBeExpressive = new(
        id: "EXP0032",
        title: "Override of an [Expressive] member is missing [Expressive]",
        messageFormat: "'{0}' overrides an [Expressive] member but is not itself marked [Expressive]. In expression-tree expansion (e.g. EF Core, MongoDB) instances of this type fall back to the base body instead of this override. Add [Expressive] so it participates in polymorphic dispatch, or [NotExpressive] to silence this.",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(OverrideShouldBeExpressive);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method, SymbolKind.Property);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        var symbol = context.Symbol;
        if (!symbol.IsOverride)
            return;

        // Property accessors arrive as SymbolKind.Method too; the property is handled separately.
        if (symbol is IMethodSymbol { MethodKind: not MethodKind.Ordinary })
            return;

        if (ExpressiveSymbolHelpers.HasExpressiveAttribute(symbol) ||
            ExpressiveSymbolHelpers.HasNotExpressiveAttribute(symbol))
            return;

        if (!OverriddenChainHasExpressive(symbol))
            return;

        if (symbol.DeclaringSyntaxReferences.Length == 0)
            return;

        var declSyntax = symbol.DeclaringSyntaxReferences[0].GetSyntax(context.CancellationToken);
        var location = GetIdentifierLocation(declSyntax) ?? declSyntax.GetLocation();

        context.ReportDiagnostic(Diagnostic.Create(
            OverrideShouldBeExpressive,
            location,
            additionalLocations: new[] { declSyntax.GetLocation() },
            properties: null,
            symbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
    }

    private static bool OverriddenChainHasExpressive(ISymbol symbol)
    {
        for (var current = GetOverridden(symbol); current is not null; current = GetOverridden(current))
        {
            if (ExpressiveSymbolHelpers.HasExpressiveAttribute(current))
                return true;
        }

        return false;
    }

    private static ISymbol? GetOverridden(ISymbol symbol) => symbol switch
    {
        IMethodSymbol m => m.OverriddenMethod,
        IPropertySymbol p => p.OverriddenProperty,
        _ => null
    };

    private static Location? GetIdentifierLocation(SyntaxNode declSyntax) => declSyntax switch
    {
        MethodDeclarationSyntax method => method.Identifier.GetLocation(),
        PropertyDeclarationSyntax property => property.Identifier.GetLocation(),
        _ => null
    };
}
