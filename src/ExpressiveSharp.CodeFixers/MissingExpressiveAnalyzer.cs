using System.Collections.Immutable;
using System.Linq;
using ExpressiveSharp.CodeFixers.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExpressiveSharp.CodeFixers;

/// <summary>
/// Reports EXP0013 when a member referenced inside an [Expressive] body, an
/// <c>ExpressionPolyfill.Create()</c> lambda, or an <c>IExpressiveQueryable</c>
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

        // Prong 2: ExpressionPolyfill.Create() and IExpressiveQueryable LINQ lambdas
        context.RegisterSyntaxNodeAction(AnalyzePolyfillInvocation,
            SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeExpressiveMember(SyntaxNodeAnalysisContext context)
    {
        var memberDecl = (MemberDeclarationSyntax)context.Node;

        var symbol = context.SemanticModel.GetDeclaredSymbol(memberDecl, context.CancellationToken);
        if (symbol is null || !ExpressiveSymbolHelpers.HasExpressiveAttribute(symbol))
            return;

        AnalyzeDescendants(context, memberDecl);
    }

    private static void AnalyzePolyfillInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken);
        if (symbolInfo.Symbol is not IMethodSymbol method)
            return;

        if (!IsExpressionPolyfillCreate(method) && !IsExpressiveQueryableMethod(method))
            return;

        foreach (var arg in invocation.ArgumentList.Arguments)
        {
            if (arg.Expression is LambdaExpressionSyntax lambda)
            {
                AnalyzeDescendants(context, lambda);
            }
            else
            {
                // Handle method group syntax: .Select(MyMethod)
                var argSymbol = context.SemanticModel.GetSymbolInfo(arg.Expression, context.CancellationToken);
                if (argSymbol.Symbol is IMethodSymbol methodGroup)
                {
                    WarnIfMissingExpressive(context, methodGroup, arg.Expression.GetLocation());
                }
            }
        }
    }

    private static bool IsExpressionPolyfillCreate(IMethodSymbol method) =>
        method.Name == "Create" &&
        method.ContainingType?.Name == "ExpressionPolyfill" &&
        method.ContainingType.ContainingNamespace?.ToDisplayString() == "ExpressiveSharp";

    private static bool IsExpressiveQueryableMethod(IMethodSymbol method)
    {
        if (!method.IsExtensionMethod)
            return false;

        var originalMethod = method.ReducedFrom ?? method;
        if (originalMethod.Parameters.Length == 0)
            return false;

        var receiverType = originalMethod.Parameters[0].Type;
        return IsOrImplementsExpressiveQueryable(receiverType);
    }

    private static bool IsOrImplementsExpressiveQueryable(ITypeSymbol type)
        => ExpressiveSymbolHelpers.IsOrImplementsExpressiveQueryable(type);

    private static void AnalyzeDescendants(SyntaxNodeAnalysisContext context, SyntaxNode scope)
    {
        foreach (var node in scope.DescendantNodes())
        {
            ISymbol? referencedSymbol = null;
            Location? location = null;

            if (node is InvocationExpressionSyntax invocation)
            {
                var info = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken);
                if (info.Symbol is not IMethodSymbol invokedMethod)
                    continue;

                // Skip enum method calls — the generator expands these via TryEmitEnumMethodExpansion
                if (HasEnumReceiver(invokedMethod, invocation, context.SemanticModel, context.CancellationToken))
                    continue;

                referencedSymbol = invokedMethod;
                location = invocation.Expression is MemberAccessExpressionSyntax memberAccess
                    ? memberAccess.Name.GetLocation()
                    : invocation.Expression.GetLocation();
            }
            else if (node is MemberAccessExpressionSyntax memberAccessExpr &&
                     memberAccessExpr.Parent is not InvocationExpressionSyntax)
            {
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

    // Generator already expands enum/Nullable<Enum> receivers via TryEmitEnumMethodExpansion,
    // so EXP0013 would be a false positive.
    private static bool HasEnumReceiver(
        IMethodSymbol method,
        InvocationExpressionSyntax invocation,
        SemanticModel semanticModel,
        System.Threading.CancellationToken ct)
    {
        ITypeSymbol? receiverType = null;

        if (!method.IsStatic && method.ReceiverType is not null)
        {
            receiverType = method.ReceiverType;
        }
        else if (method.IsExtensionMethod)
        {
            var original = method.ReducedFrom ?? method;
            if (original.Parameters.Length > 0)
                receiverType = original.Parameters[0].Type;
        }

        return receiverType is not null && IsEnumOrNullableEnum(receiverType);
    }

    private static bool IsEnumOrNullableEnum(ITypeSymbol type)
    {
        if (type.TypeKind == TypeKind.Enum)
            return true;

        if (type is INamedTypeSymbol { IsGenericType: true } namedType &&
            namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T &&
            namedType.TypeArguments.Length == 1 &&
            namedType.TypeArguments[0].TypeKind == TypeKind.Enum)
            return true;

        return false;
    }

    // Mirrors ExpressionTreeEmitter.WarnIfMissingExpressive.
    private static void WarnIfMissingExpressive(
        SyntaxNodeAnalysisContext context, ISymbol symbol, Location location)
    {
        if (symbol.DeclaringSyntaxReferences.Length == 0)
            return;

        if (symbol.IsAbstract || symbol.IsExtern)
            return;

        if (ExpressiveSymbolHelpers.HasExpressiveAttribute(symbol))
            return;

        if (ExpressiveSymbolHelpers.HasNotExpressiveAttribute(symbol))
            return;

        // A sibling stub with [ExpressiveProperty("X")] or [ExpressiveFor(... "X")]
        // on the same containing type registers an expression for this member.
        if (HasSiblingMappingTargetingMember(symbol))
            return;

        if (!ExpressiveSymbolHelpers.HasExpandableBody(symbol, context.CancellationToken))
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

    /// <summary>
    /// True when a sibling stub declares <c>[ExpressiveProperty("X")]</c> or
    /// <c>[ExpressiveFor("X")]</c> targeting <paramref name="symbol"/>. Such siblings register
    /// an entry for the member even without a direct <c>[Expressive]</c> attribute.
    /// </summary>
    private static bool HasSiblingMappingTargetingMember(ISymbol symbol)
    {
        var containingType = symbol.ContainingType;
        if (containingType is null)
            return false;

        var targetName = symbol.Name;

        foreach (var sibling in containingType.GetMembers())
        {
            if (SymbolEqualityComparer.Default.Equals(sibling, symbol))
                continue;

            foreach (var attr in sibling.GetAttributes())
            {
                if (attr.AttributeClass is not { } attrClass) continue;
                var ns = attrClass.ContainingNamespace?.ToDisplayString();
                if (ns != "ExpressiveSharp.Mapping") continue;

                if (attrClass.Name == "ExpressivePropertyAttribute"
                    && ExtractStringArg(attr, 0) == targetName)
                {
                    return true;
                }

                if (attrClass.Name == "ExpressiveForAttribute" && MapsExpressiveForTo(attr, containingType, targetName))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool MapsExpressiveForTo(AttributeData attr, INamedTypeSymbol containingType, string targetName)
    {
        if (attr.ConstructorArguments.Length == 1 &&
            attr.ConstructorArguments[0].Value is string singleArg)
        {
            return singleArg == targetName;
        }

        if (attr.ConstructorArguments.Length == 2 &&
            attr.ConstructorArguments[0].Value is INamedTypeSymbol targetType &&
            attr.ConstructorArguments[1].Value is string twoArgName)
        {
            return twoArgName == targetName
                && SymbolEqualityComparer.Default.Equals(targetType, containingType);
        }

        return false;
    }

    private static string? ExtractStringArg(AttributeData attr, int index) =>
        attr.ConstructorArguments.Length > index
            ? attr.ConstructorArguments[index].Value as string
            : null;
}
