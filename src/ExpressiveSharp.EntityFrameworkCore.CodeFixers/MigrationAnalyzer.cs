using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExpressiveSharp.EntityFrameworkCore.CodeFixers;

/// <summary>
/// Detects EntityFrameworkCore.Projectables API usage and reports diagnostics
/// so that <see cref="MigrationCodeFixProvider"/> can offer automated fixes.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MigrationAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor ProjectableAttribute = new(
        id: "EXP1001",
        title: "Replace [Projectable] with [Expressive]",
        messageFormat: "[Projectable] should be replaced with [Expressive] from ExpressiveSharp",
        category: "Migration",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "The [Projectable] attribute from EntityFrameworkCore.Projectables has been superseded by [Expressive] from ExpressiveSharp.");

    public static readonly DiagnosticDescriptor UseProjectablesCall = new(
        id: "EXP1002",
        title: "Replace UseProjectables() with UseExpressives()",
        messageFormat: "UseProjectables() should be replaced with UseExpressives() from ExpressiveSharp",
        category: "Migration",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "The UseProjectables() extension method has been superseded by UseExpressives() from ExpressiveSharp.EntityFrameworkCore.");

    public static readonly DiagnosticDescriptor ProjectablesUsing = new(
        id: "EXP1003",
        title: "Replace EntityFrameworkCore.Projectables namespace",
        messageFormat: "Namespace '{0}' should be replaced with the ExpressiveSharp equivalent",
        category: "Migration",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "EntityFrameworkCore.Projectables namespaces have been superseded by ExpressiveSharp namespaces.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(ProjectableAttribute, UseProjectablesCall, ProjectablesUsing);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeAttribute, SyntaxKind.Attribute);
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeUsingDirective, SyntaxKind.UsingDirective);
    }

    private static void AnalyzeAttribute(SyntaxNodeAnalysisContext context)
    {
        var attribute = (AttributeSyntax)context.Node;
        var name = attribute.Name.ToString();

        if (name != "Projectable" && name != "ProjectableAttribute")
        {
            return;
        }

        // Verify it's actually the Projectables attribute via semantic model
        var symbolInfo = context.SemanticModel.GetSymbolInfo(attribute, context.CancellationToken);
        if (symbolInfo.Symbol is IMethodSymbol ctor &&
            ctor.ContainingType.ToDisplayString() == "EntityFrameworkCore.Projectables.ProjectableAttribute")
        {
            context.ReportDiagnostic(Diagnostic.Create(ProjectableAttribute, attribute.GetLocation()));
        }
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
            return;
        }

        if (memberAccess.Name.Identifier.Text != "UseProjectables")
        {
            return;
        }

        // UseProjectables() is defined in the Microsoft.EntityFrameworkCore namespace
        // (following EF Core's extension method convention), so we can't verify via
        // containing namespace. The method name is distinctive enough, and we still
        // verify it's an actual method symbol (not just a similarly-named identifier).
        var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken);
        if (symbolInfo.Symbol is IMethodSymbol || symbolInfo.CandidateSymbols.Any(s => s is IMethodSymbol))
        {
            context.ReportDiagnostic(Diagnostic.Create(UseProjectablesCall, invocation.GetLocation()));
        }
    }

    private static void AnalyzeUsingDirective(SyntaxNodeAnalysisContext context)
    {
        var usingDirective = (UsingDirectiveSyntax)context.Node;

        if (usingDirective.Name is null)
        {
            return;
        }

        var nameText = usingDirective.Name.ToString();

        if (nameText.StartsWith("EntityFrameworkCore.Projectables"))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                ProjectablesUsing,
                usingDirective.GetLocation(),
                nameText));
        }
    }
}
