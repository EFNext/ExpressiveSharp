namespace ExpressiveSharp.EntityFrameworkCore;

/// <summary>
/// Builder for configuring ExpressiveSharp EF Core integration.
/// Passed to the <c>UseExpressives(options => ...)</c> callback to register plugins.
/// </summary>
public sealed class ExpressiveOptionsBuilder
{
    internal List<IExpressivePlugin> Plugins { get; } = [];

    /// <summary>
    /// Registers an <see cref="IExpressivePlugin"/> that contributes services and/or
    /// expression tree transformers to the EF Core pipeline.
    /// </summary>
    public ExpressiveOptionsBuilder AddPlugin(IExpressivePlugin plugin)
    {
        Plugins.Add(plugin);
        return this;
    }
}
