using System.Linq.Expressions;
using System.Reflection;

namespace ExpressiveSharp.Transformers;

/// <summary>
/// Rewrites <c>string.Concat(new string[] { a, b, c, ... })</c> into a chain of 2/3/4-arg
/// <c>string.Concat</c> calls so providers like EF Core (which can't translate <c>NewArrayInit</c>
/// with non-constant elements) can emit SQL concatenation.
/// </summary>
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

    private static Expression ReduceParts(IList<Expression> parts)
    {
        var i = 0;
        Expression current;

        var firstChunkSize = Math.Min(4, parts.Count);
        current = firstChunkSize switch
        {
            2 => Expression.Call(Concat2, parts[0], parts[1]),
            3 => Expression.Call(Concat3, parts[0], parts[1], parts[2]),
            _ => Expression.Call(Concat4, parts[0], parts[1], parts[2], parts[3]),
        };
        i = firstChunkSize;

        // Each step consumes up to 3 new parts (current + 3 = 4 args max via Concat4).
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
