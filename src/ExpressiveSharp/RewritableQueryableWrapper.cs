using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressiveSharp
{
    /// <summary>
    /// Internal wrapper that adapts an <see cref="IQueryable{T}"/> to <see cref="IRewritableQueryable{T}"/>.
    /// Created by <see cref="Extensions.ExpressionRewriteExtensions.WithExpressionRewrite{T}"/> and by source-generated interceptors.
    /// </summary>
    /// <summary>
    /// Also implements <see cref="IOrderedQueryable{T}"/> (which adds no members over <see cref="IQueryable{T}"/>)
    /// so that the source-generated <c>ThenBy</c>/<c>ThenByDescending</c> interceptors can cast the wrapper
    /// to <see cref="IOrderedQueryable{T}"/> without a runtime exception.
    /// </summary>
    internal sealed class RewritableQueryableWrapper<T> : IRewritableQueryable<T>, IOrderedQueryable<T>
    {
        private readonly IQueryable<T> _source;

        public RewritableQueryableWrapper(IQueryable<T> source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public Type ElementType => _source.ElementType;
        public Expression Expression => _source.Expression;
        public IQueryProvider Provider => _source.Provider;
        public IEnumerator<T> GetEnumerator() => _source.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_source).GetEnumerator();
    }
}
