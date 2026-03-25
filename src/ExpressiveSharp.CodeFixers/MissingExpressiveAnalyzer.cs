using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExpressiveSharp.CodeFixers;

/// <summary>
/// Reports EXP0013 when a member referenced inside an [Expressive] body, an
/// <c>ExpressionPolyfill.Create()</c> lambda, or an <c>IRewritableQueryable</c>
/// LINQ lambda has an expandable body but is not marked [Expressive].
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MissingExpressiveAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor MemberCouldBeExpressive = new(
        id: "EXP0013",
        title: "Referenced member could benefit from [Expressive]",
        messageFormat: "Member '{0}' is referenced in an [Expressive] expression but is not marked [Expressive]. Adding [Expressive] would allow its body to be inlined into the expression tree.",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MemberCouldBeExpressive);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Prong 1: members decorated with [Expressive]
        context.RegisterSyntaxNodeAction(AnalyzeExpressiveMember,
            SyntaxKind.MethodDeclaration,
            SyntaxKind.PropertyDeclaration,
            SyntaxKind.ConstructorDeclaration);

        // Prong 2: ExpressionPolyfill.Create() and IRewritableQueryable LINQ lambdas
        context.RegisterSyntaxNodeAction(AnalyzePolyfillInvocation,
            SyntaxKind.InvocationExpression);
    }

    // ── Prong 1: [Expressive] member bodies ─────────────────────────────────

    private static void AnalyzeExpressiveMember(SyntaxNodeAnalysisContext context)
    {
        var memberDecl = (MemberDeclarationSyntax)context.Node;

        var symbol = context.SemanticModel.GetDeclaredSymbol(memberDecl, context.CancellationToken);
        if (symbol is null || !HasExpressiveAttribute(symbol))
            return;

        AnalyzeDescendants(context, memberDecl);
    }

    // ── Prong 2: polyfill / rewritable-queryable lambdas ────────────────────

    private static void AnalyzePolyfillInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken);
        if (symbolInfo.Symbol is not IMethodSymbol method)
            return;

        if (!IsExpressionPolyfillCreate(method) && !IsRewritableQueryableMethod(method))
            return;

        foreach (var arg in invocation.ArgumentList.Arguments)
        {
            if (arg.Expression is LambdaExpressionSyntax lambda)
            {
                AnalyzeDescendants(context, lambda);
            }
        }
    }

    private static bool IsExpressionPolyfillCreate(IMethodSymbol method) =>
        method.Name == "Create" &&
        method.ContainingType?.Name == "ExpressionPolyfill" &&
        method.ContainingType.ContainingNamespace?.ToDisplayString() == "ExpressiveSharp";

    private static bool IsRewritableQueryableMethod(IMethodSymbol method)
    {
        if (!method.IsExtensionMethod)
            return false;

        // Check if the first parameter (the receiver) is IRewritableQueryable<T>
        var originalMethod = method.ReducedFrom ?? method;
        if (originalMethod.Parameters.Length == 0)
            return false;

        var receiverType = originalMethod.Parameters[0].Type;
        return IsOrImplementsRewritableQueryable(receiverType);
    }

    private static bool IsOrImplementsRewritableQueryable(ITypeSymbol type)
    {
        if (IsRewritableQueryableType(type))
            return true;

        foreach (var iface in type.AllInterfaces)
        {
            if (IsRewritableQueryableType(iface))
                return true;
        }

        return false;
    }

    private static bool IsRewritableQueryableType(ITypeSymbol type) =>
        type.Name == "IRewritableQueryable" &&
        type.ContainingNamespace?.ToDisplayString() == "ExpressiveSharp";

    // ── Shared: walk descendants for member references ──────────────────────

    private static void AnalyzeDescendants(SyntaxNodeAnalysisContext context, SyntaxNode scope)
    {
        foreach (var node in scope.DescendantNodes())
        {
            ISymbol? referencedSymbol = null;
            Location? location = null;

            if (node is InvocationExpressionSyntax invocation)
            {
                var info = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken);
                referencedSymbol = info.Symbol;
                location = invocation.Expression is MemberAccessExpressionSyntax memberAccess
                    ? memberAccess.Name.GetLocation()
                    : invocation.Expression.GetLocation();
            }
            else if (node is MemberAccessExpressionSyntax memberAccessExpr &&
                     memberAccessExpr.Parent is not InvocationExpressionSyntax)
            {
                // Property/field access (not a method call receiver)
                var info = context.SemanticModel.GetSymbolInfo(memberAccessExpr, context.CancellationToken);
                if (info.Symbol is IPropertySymbol)
                {
                    referencedSymbol = info.Symbol;
                    location = memberAccessExpr.Name.GetLocation();
                }
            }
            else if (node is IdentifierNameSyntax identifier &&
                     identifier.Parent is not MemberAccessExpressionSyntax &&
                     identifier.Parent is not InvocationExpressionSyntax)
            {
                // Unqualified property access (e.g., `Value` instead of `this.Value`)
                var info = context.SemanticModel.GetSymbolInfo(identifier, context.CancellationToken);
                if (info.Symbol is IPropertySymbol)
                {
                    referencedSymbol = info.Symbol;
                    location = identifier.GetLocation();
                }
            }

            if (referencedSymbol is null || location is null)
                continue;

            WarnIfMissingExpressive(context, referencedSymbol, location);
        }
    }

    // ── Core logic (matches ExpressionTreeEmitter.WarnIfMissingExpressive) ──

    private static void WarnIfMissingExpressive(
        SyntaxNodeAnalysisContext context, ISymbol symbol, Location location)
    {
        if (symbol.DeclaringSyntaxReferences.Length == 0)
            return;

        if (symbol.IsAbstract || symbol.IsExtern)
            return;

        if (HasExpressiveAttribute(symbol))
            return;

        if (!HasExpandableBody(symbol, context.CancellationToken))
            return;

        var declLocation = symbol.DeclaringSyntaxReferences[0]
            .GetSyntax(context.CancellationToken).GetLocation();

        context.ReportDiagnostic(Diagnostic.Create(
            MemberCouldBeExpressive,
            location,
            additionalLocations: new[] { declLocation },
            properties: null,
            symbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
    }

    private static bool HasExpressiveAttribute(ISymbol symbol)
    {
        foreach (var attr in symbol.GetAttributes())
        {
            var attrClass = attr.AttributeClass;
            if (attrClass is null)
                continue;
            if (attrClass.Name == "ExpressiveAttribute" &&
                attrClass.ContainingNamespace?.ToDisplayString() == "ExpressiveSharp")
                return true;
        }
        return false;
    }

    private static bool HasExpandableBody(ISymbol symbol, System.Threading.CancellationToken ct)
    {
        foreach (var syntaxRef in symbol.DeclaringSyntaxReferences)
        {
            var syntax = syntaxRef.GetSyntax(ct);

            if (syntax is MethodDeclarationSyntax methodDecl)
            {
                if (methodDecl.ExpressionBody is not null || methodDecl.Body is not null)
                    return true;
            }
            else if (syntax is PropertyDeclarationSyntax propDecl)
            {
                if (propDecl.ExpressionBody is not null)
                    return true;

                if (propDecl.AccessorList is not null)
                {
                    foreach (var accessor in propDecl.AccessorList.Accessors)
                    {
                        if (accessor.IsKind(SyntaxKind.GetAccessorDeclaration) &&
                            (accessor.ExpressionBody is not null || accessor.Body is not null))
                            return true;
                    }
                }
            }
        }
        return false;
    }
}
