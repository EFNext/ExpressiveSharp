using System.Linq.Expressions;

namespace ExpressiveSharp.Transformers;

/// <summary>
/// Replaces <see cref="ExpressionType.Throw"/> nodes with <see cref="Expression.Default(Type)"/>
/// so providers like EF Core (which can't translate <c>Throw</c> to SQL) still see a node of
/// the same type, preserving surrounding <c>Coalesce</c>/<c>Condition</c> structure.
/// </summary>
public sealed class ReplaceThrowWithDefault : ExpressionVisitor, IExpressionTreeTransformer
{
    public Expression Transform(Expression expression)
        => Visit(expression);

    protected override Expression VisitUnary(UnaryExpression node)
    {
        if (node.NodeType == ExpressionType.Throw)
            return Expression.Default(node.Type);
        return base.VisitUnary(node);
    }
}
