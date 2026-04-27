using System.Linq.Expressions;
using System.Reflection;

namespace ExpressiveSharp.Transformers;

/// <summary>
/// Replaces field access on an inline <c>ValueTuple</c> construction with the corresponding
/// constructor argument, e.g. <c>new (a, b).Item1</c> -> <c>a</c>. This lets providers translate
/// <c>(Price, Quantity) == (50.0, 5)</c> as <c>Price == 50.0 AND Quantity == 5</c> instead of
/// failing on the unsupported <c>ValueTuple</c> construction.
/// </summary>
public sealed class FlattenTupleComparisons : ExpressionVisitor, IExpressionTreeTransformer
{
    public Expression Transform(Expression expression)
        => Visit(expression);

    protected override Expression VisitMember(MemberExpression node)
    {
        var visited = (MemberExpression)base.VisitMember(node);

        if (visited.Expression is NewExpression newExpr
            && visited.Member is FieldInfo field
            && IsValueTupleType(newExpr.Type)
            && TryGetItemIndex(field.Name, out var index)
            && index < newExpr.Arguments.Count)
        {
            return newExpr.Arguments[index];
        }

        return visited;
    }

    private static bool IsValueTupleType(Type type)
        => type.IsValueType
           && type.IsGenericType
           && type.FullName is not null
           && type.FullName.StartsWith("System.ValueTuple`", StringComparison.Ordinal);

    private static bool TryGetItemIndex(string fieldName, out int index)
    {
        // Item1..Item7 map to arguments 0..6, Rest maps to argument 7
        if (fieldName == "Rest")
        {
            index = 7;
            return true;
        }

        if (fieldName.Length == 5
            && fieldName.StartsWith("Item", StringComparison.Ordinal)
            && fieldName[4] >= '1' && fieldName[4] <= '7')
        {
            index = fieldName[4] - '1';
            return true;
        }

        index = -1;
        return false;
    }
}
