using System.Linq.Expressions;
using System.Reflection;

namespace ExpressiveSharp.Transformers;

/// <summary>
/// Rewrites <c>string.Concat(new string[] { a, b, c, ... })</c> into a chain of
/// <c>string.Concat</c> calls using the 2-, 3-, or 4-arg overloads.
/// </summary>
/// <remarks>
/// <para>
/// EF Core and other LINQ providers cannot translate <c>NewArrayInit</c> with non-constant
/// elements to SQL. This transformer flattens the array form into individual <c>Concat</c>
/// calls that providers can translate to SQL string concatenation.
/// </para>
/// <para>
/// For example, <c>string.Concat(new string[] { a, b, c, d, e })</c> becomes
/// <c>string.Concat(string.Concat(a, b, c, d), e)</c>.
/// </para>
/// </remarks>
public sealed class FlattenConcatArrayCalls : ExpressionVisitor, IExpressionTreeTransformer
{
    private static readonly MethodInfo Concat2 = typeof(string).GetMethod(
        nameof(string.Concat), [typeof(string), typeof(string)])!;

    private static readonly MethodInfo Concat3 = typeof(string).GetMethod(
        nameof(string.Concat), [typeof(string), typeof(string), typeof(string)])!;

    private static readonly MethodInfo Concat4 = typeof(string).GetMethod(
        nameof(string.Concat), [typeof(string), typeof(string), typeof(string), typeof(string)])!;

    public Expression Transform(Expression expression)
        => Visit(expression);

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        var visited = (MethodCallExpression)base.VisitMethodCall(node);

        // Match: string.Concat(string[]) where the argument is NewArrayInit
        if (visited.Method.DeclaringType == typeof(string)
            && visited.Method.Name == nameof(string.Concat)
            && visited.Arguments.Count == 1
            && visited.Arguments[0] is NewArrayExpression { NodeType: ExpressionType.NewArrayInit } arrayExpr
            && arrayExpr.Type == typeof(string[]))
        {
            var parts = arrayExpr.Expressions;

            if (parts.Count == 0)
                return Expression.Constant("", typeof(string));

            if (parts.Count == 1)
                return parts[0];

            return ReduceParts(parts);
        }

        return visited;
    }

    /// <summary>
    /// Reduces a list of parts into chained Concat calls, consuming up to 4 parts
    /// at a time starting from the left.
    /// </summary>
    private static Expression ReduceParts(IList<Expression> parts)
    {
        var i = 0;
        Expression current;

        // First chunk: consume up to 4
        var firstChunkSize = Math.Min(4, parts.Count);
        current = firstChunkSize switch
        {
            2 => Expression.Call(Concat2, parts[0], parts[1]),
            3 => Expression.Call(Concat3, parts[0], parts[1], parts[2]),
            _ => Expression.Call(Concat4, parts[0], parts[1], parts[2], parts[3]),
        };
        i = firstChunkSize;

        // Remaining parts: fold with Concat2, consuming up to 3 new parts per step
        // (current + up to 3 new = 4 args max via Concat4)
        while (i < parts.Count)
        {
            var remaining = parts.Count - i;
            if (remaining >= 3)
            {
                current = Expression.Call(Concat4, current, parts[i], parts[i + 1], parts[i + 2]);
                i += 3;
            }
            else if (remaining == 2)
            {
                current = Expression.Call(Concat3, current, parts[i], parts[i + 1]);
                i += 2;
            }
            else
            {
                current = Expression.Call(Concat2, current, parts[i]);
                i += 1;
            }
        }

        return current;
    }
}
