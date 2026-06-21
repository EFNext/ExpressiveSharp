namespace ExpressiveSharp.Services;

/// <summary>
/// Set of transformers applied by <c>ExpandExpressives()</c>. Use <see cref="Default"/>
/// for the global instance, or new instances for isolated scenarios.
/// </summary>
public class ExpressiveOptions
{
    public static ExpressiveOptions Default { get; } = new();

    private readonly List<IExpressionTreeTransformer> _transformers = [];
#if NET9_0_OR_GREATER
    private readonly Lock _lock = new();
#else
    private readonly object _lock = new();
#endif

    public void AddTransformers(params IExpressionTreeTransformer[] transformers)
    {
        lock (_lock)
        {
            _transformers.AddRange(transformers);
        }
    }

    public void ClearTransformers()
    {
        lock (_lock)
        {
            _transformers.Clear();
        }
    }

    public IReadOnlyList<IExpressionTreeTransformer> GetTransformers()
    {
        lock (_lock)
        {
            return _transformers.ToArray();
        }
    }

    private bool _polymorphicDispatch = true;

    /// <summary>
    /// Disables runtime polymorphic dispatch of virtual/<c>override</c> <c>[Expressive]</c> members;
    /// they then expand using the static (declared) type only. Per-member <c>[NotExpressive]</c> is
    /// independent of this. Default: enabled.
    /// </summary>
    public void DisablePolymorphicDispatch()
    {
        lock (_lock)
        {
            _polymorphicDispatch = false;
        }
    }

    public bool IsPolymorphicDispatchEnabled
    {
        get
        {
            lock (_lock)
            {
                return _polymorphicDispatch;
            }
        }
    }
}
