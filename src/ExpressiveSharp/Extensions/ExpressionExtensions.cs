using System.Diagnostics;
using System.Linq.Expressions;
using ExpressiveSharp.Diagnostics;
using ExpressiveSharp.Services;

namespace ExpressiveSharp;

public static class ExpressionExtensions
{
    /// <summary>
    /// Replaces calls to <c>[Expressive]</c> members with their generated expression trees,
    /// then applies the transformers registered on <see cref="ExpressiveOptions.Default"/>.
    /// </summary>
    public static Expression ExpandExpressives(this Expression expression)
        => expression.ExpandExpressives(ExpressiveOptions.Default);

    /// <summary>
    /// Like <see cref="ExpandExpressives(Expression)"/> but uses transformers from the given options.
    /// </summary>
    public static Expression ExpandExpressives(this Expression expression, ExpressiveOptions options)
        => ExpandExpressivesCore(expression, options.GetTransformers());

    /// <summary>
    /// Like <see cref="ExpandExpressives(Expression)"/> but uses the explicitly supplied transformers.
    /// </summary>
    public static Expression ExpandExpressives(
        this Expression expression,
        params IExpressionTreeTransformer[] transformers)
        => ExpandExpressivesCore(expression, transformers);

    private static Expression ExpandExpressivesCore(
        Expression expression,
        IReadOnlyList<IExpressionTreeTransformer> transformers)
    {
        using var activity = ExpressiveDiagnostics.ActivitySource.StartActivity("Expressive.Expand");

        var measureDuration = activity is not null || ExpressiveDiagnostics.ExpansionDurationMs.Enabled;
        var startTimestamp = measureDuration ? Stopwatch.GetTimestamp() : 0L;

        var expanded = new ExpressiveReplacer(new ExpressiveResolver()).Replace(expression);
        for (var i = 0; i < transformers.Count; i++)
        {
            expanded = transformers[i].Transform(expanded);
        }

        if (activity is not null)
        {
            activity.SetTag("transformer.count", transformers.Count);
        }

        if (activity is not null || ExpressiveDiagnostics.ExpansionNodeCount.Enabled)
        {
            var nodeCount = ExpansionNodeCounter.Count(expanded);
            ExpressiveDiagnostics.ExpansionNodeCount.Record(nodeCount);
            activity?.SetTag("expansion.node_count", nodeCount);
        }

        if (measureDuration)
        {
            var elapsedMs = Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;
            ExpressiveDiagnostics.ExpansionDurationMs.Record(elapsedMs);
            activity?.SetTag("expansion.duration_ms", elapsedMs);
        }

        return expanded;
    }

    private sealed class ExpansionNodeCounter : ExpressionVisitor
    {
        private int _count;

        public static int Count(Expression? expression)
        {
            if (expression is null) return 0;
            var counter = new ExpansionNodeCounter();
            counter.Visit(expression);
            return counter._count;
        }

        public override Expression? Visit(Expression? node)
        {
            if (node is null) return null;
            _count++;
            return base.Visit(node);
        }
    }
}
