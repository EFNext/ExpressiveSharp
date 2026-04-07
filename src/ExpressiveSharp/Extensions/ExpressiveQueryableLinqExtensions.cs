using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace ExpressiveSharp.Extensions
{
    /// <summary>
    /// Delegate-based LINQ overloads on <see cref="IExpressiveQueryable{T}"/> that shadow the
    /// <see cref="Queryable"/> extension methods. These stubs are intercepted at compile time by
    /// the ExpressiveSharp source generator, which rewrites the inline lambda body
    /// into an expression tree and routes the call to the corresponding <see cref="IQueryable{T}"/>
    /// operator. Do not call these methods directly.
    /// </summary>
    /// <remarks>
    /// These methods are hidden from IntelliSense. The C# compiler resolves them in preference to the
    /// <see cref="Queryable"/> equivalents because <see cref="IExpressiveQueryable{T}"/> is a more specific
    /// type than <see cref="IQueryable{T}"/>, which causes the delegate-accepting overload to win,
    /// allowing modern C# syntax (null-conditional operators, pattern matching, etc.) to compile.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressiveQueryableLinqExtensions
    {
        private const string InterceptedMessage =
            "This method must be intercepted by the ExpressiveSharp source generator. " +
            "Ensure the generator package is installed and the InterceptorsNamespaces MSBuild property is configured.";

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> Where<T>(
            this IExpressiveQueryable<T> source,
            Func<T, bool> predicate)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<TResult> Select<T, TResult>(
            this IExpressiveQueryable<T> source,
            Func<T, TResult> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<TResult> Select<T, TResult>(
            this IExpressiveQueryable<T> source,
            Func<T, int, TResult> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<TResult> SelectMany<T, TResult>(
            this IExpressiveQueryable<T> source,
            Func<T, IEnumerable<TResult>> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<TResult> SelectMany<T, TCollection, TResult>(
            this IExpressiveQueryable<T> source,
            Func<T, IEnumerable<TCollection>> collectionSelector,
            Func<T, TCollection, TResult> resultSelector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> OrderBy<T, TKey>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> OrderByDescending<T, TKey>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> ThenBy<T, TKey>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> ThenByDescending<T, TKey>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<IGrouping<TKey, T>> GroupBy<T, TKey>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector)
            => throw new UnreachableException(InterceptedMessage);

        // ── Partitioning ─────────────────────────────────────────────────────

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> TakeWhile<T>(
            this IExpressiveQueryable<T> source,
            Func<T, bool> predicate)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> SkipWhile<T>(
            this IExpressiveQueryable<T> source,
            Func<T, bool> predicate)
            => throw new UnreachableException(InterceptedMessage);

        // ── Set operations with key selector ─────────────────────────────────

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> DistinctBy<T, TKey>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector)
            => throw new UnreachableException(InterceptedMessage);

#if NET9_0_OR_GREATER
        // ── .NET 9+ methods ──────────────────────────────────────────────────

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<KeyValuePair<TKey, int>> CountBy<T, TKey>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector)
            => throw new UnreachableException(InterceptedMessage);
#endif

        // ── Predicate methods (scalar-returning) ─────────────────────────────

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool Any<T>(
            this IExpressiveQueryable<T> source,
            Func<T, bool> predicate)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool All<T>(
            this IExpressiveQueryable<T> source,
            Func<T, bool> predicate)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static int Count<T>(
            this IExpressiveQueryable<T> source,
            Func<T, bool> predicate)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static long LongCount<T>(
            this IExpressiveQueryable<T> source,
            Func<T, bool> predicate)
            => throw new UnreachableException(InterceptedMessage);

        // ── Element methods (scalar-returning) ───────────────────────────────

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T First<T>(
            this IExpressiveQueryable<T> source,
            Func<T, bool> predicate)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T? FirstOrDefault<T>(
            this IExpressiveQueryable<T> source,
            Func<T, bool> predicate)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T Last<T>(
            this IExpressiveQueryable<T> source,
            Func<T, bool> predicate)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T? LastOrDefault<T>(
            this IExpressiveQueryable<T> source,
            Func<T, bool> predicate)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T Single<T>(
            this IExpressiveQueryable<T> source,
            Func<T, bool> predicate)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T? SingleOrDefault<T>(
            this IExpressiveQueryable<T> source,
            Func<T, bool> predicate)
            => throw new UnreachableException(InterceptedMessage);

        // ── Aggregation: Sum ─────────────────────────────────────────────────

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static int Sum<T>(
            this IExpressiveQueryable<T> source,
            Func<T, int> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static int? Sum<T>(
            this IExpressiveQueryable<T> source,
            Func<T, int?> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static long Sum<T>(
            this IExpressiveQueryable<T> source,
            Func<T, long> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static long? Sum<T>(
            this IExpressiveQueryable<T> source,
            Func<T, long?> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static float Sum<T>(
            this IExpressiveQueryable<T> source,
            Func<T, float> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static float? Sum<T>(
            this IExpressiveQueryable<T> source,
            Func<T, float?> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static double Sum<T>(
            this IExpressiveQueryable<T> source,
            Func<T, double> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static double? Sum<T>(
            this IExpressiveQueryable<T> source,
            Func<T, double?> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static decimal Sum<T>(
            this IExpressiveQueryable<T> source,
            Func<T, decimal> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static decimal? Sum<T>(
            this IExpressiveQueryable<T> source,
            Func<T, decimal?> selector)
            => throw new UnreachableException(InterceptedMessage);

        // ── Aggregation: Average ─────────────────────────────────────────────

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static double Average<T>(
            this IExpressiveQueryable<T> source,
            Func<T, int> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static double? Average<T>(
            this IExpressiveQueryable<T> source,
            Func<T, int?> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static double Average<T>(
            this IExpressiveQueryable<T> source,
            Func<T, long> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static double? Average<T>(
            this IExpressiveQueryable<T> source,
            Func<T, long?> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static float Average<T>(
            this IExpressiveQueryable<T> source,
            Func<T, float> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static float? Average<T>(
            this IExpressiveQueryable<T> source,
            Func<T, float?> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static double Average<T>(
            this IExpressiveQueryable<T> source,
            Func<T, double> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static double? Average<T>(
            this IExpressiveQueryable<T> source,
            Func<T, double?> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static decimal Average<T>(
            this IExpressiveQueryable<T> source,
            Func<T, decimal> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static decimal? Average<T>(
            this IExpressiveQueryable<T> source,
            Func<T, decimal?> selector)
            => throw new UnreachableException(InterceptedMessage);

        // ── Aggregation: Min / Max ───────────────────────────────────────────

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TResult Min<T, TResult>(
            this IExpressiveQueryable<T> source,
            Func<T, TResult> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TResult Max<T, TResult>(
            this IExpressiveQueryable<T> source,
            Func<T, TResult> selector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T? MinBy<T, TKey>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T? MaxBy<T, TKey>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector)
            => throw new UnreachableException(InterceptedMessage);

        // ── Passthrough chain-continuity stubs (runtime, not intercepted) ────

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> Take<T>(
            this IExpressiveQueryable<T> source,
            int count)
            => Queryable.Take(source, count).AsExpressive();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> Skip<T>(
            this IExpressiveQueryable<T> source,
            int count)
            => Queryable.Skip(source, count).AsExpressive();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> Distinct<T>(
            this IExpressiveQueryable<T> source)
            => Queryable.Distinct(source).AsExpressive();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> Reverse<T>(
            this IExpressiveQueryable<T> source)
            => Queryable.Reverse(source).AsExpressive();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> Concat<T>(
            this IExpressiveQueryable<T> source,
            IEnumerable<T> other)
            => Queryable.Concat(source, other).AsExpressive();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> Union<T>(
            this IExpressiveQueryable<T> source,
            IEnumerable<T> other)
            => Queryable.Union(source, other).AsExpressive();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> Intersect<T>(
            this IExpressiveQueryable<T> source,
            IEnumerable<T> other)
            => Queryable.Intersect(source, other).AsExpressive();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> Except<T>(
            this IExpressiveQueryable<T> source,
            IEnumerable<T> other)
            => Queryable.Except(source, other).AsExpressive();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T?> DefaultIfEmpty<T>(
            this IExpressiveQueryable<T> source)
            => Queryable.DefaultIfEmpty(source).AsExpressive();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> DefaultIfEmpty<T>(
            this IExpressiveQueryable<T> source,
            T defaultValue)
            => Queryable.DefaultIfEmpty(source, defaultValue).AsExpressive();

#if NET9_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<(int Index, T Item)> Index<T>(
            this IExpressiveQueryable<T> source)
            => Queryable.Index(source).AsExpressive();
#endif

        // ── Non-lambda-first intercepted methods ─────────────────────────────

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<TResult> Zip<T, TSecond, TResult>(
            this IExpressiveQueryable<T> source,
            IEnumerable<TSecond> second,
            Func<T, TSecond, TResult> resultSelector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<(T First, TSecond Second)> Zip<T, TSecond>(
            this IExpressiveQueryable<T> source,
            IEnumerable<TSecond> second)
            => Queryable.Zip(source, second).AsExpressive();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> ExceptBy<T, TKey>(
            this IExpressiveQueryable<T> source,
            IEnumerable<TKey> second,
            Func<T, TKey> keySelector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> IntersectBy<T, TKey>(
            this IExpressiveQueryable<T> source,
            IEnumerable<TKey> second,
            Func<T, TKey> keySelector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> UnionBy<T, TKey>(
            this IExpressiveQueryable<T> source,
            IEnumerable<TKey> second,
            Func<T, TKey> keySelector)
            => throw new UnreachableException(InterceptedMessage);

        // ── Multi-lambda methods ─────────────────────────────────────────────

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<IGrouping<TKey, TElement>> GroupBy<T, TKey, TElement>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector,
            Func<T, TElement> elementSelector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<TResult> GroupBy<T, TKey, TResult>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector,
            Func<TKey, IEnumerable<T>, TResult> resultSelector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<TResult> GroupBy<T, TKey, TElement, TResult>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector,
            Func<T, TElement> elementSelector,
            Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<TResult> Join<T, TInner, TKey, TResult>(
            this IExpressiveQueryable<T> source,
            IEnumerable<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<TResult> GroupJoin<T, TInner, TKey, TResult>(
            this IExpressiveQueryable<T> source,
            IEnumerable<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, IEnumerable<TInner>, TResult> resultSelector)
            => throw new UnreachableException(InterceptedMessage);

#if NET10_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<TResult> LeftJoin<T, TInner, TKey, TResult>(
            this IExpressiveQueryable<T> source,
            IEnumerable<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner?, TResult> resultSelector)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<TResult> RightJoin<T, TInner, TKey, TResult>(
            this IExpressiveQueryable<T> source,
            IEnumerable<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T?, TInner, TResult> resultSelector)
            => throw new UnreachableException(InterceptedMessage);
#endif

#if NET9_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<KeyValuePair<TKey, TAccumulate>> AggregateBy<T, TKey, TAccumulate>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector,
            TAccumulate seed,
            Func<TAccumulate, T, TAccumulate> func)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<KeyValuePair<TKey, TAccumulate>> AggregateBy<T, TKey, TAccumulate>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector,
            Func<TKey, TAccumulate> seedSelector,
            Func<TAccumulate, T, TAccumulate> func)
            => throw new UnreachableException(InterceptedMessage);
#endif

        // ── IEqualityComparer / IComparer overloads (intercepted) ────────────

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> OrderBy<T, TKey>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector,
            IComparer<TKey>? comparer)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> OrderByDescending<T, TKey>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector,
            IComparer<TKey>? comparer)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> ThenBy<T, TKey>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector,
            IComparer<TKey>? comparer)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> ThenByDescending<T, TKey>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector,
            IComparer<TKey>? comparer)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<IGrouping<TKey, T>> GroupBy<T, TKey>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector,
            IEqualityComparer<TKey>? comparer)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> DistinctBy<T, TKey>(
            this IExpressiveQueryable<T> source,
            Func<T, TKey> keySelector,
            IEqualityComparer<TKey>? comparer)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> ExceptBy<T, TKey>(
            this IExpressiveQueryable<T> source,
            IEnumerable<TKey> second,
            Func<T, TKey> keySelector,
            IEqualityComparer<TKey>? comparer)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> IntersectBy<T, TKey>(
            this IExpressiveQueryable<T> source,
            IEnumerable<TKey> second,
            Func<T, TKey> keySelector,
            IEqualityComparer<TKey>? comparer)
            => throw new UnreachableException(InterceptedMessage);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> UnionBy<T, TKey>(
            this IExpressiveQueryable<T> source,
            IEnumerable<TKey> second,
            Func<T, TKey> keySelector,
            IEqualityComparer<TKey>? comparer)
            => throw new UnreachableException(InterceptedMessage);

        // ── IEqualityComparer passthrough (no lambda, chain continuity) ──────

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> Distinct<T>(
            this IExpressiveQueryable<T> source,
            IEqualityComparer<T>? comparer)
            => Queryable.Distinct(source, comparer).AsExpressive();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> Union<T>(
            this IExpressiveQueryable<T> source,
            IEnumerable<T> other,
            IEqualityComparer<T>? comparer)
            => Queryable.Union(source, other, comparer).AsExpressive();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> Intersect<T>(
            this IExpressiveQueryable<T> source,
            IEnumerable<T> other,
            IEqualityComparer<T>? comparer)
            => Queryable.Intersect(source, other, comparer).AsExpressive();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IExpressiveQueryable<T> Except<T>(
            this IExpressiveQueryable<T> source,
            IEnumerable<T> other,
            IEqualityComparer<T>? comparer)
            => Queryable.Except(source, other, comparer).AsExpressive();
    }
}
