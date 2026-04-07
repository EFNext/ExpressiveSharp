using System.Linq;

namespace ExpressiveSharp
{
    /// <summary>
    /// Entry-point extension methods for the <see cref="IExpressiveQueryable{T}"/> query chain.
    /// </summary>
    public static class ExpressiveQueryableExtensions
    {
        /// <summary>
        /// Wraps an <see cref="IQueryable{T}"/> in an <see cref="IExpressiveQueryable{T}"/> to enable
        /// delegate-based LINQ overloads that support modern C# syntax (null-conditional operators, etc.)
        /// via compile-time source generator interception.
        /// </summary>
        /// <param name="source">The queryable source to wrap.</param>
        /// <param name="options">
        /// Options controlling how inline lambda bodies are rewritten to expression trees.
        /// This value is read by the source generator at compile time; it is ignored at runtime.
        /// </param>
        public static IExpressiveQueryable<T> AsExpressive<T>(
            this IQueryable<T> source,
            ExpressionRewriteOptions? options = null)
            // If the source is already an IExpressiveQueryable<T> (e.g. a provider-specific
            // wrapper like ExpressiveMongoQueryable<T>), return it unchanged so that any
            // additional interfaces it implements (IAsyncCursorSource<T>, IAsyncEnumerable<T>,
            // etc.) remain observable through the returned reference.
            => source as IExpressiveQueryable<T>
               ?? new ExpressiveQueryableWrapper<T>(source);
    }
}
