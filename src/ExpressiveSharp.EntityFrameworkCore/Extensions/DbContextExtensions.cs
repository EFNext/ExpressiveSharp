using ExpressiveSharp.EntityFrameworkCore;

// ReSharper disable once CheckNamespace — intentionally in Microsoft.EntityFrameworkCore for discoverability
namespace Microsoft.EntityFrameworkCore;

public static class DbContextExtensions
{
    /// <summary>
    /// Returns an <see cref="ExpressiveDbSet{TEntity}"/> for the given entity type,
    /// enabling delegate-based LINQ methods with modern C# syntax (e.g., <c>?.</c>).
    /// </summary>
    public static ExpressiveDbSet<TEntity> ExpressiveSet<TEntity>(this DbContext context)
        where TEntity : class
        => context.Set<TEntity>().AsExpressiveDbSet();
}
