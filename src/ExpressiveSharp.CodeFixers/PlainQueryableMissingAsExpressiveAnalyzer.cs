using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using ExpressiveSharp.CodeFixers.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExpressiveSharp.CodeFixers;

/// <summary>
/// Reports EXP0027 when a LINQ method on a plain <see cref="System.Linq.IQueryable{T}"/> receiver
/// (not <c>IExpressiveQueryable&lt;T&gt;</c>) is invoked with a lambda whose body references an
/// <c>[Expressive]</c> member. Without <c>.AsExpressive()</c>, the body of the referenced member
/// is not expanded into the query tree and the provider may silently fall back to client-side
/// evaluation or fail to translate the call.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PlainQueryableMissingAsExpressiveAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor PlainQueryableMissingAsExpressive = new(
        id: "EXP0027",
        title: "Plain IQueryable chain references an [Expressive] member without .AsExpressive()",
        messageFormat: "LINQ method '{0}' on a plain IQueryable<T> references the [Expressive] member '{1}'. Without .AsExpressive(), the member's body will not be inlined into the expression tree; the provider may evaluate the call in memory or fail to translate it. Wrap the source with .AsExpressive().",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Marking a member [Expressive] only takes effect when the query chain is wrapped with .AsExpressive() (or runs through a provider with .UseExpressives() configured). Plain IQueryable chains skip the rewrite step, so [Expressive] member bodies fall back to runtime delegate invocation.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(PlainQueryableMissingAsExpressive);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken);
        if (symbolInfo.Symbol is not IMethodSymbol method)
            return;

        // Only LINQ extension methods on Queryable count — Enumerable methods are LINQ-to-Objects
        // and gain nothing from .AsExpressive().
        if (!IsQueryableMethod(method))
            return;

        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
            return;

        var receiverType = context.SemanticModel.GetTypeInfo(
            memberAccess.Expression, context.CancellationToken).Type;
        if (receiverType is null)
            return;

        if (!ImplementsIQueryable(receiverType))
            return;

        // If the chain is already an IExpressiveQueryable, the existing EXP0013 / EXP0021
        // diagnostics cover it.
        if (ExpressiveSymbolHelpers.IsOrImplementsExpressiveQueryable(receiverType))
            return;

        // Walk every lambda argument and look for a reference to an [Expressive] member.
        foreach (var arg in invocation.ArgumentList.Arguments)
        {
            if (arg.Expression is not LambdaExpressionSyntax lambda)
                continue;

            var (referencedSymbol, location) = FindExpressiveReference(
                context.SemanticModel, lambda, context.CancellationToken);

            if (referencedSymbol is null || location is null)
                continue;

            var diagnosticLocation = memberAccess.Name.GetLocation();

            context.ReportDiagnostic(Diagnostic.Create(
                PlainQueryableMissingAsExpressive,
                diagnosticLocation,
                additionalLocations: new[] { location },
                properties: null,
                method.Name,
                referencedSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));

            // Only one report per invocation — additional matches would just be noise.
            return;
        }
    }

    private static (ISymbol? Symbol, Location? Location) FindExpressiveReference(
        SemanticModel semanticModel, LambdaExpressionSyntax lambda, CancellationToken ct)
    {
        foreach (var node in lambda.DescendantNodes())
        {
            ISymbol? candidate = null;
            Location? location = null;

            if (node is InvocationExpressionSyntax inv)
            {
                var info = semanticModel.GetSymbolInfo(inv, ct);
                if (info.Symbol is IMethodSymbol invokedMethod)
                {
                    candidate = invokedMethod;
                    location = inv.Expression is MemberAccessExpressionSyntax m
                        ? m.Name.GetLocation()
                        : inv.Expression.GetLocation();
                }
            }
            else if (node is MemberAccessExpressionSyntax memberAccess &&
                     memberAccess.Parent is not InvocationExpressionSyntax)
            {
                var info = semanticModel.GetSymbolInfo(memberAccess, ct);
                if (info.Symbol is IPropertySymbol)
                {
                    candidate = info.Symbol;
                    location = memberAccess.Name.GetLocation();
                }
            }
            else if (node is IdentifierNameSyntax identifier &&
                     identifier.Parent is not MemberAccessExpressionSyntax &&
                     identifier.Parent is not InvocationExpressionSyntax)
            {
                var info = semanticModel.GetSymbolInfo(identifier, ct);
                if (info.Symbol is IPropertySymbol)
                {
                    candidate = info.Symbol;
                    location = identifier.GetLocation();
                }
            }

            if (candidate is null || location is null)
                continue;

            if (ExpressiveSymbolHelpers.HasNotExpressiveAttribute(candidate))
                continue;

            if (!ExpressiveSymbolHelpers.HasExpressiveAttribute(candidate))
                continue;

            return (candidate, location);
        }

        return (null, null);
    }

    private static bool IsQueryableMethod(IMethodSymbol method) =>
        method.ContainingType?.Name == "Queryable" &&
        method.ContainingType.ContainingNamespace?.ToDisplayString() == "System.Linq";

    private static bool ImplementsIQueryable(ITypeSymbol type)
    {
        if (IsIQueryable(type))
            return true;

        foreach (var iface in type.AllInterfaces)
        {
            if (IsIQueryable(iface))
                return true;
        }

        return false;
    }

    private static bool IsIQueryable(ITypeSymbol type) =>
        type.Name == "IQueryable" &&
        type.ContainingNamespace?.ToDisplayString() == "System.Linq";
}
