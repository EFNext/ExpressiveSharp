using Microsoft.EntityFrameworkCore.Query;

namespace ExpressiveSharp.EntityFrameworkCore;

/// <summary>
/// Combines <see cref="IIncludableQueryable{TEntity,TProperty}"/> (for <c>ThenInclude</c> chaining)
/// with <see cref="IExpressiveQueryable{T}"/>. Returned by <c>Include</c>/<c>ThenInclude</c>
/// on expressive sources.
/// </summary>
public interface IIncludableExpressiveQueryable<TEntity, TProperty>
    : IIncludableQueryable<TEntity, TProperty>,
      IExpressiveQueryable<TEntity>
    where TEntity : class
{
}
