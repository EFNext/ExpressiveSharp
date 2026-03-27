using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace ExpressiveSharp.Extensions
{
    /// <summary>
    /// Delegate-based LINQ overloads on <see cref="IRewritableQueryable{T}"/> that shadow the
    /// <see cref="Queryable"/> extension methods. These stubs are intercepted at compile time by
    /// the ExpressiveSharp source generator, which rewrites the inline lambda body
    /// into an expression tree and routes the call to the corresponding <see cref="IQueryable{T}"/>
    /// operator. Do not call these methods directly.
    /// </summary>
    /// <remarks>
    /// These methods are hidden from IntelliSense. The C# compiler resolves them in preference to the
    /// <see cref="Queryable"/> equivalents because <see cref="IRewritableQueryable{T}"/> is a more specific
    /// type than <see cref="IQueryable{T}"/>, which causes the delegate-accepting overload to win,
    /// allowing modern C# syntax (null-conditional operators, pattern matching, etc.) to compile.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RewritableQueryableLinqExtensions
    {
        private const string InterceptedMessage =
            "This method must be intercepted by the ExpressiveSharp source generator. " +
            "Ensure the generator package is installed and InterceptorsPreviewNamespaces is configured.";

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IRewritableQueryable<T> Where<T>(
            this IRewritableQueryable<T> source,
            Func<T, bool> predicate)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IRewritableQueryable<TResult> Select<T, TResult>(
            this IRewritableQueryable<T> source,
            Func<T, TResult> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IRewritableQueryable<TResult> SelectMany<T, TResult>(
            this IRewritableQueryable<T> source,
            Func<T, IEnumerable<TResult>> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IRewritableQueryable<TResult> SelectMany<T, TCollection, TResult>(
            this IRewritableQueryable<T> source,
            Func<T, IEnumerable<TCollection>> collectionSelector,
            Func<T, TCollection, TResult> resultSelector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IRewritableQueryable<T> OrderBy<T, TKey>(
            this IRewritableQueryable<T> source,
            Func<T, TKey> keySelector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IRewritableQueryable<T> OrderByDescending<T, TKey>(
            this IRewritableQueryable<T> source,
            Func<T, TKey> keySelector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IRewritableQueryable<T> ThenBy<T, TKey>(
            this IRewritableQueryable<T> source,
            Func<T, TKey> keySelector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IRewritableQueryable<T> ThenByDescending<T, TKey>(
            this IRewritableQueryable<T> source,
            Func<T, TKey> keySelector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IRewritableQueryable<IGrouping<TKey, T>> GroupBy<T, TKey>(
            this IRewritableQueryable<T> source,
            Func<T, TKey> keySelector)
            => throw new UnreachableException(InterceptedMessage);
    }
}
