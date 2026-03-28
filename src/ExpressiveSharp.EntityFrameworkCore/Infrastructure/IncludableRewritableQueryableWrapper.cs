using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace ExpressiveSharp.EntityFrameworkCore.Infrastructure;

/// <summary>
/// Wraps an <see cref="IIncludableQueryable{TEntity,TProperty}"/> to also implement
/// <see cref="IRewritableQueryable{T}"/>, preserving chain continuity for delegate-based LINQ stubs.
/// </summary>
internal sealed class IncludableRewritableQueryableWrapper<TEntity, TProperty>
    : IIncludableRewritableQueryable<TEntity, TProperty>
    where TEntity : class
{
    private readonly IIncludableQueryable<TEntity, TProperty> _inner;

    public IncludableRewritableQueryableWrapper(IIncludableQueryable<TEntity, TProperty> inner)
        => _inner = inner;

    Type IQueryable.ElementType => ((IQueryable)_inner).ElementType;
    Expression IQueryable.Expression => ((IQueryable)_inner).Expression;
    IQueryProvider IQueryable.Provider => ((IQueryable)_inner).Provider;

    IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator() => _inner.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_inner).GetEnumerator();
}
