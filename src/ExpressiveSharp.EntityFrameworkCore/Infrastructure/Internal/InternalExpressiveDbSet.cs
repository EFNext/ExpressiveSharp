using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.Infrastructure.Internal;

/// <summary>
/// The concrete runtime instance produced by <c>AsExpressiveDbSet()</c>. It implements
/// <see cref="IAsyncEnumerable{T}"/> — satisfied by the inherited
/// <see cref="ExpressiveDbSet{TEntity}.GetAsyncEnumerator"/> — so EF Core's streaming async
/// terminals (<c>ToListAsync</c>/<c>ToArrayAsync</c>/...), which runtime-cast the source to
/// <see cref="IAsyncEnumerable{T}"/>, work directly on the set.
/// <para>
/// The interface lives here rather than on the public <see cref="ExpressiveDbSet{TEntity}"/> so
/// callers never hold a static type that is both <see cref="IQueryable{T}"/> and
/// <see cref="IAsyncEnumerable{T}"/> — which on .NET 10 makes those terminals ambiguous between
/// <c>System.Linq.AsyncEnumerable</c> and EF Core's extensions. This mirrors EF Core's own
/// public <c>DbSet&lt;T&gt;</c> / internal <c>InternalDbSet&lt;T&gt;</c> split.
/// </para>
/// </summary>
internal sealed class InternalExpressiveDbSet<TEntity> : ExpressiveDbSet<TEntity>, IAsyncEnumerable<TEntity>
    where TEntity : class
{
    public InternalExpressiveDbSet(DbSet<TEntity> inner) : base(inner)
    {
    }
}
