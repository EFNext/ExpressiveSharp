using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressiveSharp
{
    /// <summary>
    /// Adapts an <see cref="IQueryable{T}"/> to <see cref="IExpressiveQueryable{T}"/>. Implements
    /// <see cref="IOrderedExpressiveQueryable{T}"/> unconditionally so it can serve as the fallback
    /// of both <c>AsExpressive</c> overloads; orderedness is enforced by the stub signatures, not
    /// by this type.
    /// </summary>
    internal sealed class ExpressiveQueryableWrapper<T> : IOrderedExpressiveQueryable<T>, IAsyncEnumerable<T>
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
