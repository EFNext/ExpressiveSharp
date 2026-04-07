using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace ExpressiveSharp.EntityFrameworkCore.Infrastructure;

/// <summary>
/// Wraps an <see cref="IIncludableQueryable{TEntity,TProperty}"/> to also implement
/// <see cref="IExpressiveQueryable{T}"/>, preserving chain continuity for delegate-based LINQ stubs.
/// </summary>
internal sealed class IncludableExpressiveQueryableWrapper<TEntity, TProperty>
    : IIncludableExpressiveQueryable<TEntity, TProperty>
    where TEntity : class
{
    private readonly IIncludableQueryable<TEntity, TProperty> _inner;

    public IncludableExpressiveQueryableWrapper(IIncludableQueryable<TEntity, TProperty> inner)
        => _inner = inner;

    Type IQueryable.ElementType => ((IQueryable)_inner).ElementType;
    Expression IQueryable.Expression => ((IQueryable)_inner).Expression;
    IQueryProvider IQueryable.Provider => ((IQueryable)_inner).Provider;

    IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator() => _inner.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_inner).GetEnumerator();
}
