namespace ExpressiveSharp.Services;

/// <summary>
/// Configuration for expression tree transformers applied by
/// <see cref="Extensions.ExpressionExtensions.ExpandExpressives(System.Linq.Expressions.Expression)"/>.
/// Use <see cref="Default"/> for the global instance, or create new instances for isolated scenarios.
/// </summary>
public class ExpressiveOptions
{
    /// <summary>
    /// The global default instance used by the parameterless
    /// <see cref="Extensions.ExpressionExtensions.ExpandExpressives(System.Linq.Expressions.Expression)"/> overload.
    /// </summary>
    public static ExpressiveOptions Default { get; } = new();

    private readonly List<IExpressionTreeTransformer> _transformers = [];
#if NET9_0_OR_GREATER
    private readonly Lock _lock = new();
#else
    private readonly object _lock = new();
#endif

    /// <summary>
    /// Registers additional transformers that will be applied to
    /// <c>ExpandExpressives()</c> calls using this instance.
    /// </summary>
    public void AddTransformers(params IExpressionTreeTransformer[] transformers)
    {
        lock (_lock)
        {
            _transformers.AddRange(transformers);
        }
    }

    /// <summary>
    /// Clears all registered transformers on this instance.
    /// </summary>
    public void ClearTransformers()
    {
        lock (_lock)
        {
            _transformers.Clear();
        }
    }

    /// <summary>
    /// Returns a snapshot of the currently registered transformers.
    /// </summary>
    public IReadOnlyList<IExpressionTreeTransformer> GetTransformers()
    {
        lock (_lock)
        {
            return _transformers.ToArray();
        }
    }
}
