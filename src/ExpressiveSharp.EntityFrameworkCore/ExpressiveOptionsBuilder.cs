namespace ExpressiveSharp.EntityFrameworkCore;

/// <summary>
/// Builder for configuring ExpressiveSharp EF Core integration.
/// Passed to the <c>UseExpressives(options => ...)</c> callback to register plugins.
/// </summary>
public sealed class ExpressiveOptionsBuilder
{
    internal List<IExpressivePlugin> Plugins { get; } = [];

    internal bool ShouldPreserveThrowExpressions { get; private set; }

    /// <summary>
    /// Registers an <see cref="IExpressivePlugin"/> that contributes services and/or
    /// expression tree transformers to the EF Core pipeline.
    /// </summary>
    public ExpressiveOptionsBuilder AddPlugin(IExpressivePlugin plugin)
    {
        Plugins.Add(plugin);
        return this;
    }

    /// <summary>
    /// Prevents the <see cref="Transformers.ReplaceThrowWithDefault"/> transformer from
    /// being applied. When set, <c>Expression.Throw</c> nodes are preserved in the
    /// expression tree, and the LINQ provider is responsible for translating them.
    /// </summary>
    public ExpressiveOptionsBuilder PreserveThrowExpressions()
    {
        ShouldPreserveThrowExpressions = true;
        return this;
    }
}
