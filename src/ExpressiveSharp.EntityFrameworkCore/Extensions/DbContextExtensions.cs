using ExpressiveSharp.EntityFrameworkCore;

// ReSharper disable once CheckNamespace — intentionally in Microsoft.EntityFrameworkCore for discoverability
namespace Microsoft.EntityFrameworkCore;

public static class DbContextExtensions
{
    public static ExpressiveDbSet<TEntity> ExpressiveSet<TEntity>(this DbContext context)
        where TEntity : class
        => context.Set<TEntity>().AsExpressiveDbSet();
}
