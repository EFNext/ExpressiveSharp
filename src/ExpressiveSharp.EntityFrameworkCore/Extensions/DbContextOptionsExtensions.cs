using ExpressiveSharp.EntityFrameworkCore;
using ExpressiveSharp.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

// ReSharper disable once CheckNamespace — intentionally in Microsoft.EntityFrameworkCore for discoverability
namespace Microsoft.EntityFrameworkCore;

public static class DbContextOptionsExtensions
{
    /// <summary>
    /// Enables ExpressiveSharp integration with EF Core. This will:
    /// <list type="bullet">
    /// <item>Automatically expand <c>[Expressive]</c> member references in LINQ queries</item>
    /// <item>Mark <c>[Expressive]</c> properties as unmapped in the EF model</item>
    /// <item>Expand <c>[Expressive]</c> calls in global query filters</item>
    /// <item>Apply EF Core-compatible transformers (RemoveNullConditionalPatterns, FlattenBlockExpressions)</item>
    /// </list>
    /// </summary>
    public static DbContextOptionsBuilder UseExpressives(this DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseExpressives(_ => { });

    /// <summary>
    /// Enables ExpressiveSharp integration with EF Core with additional plugin configuration.
    /// </summary>
    /// <param name="optionsBuilder">The EF Core options builder.</param>
    /// <param name="configure">A callback to configure plugins (e.g., <c>options.UseRelationalExtensions()</c>).</param>
    public static DbContextOptionsBuilder UseExpressives(
        this DbContextOptionsBuilder optionsBuilder,
        Action<ExpressiveOptionsBuilder> configure)
    {
        var builder = new ExpressiveOptionsBuilder();
        configure(builder);

        var extension = new ExpressiveOptionsExtension(builder.Plugins, builder.ShouldPreserveThrowExpressions);

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        return optionsBuilder;
    }

    /// <summary>
    /// Enables ExpressiveSharp integration with EF Core (generic overload).
    /// </summary>
    public static DbContextOptionsBuilder<TContext> UseExpressives<TContext>(
        this DbContextOptionsBuilder<TContext> optionsBuilder)
        where TContext : DbContext
    {
        ((DbContextOptionsBuilder)optionsBuilder).UseExpressives();
        return optionsBuilder;
    }

    /// <summary>
    /// Enables ExpressiveSharp integration with EF Core with additional plugin configuration (generic overload).
    /// </summary>
    public static DbContextOptionsBuilder<TContext> UseExpressives<TContext>(
        this DbContextOptionsBuilder<TContext> optionsBuilder,
        Action<ExpressiveOptionsBuilder> configure)
        where TContext : DbContext
    {
        ((DbContextOptionsBuilder)optionsBuilder).UseExpressives(configure);
        return optionsBuilder;
    }
}
