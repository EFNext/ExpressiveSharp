using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace ExpressiveSharp.EntityFrameworkCore.Infrastructure;

internal sealed class IncludableExpressiveQueryableWrapper<TEntity, TProperty>
    : IIncludableExpressiveQueryable<TEntity, TProperty>, IAsyncEnumerable<TEntity>
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

    IAsyncEnumerator<TEntity> IAsyncEnumerable<TEntity>.GetAsyncEnumerator(CancellationToken cancellationToken)
    {
        if (_inner is IAsyncEnumerable<TEntity> asyncEnumerable)
            return asyncEnumerable.GetAsyncEnumerator(cancellationToken);

        throw new InvalidOperationException(
            $"The source IQueryable<{typeof(TEntity).Name}> does not implement IAsyncEnumerable<{typeof(TEntity).Name}>. " +
            "Async operations require an async-capable provider such as Entity Framework Core.");
    }
}
