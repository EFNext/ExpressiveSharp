namespace ExpressiveSharp.EntityFrameworkCore;

/// <summary>
/// Builder for configuring ExpressiveSharp EF Core integration.
/// Passed to the <c>UseExpressives(options => ...)</c> callback to register plugins.
/// </summary>
public sealed class ExpressiveOptionsBuilder
{
    internal List<IExpressivePlugin> Plugins { get; } = [];

    internal bool ShouldPreserveThrowExpressions { get; private set; }

    public ExpressiveOptionsBuilder AddPlugin(IExpressivePlugin plugin)
    {
        Plugins.Add(plugin);
        return this;
    }

    /// <summary>
    /// Prevents the <see cref="ExpressiveSharp.Transformers.ReplaceThrowWithDefault"/> transformer from
    /// being applied — <c>Expression.Throw</c> nodes are preserved for the LINQ provider to translate.
    /// </summary>
    public ExpressiveOptionsBuilder PreserveThrowExpressions()
    {
        ShouldPreserveThrowExpressions = true;
        return this;
    }
}
