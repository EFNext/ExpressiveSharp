using System.Linq.Expressions;

namespace ExpressiveSharp.Transformers;

/// <summary>
/// Simplifies <c>(x != null) ? x.Member : default(T)</c> to <c>x.Member</c> — useful for
/// providers like EF Core where the database engine already null-propagates.
/// </summary>
public sealed class RemoveNullConditionalPatterns : ExpressionVisitor, IExpressionTreeTransformer
{
    public Expression Transform(Expression expression)
        => Visit(expression);

    protected override Expression VisitConditional(ConditionalExpression node)
    {
        // We refuse to strip when whenTrue contains a method call (e.g. x?.ToUpper()).
        // Method-call null propagation is not guaranteed across providers — MongoDB's
        // $toUpper on null/missing returns "" instead of null, so dropping the null
        // check silently changes semantics. Pure property access chains
        // (Customer?.Address?.Country) are safe in both SQL and MongoDB aggregation.
        if (node.Test is BinaryExpression { NodeType: ExpressionType.NotEqual } notEqual
            && IsNullConstant(notEqual.Right)
            && IsDefaultOrNull(node.IfFalse))
        {
            var receiver = notEqual.Left;

            if (AccessesReceiver(node.IfTrue, receiver) && !ContainsMethodCall(node.IfTrue))
            {
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

    // True if there's a method call anywhere in the receiver/member chain,
    // including inside nested null-conditional patterns.
    private static bool ContainsMethodCall(Expression expr)
    {
        var current = expr;

        while (current is not null)
        {
            switch (current)
            {
                case MethodCallExpression:
                    return true;
                case UnaryExpression { NodeType: ExpressionType.Convert } convert:
                    current = convert.Operand;
                    break;
                case MemberExpression member:
                    current = member.Expression;
                    break;
                case ConditionalExpression conditional:
                    // Other branches are null/default by construction.
                    return ContainsMethodCall(conditional.IfTrue);
                default:
                    return false;
            }
        }

        return false;
    }

    private static bool IsNullConstant(Expression expr)
        => expr is ConstantExpression { Value: null }
        || expr is DefaultExpression;

    private static bool IsDefaultOrNull(Expression expr)
        => expr is DefaultExpression
        || (expr is ConstantExpression { Value: null });

    // True if expr directly or transitively accesses receiver (e.g. receiver.Prop.SubProp).
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

        while (current is MemberExpression member)
        {
            if (member.Expression is not null && ExpressionsEqual(member.Expression, receiver))
                return true;
            current = member.Expression;
        }

        if (current is not null && ExpressionsEqual(current, receiver))
            return true;

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
        if (a is MemberExpression ma && b is MemberExpression mb)
            return ma.Member == mb.Member
                && ma.Expression is not null && mb.Expression is not null
                && ExpressionsEqual(ma.Expression, mb.Expression);
        return false;
    }
}
