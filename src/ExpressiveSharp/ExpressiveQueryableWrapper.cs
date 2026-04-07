using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressiveSharp
{
    /// <summary>
    /// Internal wrapper that adapts an <see cref="IQueryable{T}"/> to <see cref="IExpressiveQueryable{T}"/>.
    /// Created by <see cref="Extensions.ExpressiveQueryableExtensions.AsExpressive{T}"/> and by source-generated interceptors.
    /// </summary>
    /// <summary>
    /// Also implements <see cref="IOrderedQueryable{T}"/> (which adds no members over <see cref="IQueryable{T}"/>)
    /// so that the source-generated <c>ThenBy</c>/<c>ThenByDescending</c> interceptors can cast the wrapper
    /// to <see cref="IOrderedQueryable{T}"/> without a runtime exception.
    /// </summary>
    internal sealed class ExpressiveQueryableWrapper<T> : IExpressiveQueryable<T>, IOrderedQueryable<T>, IAsyncEnumerable<T>
    {
        private readonly IQueryable<T> _source;

        public ExpressiveQueryableWrapper(IQueryable<T> source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public Type ElementType => _source.ElementType;
        public Expression Expression => _source.Expression;
        public IQueryProvider Provider => _source.Provider;
        public IEnumerator<T> GetEnumerator() => _source.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_source).GetEnumerator();

        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
        {
            if (_source is IAsyncEnumerable<T> asyncEnumerable)
                return asyncEnumerable.GetAsyncEnumerator(cancellationToken);

            throw new InvalidOperationException(
                $"The source IQueryable<{typeof(T).Name}> does not implement IAsyncEnumerable<{typeof(T).Name}>. " +
                "Async operations require an async-capable provider such as Entity Framework Core.");
        }
    }
}
