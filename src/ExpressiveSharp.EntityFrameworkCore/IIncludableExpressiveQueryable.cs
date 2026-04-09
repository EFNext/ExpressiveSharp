using Microsoft.EntityFrameworkCore.Query;

namespace ExpressiveSharp.EntityFrameworkCore;

/// <summary>
/// Combines <see cref="IIncludableQueryable{TEntity,TProperty}"/> (for <c>ThenInclude</c> chaining)
/// with <see cref="IExpressiveQueryable{T}"/> (for delegate-based LINQ stubs with modern C# syntax).
/// Returned by <c>Include</c> and <c>ThenInclude</c> on <see cref="IExpressiveQueryable{T}"/> sources.
/// </summary>
public interface IIncludableExpressiveQueryable<TEntity, TProperty>
    : IIncludableQueryable<TEntity, TProperty>,
      IExpressiveQueryable<TEntity>
    where TEntity : class
{
}
