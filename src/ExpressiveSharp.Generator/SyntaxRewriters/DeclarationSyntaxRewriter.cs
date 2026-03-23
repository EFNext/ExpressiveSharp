using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExpressiveSharp.Generator.SyntaxRewriters;

internal class DeclarationSyntaxRewriter : CSharpSyntaxRewriter
{
    readonly SemanticModel _semanticModel;

    public DeclarationSyntaxRewriter(SemanticModel semanticModel)
    {
        _semanticModel = semanticModel;
    }

    public override SyntaxNode? VisitParameter(ParameterSyntax node)
    {
        var visitedNode = base.VisitParameter(node);

        if (visitedNode is ParameterSyntax visitedParameterSyntax)
        {
            // Strip the this keyword of any parameter
            var thisKeywordIndex = visitedParameterSyntax.Modifiers.IndexOf(SyntaxKind.ThisKeyword);
            if (thisKeywordIndex != -1)
            {
                visitedNode = visitedParameterSyntax.WithModifiers(node.Modifiers.RemoveAt(thisKeywordIndex));
            }

            // Strip the params keyword of any parameter
            var paramsKeywordIndex = ((ParameterSyntax)visitedNode).Modifiers.IndexOf(SyntaxKind.ParamsKeyword);
            if (paramsKeywordIndex != -1)
            {
                visitedNode = ((ParameterSyntax)visitedNode).WithModifiers(node.Modifiers.RemoveAt(paramsKeywordIndex));
            }

            // Remove default values from parameters as this is not accepted in an expression tree
            if (visitedParameterSyntax.Default is not null)
            {
                visitedNode = ((ParameterSyntax)visitedNode).WithDefault(null);
            }
        }

        return visitedNode;
    }

    public override SyntaxNode? VisitNullableType(NullableTypeSyntax node)
    {
        var typeInfo = _semanticModel.GetTypeInfo(node);
        if (typeInfo.Type is not null)
        {
            if (typeInfo.Type.TypeKind is not TypeKind.Struct)
            {
                return Visit(node.ElementType)
                    .WithTriviaFrom(node);
            }
        }

        return base.VisitNullableType(node);
    }

    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
    {
        var typeInfo = _semanticModel.GetTypeInfo(node);
        if (typeInfo.Type is not null)
        {
            return SyntaxFactory.ParseTypeName(
                typeInfo.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            ).WithTriviaFrom(node);
        }

        return base.VisitIdentifierName(node);
    }

    public override SyntaxNode? VisitGenericName(GenericNameSyntax node)
    {
        var typeInfo = _semanticModel.GetTypeInfo(node);
        if (typeInfo.Type is not null)
        {
            return SyntaxFactory.ParseTypeName(
                typeInfo.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            ).WithTriviaFrom(node);
        }

        return base.VisitGenericName(node);
    }

    public override SyntaxNode? VisitTupleType(TupleTypeSyntax node)
    {
        var typeInfo = _semanticModel.GetTypeInfo(node);
        if (typeInfo.Type is INamedTypeSymbol { IsTupleType: true } tupleType)
        {
            return SyntaxFactory.ParseTypeName(
                FormatValueTupleType(tupleType)
            ).WithTriviaFrom(node);
        }

        return base.VisitTupleType(node);
    }

    internal static string FormatValueTupleType(INamedTypeSymbol tupleType)
    {
        var typeArgs = (tupleType.TupleUnderlyingType ?? tupleType).TypeArguments;
        var formattedArgs = typeArgs.Select(t =>
            t is INamedTypeSymbol { IsTupleType: true } nestedTuple
                ? FormatValueTupleType(nestedTuple)
                : t.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        return $"global::System.ValueTuple<{string.Join(", ", formattedArgs)}>";
    }

    public override SyntaxNode? VisitQualifiedName(QualifiedNameSyntax node)
    {
        var typeInfo = _semanticModel.GetTypeInfo(node);
        if (typeInfo.Type is not null)
        {
            return SyntaxFactory.ParseTypeName(
                typeInfo.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            ).WithTriviaFrom(node);
        }

        return base.VisitQualifiedName(node);
    }
}
