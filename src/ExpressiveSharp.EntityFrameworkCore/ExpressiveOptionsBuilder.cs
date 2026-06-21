namespace ExpressiveSharp.EntityFrameworkCore;

/// <summary>
/// Builder for configuring ExpressiveSharp EF Core integration.
/// Passed to the <c>UseExpressives(options => ...)</c> callback to register plugins.
/// </summary>
public sealed class ExpressiveOptionsBuilder
{
    internal List<IExpressivePlugin> Plugins { get; } = [];

    internal bool ShouldPreserveThrowExpressions { get; private set; }

    internal bool ShouldDisablePolymorphicDispatch { get; private set; }

    public ExpressiveOptionsBuilder AddPlugin(IExpressivePlugin plugin)
    {
        Plugins.Add(plugin);
        return this;
    }

    /// <summary>
    /// Disables runtime polymorphic dispatch of virtual/<c>override</c> <c>[Expressive]</c> members:
    /// they expand using the static (declared) type only, never an `is Derived ? ...` chain. Use for
    /// providers that cannot translate type tests. Per-member <c>[NotExpressive]</c> is independent.
    /// </summary>
    public ExpressiveOptionsBuilder DisablePolymorphicDispatch()
    {
        ShouldDisablePolymorphicDispatch = true;
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
