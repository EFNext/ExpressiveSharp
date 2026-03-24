namespace ExpressiveSharp.Services;

/// <summary>
/// Global configuration for expression tree transformers applied by
/// <see cref="Extensions.ExpressionExtensions.ExpandExpressives(System.Linq.Expressions.Expression)"/>.
/// Consumer libraries register default transformers at startup.
/// </summary>
public static class ExpressiveDefaults
{
    private static readonly List<IExpressionTreeTransformer> _transformers = [];
    private static readonly object _lock = new();

    /// <summary>
    /// Registers additional default transformers that will be applied to all
    /// <c>ExpandExpressives()</c> calls unless overridden at the call site.
    /// </summary>
    public static void AddTransformers(params IExpressionTreeTransformer[] transformers)
    {
        lock (_lock)
        {
            _transformers.AddRange(transformers);
        }
    }

    /// <summary>
    /// Clears all registered default transformers.
    /// </summary>
    public static void ClearTransformers()
    {
        lock (_lock)
        {
            _transformers.Clear();
        }
    }

    /// <summary>
    /// Returns a snapshot of the currently registered default transformers.
    /// </summary>
    internal static IReadOnlyList<IExpressionTreeTransformer> GetTransformers()
    {
        lock (_lock)
        {
            return _transformers.ToArray();
        }
    }
}
