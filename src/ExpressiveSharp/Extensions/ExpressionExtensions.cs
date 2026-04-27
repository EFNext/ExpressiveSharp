using System.Linq.Expressions;
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
    {
        var expanded = new ExpressiveReplacer(new ExpressiveResolver()).Replace(expression);
        var transformers = options.GetTransformers();
        foreach (var transformer in transformers)
        {
            expanded = transformer.Transform(expanded);
        }
        return expanded;
    }

    /// <summary>
    /// Like <see cref="ExpandExpressives(Expression)"/> but uses the explicitly supplied transformers.
    /// </summary>
    public static Expression ExpandExpressives(
        this Expression expression,
        params IExpressionTreeTransformer[] transformers)
    {
        var expanded = new ExpressiveReplacer(new ExpressiveResolver()).Replace(expression);
        foreach (var transformer in transformers)
        {
            expanded = transformer.Transform(expanded);
        }
        return expanded;
    }
}
