using System.Linq.Expressions;

namespace ExpressiveSharp.Transformers;

/// <summary>
/// Removes null-conditional patterns from expression trees.
/// Matches the pattern: <c>(x != null) ? x.Member : default(T)</c>
/// and simplifies to: <c>x.Member</c>.
/// </summary>
/// <remarks>
/// This is useful for LINQ providers like EF Core where null propagation
/// is handled by the database engine and the null-check ternary is unnecessary.
/// </remarks>
public sealed class RemoveNullConditionalPatterns : ExpressionVisitor, IExpressionTreeTransformer
{
    public Expression Transform(Expression expression)
        => Visit(expression);

    protected override Expression VisitConditional(ConditionalExpression node)
    {
        // Pattern: (x != null) ? whenTrue : default(T)
        // where whenTrue accesses a member on x
        if (node.Test is BinaryExpression { NodeType: ExpressionType.NotEqual } notEqual
            && IsNullConstant(notEqual.Right)
            && IsDefaultOrNull(node.IfFalse))
        {
            var receiver = notEqual.Left;

            // Check if the whenTrue branch accesses the same receiver
            if (AccessesReceiver(node.IfTrue, receiver))
            {
                // Strip the null check — return the whenTrue branch with
                // any nested null-conditional patterns also removed.
                // If types differ (e.g., int? vs int from Convert), unwrap the Convert.
                var result = Visit(node.IfTrue);
                if (result is UnaryExpression { NodeType: ExpressionType.Convert } convert
                    && convert.Operand.Type == node.Type)
                {
                    return convert.Operand;
                }
                return result;
            }
        }

        return base.VisitConditional(node);
    }

    private static bool IsNullConstant(Expression expr)
        => expr is ConstantExpression { Value: null }
        || expr is DefaultExpression;

    private static bool IsDefaultOrNull(Expression expr)
        => expr is DefaultExpression
        || (expr is ConstantExpression { Value: null });

    /// <summary>
    /// Returns true if <paramref name="expr"/> directly or transitively accesses
    /// <paramref name="receiver"/> (e.g., receiver.Prop, receiver.Prop.SubProp).
    /// </summary>
    private static bool AccessesReceiver(Expression expr, Expression receiver)
    {
        var current = expr;

        // Unwrap Convert (e.g., Convert(x.Length, int?) → x.Length)
        while (current is UnaryExpression { NodeType: ExpressionType.Convert } convert)
        {
            current = convert.Operand;
        }

        // Unwrap nested conditionals (chained ?. patterns)
        if (current is ConditionalExpression nested)
        {
            if (nested.Test is BinaryExpression { NodeType: ExpressionType.NotEqual } nestedNotEqual)
            {
                current = nestedNotEqual.Left;
            }
        }

        // Walk up the member access chain
        while (current is MemberExpression member)
        {
            if (member.Expression is not null && ExpressionsEqual(member.Expression, receiver))
                return true;
            current = member.Expression;
        }

        // Direct reference
        if (current is not null && ExpressionsEqual(current, receiver))
            return true;

        // Method call on receiver (e.g., receiver.Method())
        if (expr is MethodCallExpression methodCall)
        {
            if (methodCall.Object is not null && ExpressionsEqual(methodCall.Object, receiver))
                return true;
        }

        return false;
    }

    private static bool ExpressionsEqual(Expression a, Expression b)
    {
        if (a == b) return true;
        if (a is ParameterExpression pa && b is ParameterExpression pb)
            return pa.Name == pb.Name && pa.Type == pb.Type;
        if (a is MemberExpression ma && b is MemberExpression mb)
            return ma.Member == mb.Member
                && ma.Expression is not null && mb.Expression is not null
                && ExpressionsEqual(ma.Expression, mb.Expression);
        return false;
    }
}
