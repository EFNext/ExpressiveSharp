using System.Linq;

namespace ExpressiveSharp.Extensions
{
    /// <summary>
    /// Entry-point extension methods for the <see cref="IRewritableQueryable{T}"/> query chain.
    /// </summary>
    public static class ExpressionRewriteExtensions
    {
        /// <summary>
        /// Wraps an <see cref="IQueryable{T}"/> in an <see cref="IRewritableQueryable{T}"/> to enable
        /// delegate-based LINQ overloads that support modern C# syntax (null-conditional operators, etc.)
        /// via compile-time source generator interception.
        /// </summary>
        /// <param name="source">The queryable source to wrap.</param>
        /// <param name="options">
        /// Options controlling how inline lambda bodies are rewritten to expression trees.
        /// This value is read by the source generator at compile time; it is ignored at runtime.
        /// </param>
        public static IRewritableQueryable<T> WithExpressionRewrite<T>(
            this IQueryable<T> source,
            ExpressionRewriteOptions? options = null)
            => new RewritableQueryableWrapper<T>(source);
    }
}
