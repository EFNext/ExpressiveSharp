using System.Collections.Immutable;
using ExpressiveSharp.CodeFixers.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExpressiveSharp.CodeFixers;

/// <summary>
/// Reports EXP0036 at the *dropout call* itself: a method invocation whose receiver implements
/// <c>IExpressiveQueryable&lt;T&gt;</c> but whose return type is plain <c>IQueryable&lt;T&gt;</c>.
/// The chain stops being expressive at this call; downstream LINQ skips ExpressiveSharp rewriting.
/// </summary>
/// <remarks>
/// Exemptions:
/// <list type="bullet">
///   <item><description><c>AsQueryable()</c> — sanctioned explicit downcast.</description></item>
///   <item><description>Methods marked <c>[NotExpressive]</c> — opt-out for intentional dropouts.</description></item>
/// </list>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExpressiveQueryableDropoutAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor ExpressiveQueryableDropout = new(
        id: "EXP0036",
        title: "IExpressiveQueryable<T> chain dropped to plain IQueryable<T>",
        messageFormat: "'{0}' returns IQueryable<T> from an IExpressiveQueryable<T> receiver, dropping the expressive chain. Downstream LINQ skips ExpressiveSharp rewriting and [Expressive] members may evaluate on the client. Add an IExpressiveQueryable<T>-typed overload of '{0}', wrap the result with .AsExpressive(), or mark the method [NotExpressive] if the dropout is intentional.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Once an IExpressiveQueryable<T> chain is upcast back to plain IQueryable<T>, the ExpressiveSharp rewrite layer is no longer applied to subsequent calls. User-defined helpers that take and return IQueryable<T> are the most common cause; sibling overloads typed on IExpressiveQueryable<T> preserve the chain.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(ExpressiveQueryableDropout);

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

        var calledName = memberAccess.Name.Identifier.Text;

        // Sanctioned explicit downcast — user knows exactly what they're doing.
        if (calledName == "AsQueryable")
            return;

        var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken);
        if (symbolInfo.Symbol is not IMethodSymbol method)
            return;

        // Method-level opt-out. Honored same as EXP0027 honors it on referenced members.
        if (ExpressiveSymbolHelpers.HasNotExpressiveAttribute(method))
            return;

        var receiverType = context.SemanticModel.GetTypeInfo(
            memberAccess.Expression, context.CancellationToken).Type;
        if (receiverType is null)
            return;
        if (!ExpressiveSymbolHelpers.IsOrImplementsExpressiveQueryable(receiverType))
            return;

        var resultType = context.SemanticModel.GetTypeInfo(invocation, context.CancellationToken).Type;
        if (resultType is null)
            return;

        // Terminating calls (.ToList, .Count, scalars, void) don't continue the chain — no dropout.
        if (!ImplementsIQueryable(resultType))
            return;

        // Still expressive — chain preserved, no dropout.
        if (ExpressiveSymbolHelpers.IsOrImplementsExpressiveQueryable(resultType))
            return;

        context.ReportDiagnostic(Diagnostic.Create(
            ExpressiveQueryableDropout,
            memberAccess.Name.GetLocation(),
            properties: null,
            calledName));
    }

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
