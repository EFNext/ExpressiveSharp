using System.Linq.Expressions;

namespace ExpressiveSharp.FunctionalTests.Infrastructure;

/// <summary>
/// An expression visitor that rejects expression node types that are not
/// translatable by typical LINQ providers (EF Core, NHibernate, etc.).
/// </summary>
public sealed class StrictExpressionVisitor : ExpressionVisitor
{
    private static readonly HashSet<ExpressionType> _unsupportedNodeTypes =
    [
        ExpressionType.Block,
        ExpressionType.Loop,
        ExpressionType.Goto,
        ExpressionType.Try,
        ExpressionType.Switch,
        ExpressionType.Throw,
        ExpressionType.DebugInfo,
        ExpressionType.RuntimeVariables,
        ExpressionType.Dynamic,
        ExpressionType.Label,
    ];

    public override Expression? Visit(Expression? node)
    {
        if (node != null && _unsupportedNodeTypes.Contains(node.NodeType))
        {
            throw new NotSupportedException(
                $"Expression node type '{node.NodeType}' is not supported by typical LINQ providers. " +
                $"Expression: {node}");
        }

        return base.Visit(node);
    }
}
