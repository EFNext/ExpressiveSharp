using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExpressiveSharp.EntityFrameworkCore.CodeFixers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class WindowFunctionLiteralArgsAnalyzer : DiagnosticAnalyzer
{
    private const string WindowFunctionType =
        "ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions.WindowFunction";

    public static readonly DiagnosticDescriptor NtileRequiresPositiveBuckets = new(
        id: "EXP0036",
        title: "WindowFunction.Ntile requires a positive bucket count",
        messageFormat: "WindowFunction.Ntile requires a positive bucket count; literal value {0} produces invalid SQL",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "NTILE(n) divides ordered rows into n buckets. SQL requires n >= 1; non-positive values raise a database error at execution time.");

    public static readonly DiagnosticDescriptor NavigationOffsetMustBeNonNegative = new(
        id: "EXP0037",
        title: "WindowFunction.Lag/Lead offset must be non-negative",
        messageFormat: "WindowFunction.{0} offset must be non-negative; literal value {1} produces invalid SQL",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "LAG and LEAD offsets count rows backward or forward from the current row. SQL requires the offset to be >= 0; negative literals raise a database error at execution time.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(NtileRequiresPositiveBuckets, NavigationOffsetMustBeNonNegative);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
            return;

        var name = memberAccess.Name.Identifier.Text;
        if (name != "Ntile" && name != "Lag" && name != "Lead")
            return;

        var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken);
        if (symbolInfo.Symbol is not IMethodSymbol method)
            return;

        if (method.ContainingType?.ToDisplayString() != WindowFunctionType)
            return;

        var args = invocation.ArgumentList.Arguments;
        if (args.Count == 0)
            return;

        switch (name)
        {
            case "Ntile":
                if (TryGetIntLiteral(context.SemanticModel, args[0].Expression, context.CancellationToken, out var buckets) && buckets <= 0)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        NtileRequiresPositiveBuckets,
                        args[0].GetLocation(),
                        buckets));
                }
                break;

            case "Lag":
            case "Lead":
                if (args.Count >= 2 &&
                    TryGetIntLiteral(context.SemanticModel, args[1].Expression, context.CancellationToken, out var offset) &&
                    offset < 0)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        NavigationOffsetMustBeNonNegative,
                        args[1].GetLocation(),
                        name,
                        offset));
                }
                break;
        }
    }

    private static bool TryGetIntLiteral(
        SemanticModel semanticModel,
        ExpressionSyntax expression,
        System.Threading.CancellationToken cancellationToken,
        out int value)
    {
        var constant = semanticModel.GetConstantValue(expression, cancellationToken);
        if (constant.HasValue && constant.Value is int i)
        {
            value = i;
            return true;
        }

        value = 0;
        return false;
    }
}
