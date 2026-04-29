using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExpressiveSharp.CodeFixers.Internal;

/// <summary>
/// Shared classification helpers used by the diagnostics that reason about
/// <c>[Expressive]</c>-eligible members and Expressive query chains.
/// </summary>
internal static class ExpressiveSymbolHelpers
{
    public static bool HasExpressiveAttribute(ISymbol symbol)
        => HasAttribute(symbol, "ExpressiveAttribute");

    public static bool HasNotExpressiveAttribute(ISymbol symbol)
        => HasAttribute(symbol, "NotExpressiveAttribute");

    private static bool HasAttribute(ISymbol symbol, string attributeTypeName)
    {
        foreach (var attr in symbol.GetAttributes())
        {
            var attrClass = attr.AttributeClass;
            if (attrClass is null)
                continue;
            if (attrClass.Name == attributeTypeName &&
                attrClass.ContainingNamespace?.ToDisplayString() == "ExpressiveSharp")
                return true;
        }
        return false;
    }

    /// <summary>
    /// Returns true when the symbol declares an inspectable body (expression-bodied or block-bodied)
    /// that the source generator could lift into an expression tree if <c>[Expressive]</c> were applied.
    /// </summary>
    public static bool HasExpandableBody(ISymbol symbol, CancellationToken ct)
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
                        if (accessor.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.GetAccessorDeclaration) &&
                            (accessor.ExpressionBody is not null || accessor.Body is not null))
                            return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Returns true when the type is or implements <c>ExpressiveSharp.IExpressiveQueryable&lt;T&gt;</c>.
    /// </summary>
    public static bool IsOrImplementsExpressiveQueryable(ITypeSymbol type)
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
