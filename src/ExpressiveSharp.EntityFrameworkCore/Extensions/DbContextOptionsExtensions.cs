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
    {
        var extension = optionsBuilder.Options.FindExtension<ExpressiveOptionsExtension>()
            ?? new ExpressiveOptionsExtension();

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
}
