using Microsoft.EntityFrameworkCore.Query;

namespace ExpressiveSharp.EntityFrameworkCore;

/// <summary>
/// Combines <see cref="IIncludableQueryable{TEntity,TProperty}"/> (for <c>ThenInclude</c> chaining)
/// with <see cref="IRewritableQueryable{T}"/> (for delegate-based LINQ stubs with modern C# syntax).
/// Returned by <c>Include</c> and <c>ThenInclude</c> on <see cref="IRewritableQueryable{T}"/> sources.
/// </summary>
public interface IIncludableRewritableQueryable<TEntity, TProperty>
    : IIncludableQueryable<TEntity, TProperty>,
      IRewritableQueryable<TEntity>
    where TEntity : class
{
}
