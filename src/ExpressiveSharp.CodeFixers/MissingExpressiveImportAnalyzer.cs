using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExpressiveSharp.CodeFixers;

/// <summary>
/// Reports EXP0021 when a LINQ method on an <c>IExpressiveQueryable&lt;T&gt;</c> receiver resolves to
/// <c>System.Linq.Queryable</c> instead of the ExpressiveSharp delegate-based overload — typically
/// because <c>using ExpressiveSharp;</c> is missing.
/// Reports EXP0022 when no ExpressiveSharp stub exists for the called method, meaning the
/// <c>IExpressiveQueryable</c> chain will be broken.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MissingExpressiveImportAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor StubNotResolved = new(
        id: "EXP0021",
        title: "IExpressiveQueryable LINQ method resolves to Queryable",
        messageFormat: "LINQ method '{0}' on IExpressiveQueryable<T> resolves to System.Linq.Queryable instead of the ExpressiveSharp overload. Add 'using ExpressiveSharp;' to enable expression tree rewriting and maintain the IExpressiveQueryable chain.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NoStubExists = new(
        id: "EXP0022",
        title: "No IExpressiveQueryable overload for Queryable method",
        messageFormat: "Method '{0}' from System.Linq.Queryable has no IExpressiveQueryable<T> overload. The result will be IQueryable<T>, breaking the IExpressiveQueryable chain.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(StubNotResolved, NoStubExists);

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

        // Only care about System.Linq.Queryable methods
        if (!IsQueryableMethod(method))
            return;

        // Get the receiver expression and its type
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
            return;

        var receiverType = context.SemanticModel.GetTypeInfo(
            memberAccess.Expression, context.CancellationToken).Type;
        if (receiverType is null || !IsOrImplementsExpressiveQueryable(receiverType))
            return;

        // Check if a matching stub exists in ExpressiveQueryableLinqExtensions
        var stubType = context.SemanticModel.Compilation
            .GetTypeByMetadataName("ExpressiveSharp.ExpressiveQueryableLinqExtensions");

        if (stubType != null && stubType.GetMembers(method.Name).Length > 0)
        {
            // Stub exists but wasn't resolved — missing using
            context.ReportDiagnostic(Diagnostic.Create(
                StubNotResolved,
                memberAccess.Name.GetLocation(),
                method.Name));
        }
        else
        {
            // No stub exists — chain will be broken
            context.ReportDiagnostic(Diagnostic.Create(
                NoStubExists,
                memberAccess.Name.GetLocation(),
                method.Name));
        }
    }

    private static bool IsQueryableMethod(IMethodSymbol method) =>
        method.ContainingType?.Name == "Queryable" &&
        method.ContainingType.ContainingNamespace?.ToDisplayString() == "System.Linq";

    private static bool IsOrImplementsExpressiveQueryable(ITypeSymbol type)
    {
        if (IsExpressiveQueryableType(type))
            return true;

        foreach (var iface in type.AllInterfaces)
        {
            if (IsExpressiveQueryableType(iface))
                return true;
        }

        return false;
    }

    private static bool IsExpressiveQueryableType(ITypeSymbol type) =>
        type.Name == "IExpressiveQueryable" &&
        type.ContainingNamespace?.ToDisplayString() == "ExpressiveSharp";
}
