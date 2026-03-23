using ExpressiveSharp.EntityFrameworkCore;

// ReSharper disable once CheckNamespace — intentionally in Microsoft.EntityFrameworkCore for discoverability
namespace Microsoft.EntityFrameworkCore;

public static class DbSetExtensions
{
    /// <summary>
    /// Wraps this <see cref="DbSet{TEntity}"/> as a <see cref="ExpressiveDbSet{TEntity}"/>,
    /// enabling delegate-based LINQ methods with modern C# syntax (e.g., <c>?.</c>, switch expressions)
    /// directly on the set.
    /// </summary>
    /// <example>
    /// <code>
    /// public class MyDbContext : DbContext
    /// {
    ///     public ExpressiveDbSet&lt;Order&gt; Orders => Set&lt;Order&gt;().AsExpressiveDbSet();
    /// }
    /// </code>
    /// </example>
    public static ExpressiveDbSet<TEntity> AsExpressiveDbSet<TEntity>(this DbSet<TEntity> dbSet)
        where TEntity : class
        => new(dbSet);
}
