using ExpressiveSharp.EntityFrameworkCore;
using ExpressiveSharp.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

// ReSharper disable once CheckNamespace — intentionally in Microsoft.EntityFrameworkCore for discoverability
namespace Microsoft.EntityFrameworkCore;

public static class DbContextOptionsExtensions
{
    /// <summary>
    /// Enables ExpressiveSharp integration with EF Core: expands <c>[Expressive]</c> members in
    /// queries and global filters, marks them unmapped, and applies EF-compatible transformers.
    /// </summary>
    public static DbContextOptionsBuilder UseExpressives(this DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseExpressives(_ => { });

    /// <inheritdoc cref="UseExpressives(DbContextOptionsBuilder)"/>
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

    public static DbContextOptionsBuilder<TContext> UseExpressives<TContext>(
        this DbContextOptionsBuilder<TContext> optionsBuilder)
        where TContext : DbContext
    {
        ((DbContextOptionsBuilder)optionsBuilder).UseExpressives();
        return optionsBuilder;
    }

    public static DbContextOptionsBuilder<TContext> UseExpressives<TContext>(
        this DbContextOptionsBuilder<TContext> optionsBuilder,
        Action<ExpressiveOptionsBuilder> configure)
        where TContext : DbContext
    {
        ((DbContextOptionsBuilder)optionsBuilder).UseExpressives(configure);
        return optionsBuilder;
    }
}
