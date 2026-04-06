using System.Linq.Expressions;

namespace ExpressiveSharp.Transformers;

/// <summary>
/// Replaces <see cref="ExpressionType.Throw"/> nodes with <see cref="Expression.Default(Type)"/>.
/// </summary>
/// <remarks>
/// This is useful for LINQ providers like EF Core that cannot translate
/// <c>Expression.Throw</c> to SQL. The throw node is replaced with a
/// type-compatible default, preserving the surrounding tree structure
/// (e.g., <c>Coalesce</c>, <c>Condition</c>) and its type contracts.
/// </remarks>
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
