using System.Linq.Expressions;
using ExpressiveSharp.Services;

namespace ExpressiveSharp;

public static class ExpressionExtensions
{
    /// <summary>
    /// Replaces all calls to properties and methods that are marked with the <c>Expressive</c>
    /// attribute with their respective expression trees, then applies any globally registered
    /// default transformers (see <see cref="ExpressiveOptions.Default"/>).
    /// </summary>
    public static Expression ExpandExpressives(this Expression expression)
        => expression.ExpandExpressives(ExpressiveOptions.Default);

    /// <summary>
    /// Replaces all calls to properties and methods that are marked with the <c>Expressive</c>
    /// attribute with their respective expression trees, then applies transformers from the
    /// specified <see cref="ExpressiveOptions"/> instance.
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
    /// Replaces all calls to properties and methods that are marked with the <c>Expressive</c>
    /// attribute with their respective expression trees, then applies the specified transformers
    /// (instead of the global defaults).
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
