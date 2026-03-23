using System.Linq.Expressions;
using ExpressiveSharp.Services;

namespace ExpressiveSharp.Extensions;

public static class ExpressionExtensions
{
    /// <summary>
    /// Replaces all calls to properties and methods that are marked with the <c>Expressive</c>
    /// attribute with their respective expression trees, then applies any globally registered
    /// default transformers (see <see cref="ExpressiveDefaults"/>).
    /// </summary>
    public static Expression ExpandExpressives(this Expression expression)
    {
        var expanded = new ExpressiveReplacer(new ExpressiveResolver()).Replace(expression);
        var transformers = ExpressiveDefaults.GetTransformers();
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
