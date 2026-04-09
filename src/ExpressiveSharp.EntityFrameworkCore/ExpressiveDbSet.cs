using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ExpressiveSharp.EntityFrameworkCore;

/// <summary>
/// A <see cref="DbSet{TEntity}"/> wrapper that also implements <see cref="IExpressiveQueryable{T}"/>,
/// enabling delegate-based LINQ methods with modern C# syntax (e.g., <c>?.</c>) directly on the set.
/// </summary>
/// <example>
/// <code>
/// public class MyDbContext : DbContext
/// {
///     public ExpressiveDbSet&lt;Order&gt; Orders => Set&lt;Order&gt;().AsExpressiveDbSet();
/// }
///
/// // Now you can use ?. directly:
/// ctx.Orders.Where(o => o.Customer?.Name == "Alice")
/// </code>
/// </example>
public class ExpressiveDbSet<TEntity> : DbSet<TEntity>, IExpressiveQueryable<TEntity>
    where TEntity : class
{
    private readonly DbSet<TEntity> _inner;
    private readonly IQueryable<TEntity> _queryable;

    public ExpressiveDbSet(DbSet<TEntity> inner)
    {
        _inner = inner;
        _queryable = inner;
    }

    // ── IQueryable ───────────────────────────────────────────────────────

    Type IQueryable.ElementType => _queryable.ElementType;
    Expression IQueryable.Expression => _queryable.Expression;
    IQueryProvider IQueryable.Provider => _queryable.Provider;

    // ── IEnumerable ──────────────────────────────────────────────────────

    IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator() => _queryable.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_queryable).GetEnumerator();

    // ── DbSet<TEntity> virtual members ───────────────────────────────────

    public override IEntityType EntityType => _inner.EntityType;
    public override LocalView<TEntity> Local => _inner.Local;

    public override IAsyncEnumerable<TEntity> AsAsyncEnumerable() => _inner.AsAsyncEnumerable();
    public override IQueryable<TEntity> AsQueryable() => _inner.AsQueryable();

    public override TEntity? Find(params object?[]? keyValues) => _inner.Find(keyValues);
    public override ValueTask<TEntity?> FindAsync(params object?[]? keyValues) => _inner.FindAsync(keyValues);
    public override ValueTask<TEntity?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken)
        => _inner.FindAsync(keyValues, cancellationToken);

    public override EntityEntry<TEntity> Add(TEntity entity) => _inner.Add(entity);
    public override ValueTask<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => _inner.AddAsync(entity, cancellationToken);
    public override EntityEntry<TEntity> Attach(TEntity entity) => _inner.Attach(entity);
    public override EntityEntry<TEntity> Remove(TEntity entity) => _inner.Remove(entity);
    public override EntityEntry<TEntity> Update(TEntity entity) => _inner.Update(entity);

    public override void AddRange(params TEntity[] entities) => _inner.AddRange(entities);
    public override Task AddRangeAsync(params TEntity[] entities) => _inner.AddRangeAsync(entities);
    public override void AttachRange(params TEntity[] entities) => _inner.AttachRange(entities);
    public override void RemoveRange(params TEntity[] entities) => _inner.RemoveRange(entities);
    public override void UpdateRange(params TEntity[] entities) => _inner.UpdateRange(entities);

    public override void AddRange(IEnumerable<TEntity> entities) => _inner.AddRange(entities);
    public override Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => _inner.AddRangeAsync(entities, cancellationToken);
    public override void AttachRange(IEnumerable<TEntity> entities) => _inner.AttachRange(entities);
    public override void RemoveRange(IEnumerable<TEntity> entities) => _inner.RemoveRange(entities);
    public override void UpdateRange(IEnumerable<TEntity> entities) => _inner.UpdateRange(entities);

    public override EntityEntry<TEntity> Entry(TEntity entity) => _inner.Entry(entity);

    public override IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => _inner.GetAsyncEnumerator(cancellationToken);
}
