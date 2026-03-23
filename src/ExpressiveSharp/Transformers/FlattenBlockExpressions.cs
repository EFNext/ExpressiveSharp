using System.Linq.Expressions;

namespace ExpressiveSharp.Transformers;

/// <summary>
/// Inlines block-local variables at their use sites and removes <see cref="BlockExpression"/>
/// nodes, producing a single expression that typical LINQ providers (EF Core, etc.) can translate.
/// </summary>
/// <remarks>
/// <para>
/// Handles block expressions of the form:
/// <code>
/// Block([var1, var2], [
///   Assign(var1, expr1),
///   Assign(var2, expr2),      // may reference var1
///   resultExpression           // may reference var1, var2
/// ])
/// </code>
/// Each variable is replaced with its assigned expression at every use site, and
/// the block is replaced with the final (result) expression.
/// </para>
/// <para>
/// Limitations: variables that are assigned multiple times are not inlined — the
/// block is left as-is in those cases.
/// </para>
/// </remarks>
public sealed class FlattenBlockExpressions : ExpressionVisitor, IExpressionTreeTransformer
{
    public Expression Transform(Expression expression)
        => Visit(expression);

    protected override Expression VisitBlock(BlockExpression node)
    {
        // First, recursively flatten any nested blocks in sub-expressions
        var visited = (BlockExpression)base.VisitBlock(node);

        if (visited.Variables.Count == 0)
        {
            // No variables — just return the last expression (the result)
            return visited.Expressions.Count == 1
                ? visited.Expressions[0]
                : visited;
        }

        // Build a map: variable → assigned expression.
        // Only inline variables that have exactly one assignment.
        var assignments = new Dictionary<ParameterExpression, Expression>();

        foreach (var expr in visited.Expressions)
        {
            if (expr is BinaryExpression { NodeType: ExpressionType.Assign } assign
                && assign.Left is ParameterExpression variable
                && visited.Variables.Contains(variable))
            {
                if (assignments.ContainsKey(variable))
                {
                    // Multiple assignments to the same variable — bail out entirely
                    return visited;
                }

                assignments[variable] = assign.Right;
            }
        }

        // All block variables must have exactly one assignment for safe inlining
        if (assignments.Count != visited.Variables.Count)
            return visited;

        // Resolve transitive dependencies in declaration order.
        // E.g., var a = x * 2; var b = a + 1;
        // → resolve a first, then when resolving b, substitute a's value in b's assignment.
        var resolved = new Dictionary<ParameterExpression, Expression>();
        foreach (var variable in visited.Variables)
        {
            if (!assignments.TryGetValue(variable, out var value))
                return visited;

            var inlined = new VariableInliner(resolved).Visit(value);
            resolved[variable] = inlined;
        }

        // The result is the last expression in the block
        var result = visited.Expressions[^1];

        // Replace all variable references in the result expression
        return new VariableInliner(resolved).Visit(result);
    }

    /// <summary>
    /// Replaces <see cref="ParameterExpression"/> references with their inlined values.
    /// </summary>
    private sealed class VariableInliner(IReadOnlyDictionary<ParameterExpression, Expression> replacements)
        : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
            => replacements.TryGetValue(node, out var replacement) ? replacement : node;
    }
}
