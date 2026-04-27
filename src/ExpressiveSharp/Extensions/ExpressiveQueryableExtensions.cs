using System.Linq;

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
    }
}
