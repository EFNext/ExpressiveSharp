using System.Linq.Expressions;

namespace ExpressiveSharp.Transformers;

/// <summary>
/// Inlines single-assignment block-local variables and replaces the
/// <see cref="BlockExpression"/> with its final result, producing a tree most LINQ providers
/// (EF Core, etc.) can translate. Variables assigned multiple times are left untouched.
/// </summary>
public sealed class FlattenBlockExpressions : ExpressionVisitor, IExpressionTreeTransformer
{
    public Expression Transform(Expression expression)
        => Visit(expression);

    protected override Expression VisitBlock(BlockExpression node)
    {
        var visited = (BlockExpression)base.VisitBlock(node);

        if (visited.Variables.Count == 0)
        {
            return visited.Expressions.Count == 1
                ? visited.Expressions[0]
                : visited;
        }

        // Only inline variables that have exactly one assignment; otherwise bail out.
        var assignments = new Dictionary<ParameterExpression, Expression>();

        foreach (var expr in visited.Expressions)
        {
            if (expr is BinaryExpression { NodeType: ExpressionType.Assign } assign
                && assign.Left is ParameterExpression variable
                && visited.Variables.Contains(variable))
            {
                if (assignments.ContainsKey(variable))
                    return visited;

                assignments[variable] = assign.Right;
            }
        }

        if (assignments.Count != visited.Variables.Count)
            return visited;

        // Resolve transitive references in declaration order so later variables see
        // the already-inlined values of earlier ones (var a = x * 2; var b = a + 1).
        var resolved = new Dictionary<ParameterExpression, Expression>();
        foreach (var variable in visited.Variables)
        {
            if (!assignments.TryGetValue(variable, out var value))
                return visited;

            var inlined = new VariableInliner(resolved).Visit(value);
            resolved[variable] = inlined;
        }

        var result = visited.Expressions[^1];
        return new VariableInliner(resolved).Visit(result);
    }

    private sealed class VariableInliner(IReadOnlyDictionary<ParameterExpression, Expression> replacements)
        : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
            => replacements.TryGetValue(node, out var replacement) ? replacement : node;
    }
}
