using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressiveSharp
{
    public static class ExpressiveQueryableExtensions
    {
        /// <summary>
        /// Wraps an <see cref="IQueryable{T}"/> in an <see cref="IExpressiveQueryable{T}"/> so that
        /// delegate-based LINQ overloads supporting modern C# syntax become available.
        /// </summary>
        /// <param name="options">
        /// Read by the source generator at compile time; ignored at runtime.
        /// </param>
        public static IExpressiveQueryable<T> AsExpressive<T>(
            this IQueryable<T> source,
            ExpressionRewriteOptions? options = null)
            // If the source is already an IExpressiveQueryable<T> (e.g. a provider-specific
            // wrapper like ExpressiveMongoQueryable<T>), return it unchanged so any
            // additional interfaces it implements (IAsyncCursorSource<T>, IAsyncEnumerable<T>,
            // etc.) remain observable through the returned reference.
            => source as IExpressiveQueryable<T>
               ?? new ExpressiveQueryableWrapper<T>(source);

        /// <summary>
        /// Presents a queryable as <see cref="IOrderedQueryable{T}"/> for the generated
        /// <c>ThenBy</c>/<c>ThenByDescending</c> interceptors. When the underlying expression's
        /// static type is not ordered (e.g. an EF Core <c>Include</c> call following
        /// <c>OrderBy</c>), the expression is wrapped in a cast node so
        /// <see cref="Queryable.ThenBy{TSource,TKey}"/> can compose a valid tree.
        /// </summary>
        public static IOrderedQueryable<T> AsOrdered<T>(this IQueryable<T> source)
            => source is IOrderedQueryable<T> ordered
               && typeof(IOrderedQueryable<T>).IsAssignableFrom(source.Expression.Type)
                ? ordered
                : new OrderedQueryableAdapter<T>(source);

        private sealed class OrderedQueryableAdapter<T> : IOrderedQueryable<T>
        {
            private readonly IQueryable<T> _source;

            public OrderedQueryableAdapter(IQueryable<T> source)
            {
                _source = source;
                Expression = typeof(IOrderedQueryable<T>).IsAssignableFrom(source.Expression.Type)
                    ? source.Expression
                    : Expression.Convert(source.Expression, typeof(IOrderedQueryable<T>));
            }

            public Expression Expression { get; }
            public Type ElementType => _source.ElementType;
            public IQueryProvider Provider => _source.Provider;
            public IEnumerator<T> GetEnumerator() => _source.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_source).GetEnumerator();
        }
    }
}
