using ExpressiveSharp.EntityFrameworkCore;
using ExpressiveSharp.EntityFrameworkCore.Infrastructure.Internal;

// ReSharper disable once CheckNamespace — intentionally in Microsoft.EntityFrameworkCore for discoverability
namespace Microsoft.EntityFrameworkCore;

public static class DbSetExtensions
{
    public static ExpressiveDbSet<TEntity> AsExpressiveDbSet<TEntity>(this DbSet<TEntity> dbSet)
        where TEntity : class
        => new InternalExpressiveDbSet<TEntity>(dbSet);
}
