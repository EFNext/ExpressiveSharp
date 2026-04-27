using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace ExpressiveSharp.EntityFrameworkCore.Infrastructure;

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
