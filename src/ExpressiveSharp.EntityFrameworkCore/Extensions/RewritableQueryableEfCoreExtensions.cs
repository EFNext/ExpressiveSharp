using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using ExpressiveSharp;
using ExpressiveSharp.EntityFrameworkCore;
using ExpressiveSharp.EntityFrameworkCore.Infrastructure;
using ExpressiveSharp.Extensions;

// ReSharper disable once CheckNamespace — intentionally in Microsoft.EntityFrameworkCore for discoverability
namespace Microsoft.EntityFrameworkCore;

/// <summary>
/// Extension methods on <see cref="IRewritableQueryable{T}"/> for EF Core operations.
/// Passthrough stubs maintain the <see cref="IRewritableQueryable{T}"/> chain.
/// Async lambda stubs are intercepted by the source generator via <see cref="PolyfillTargetAttribute"/>
/// to forward to <see cref="EntityFrameworkQueryableExtensions"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class RewritableQueryableEfCoreExtensions
{
    private const string InterceptedMessage =
        "This method must be intercepted by the ExpressiveSharp source generator. " +
        "Ensure the generator package is installed and InterceptorsPreviewNamespaces is configured.";
    // ── Tracking behavior ────────────────────────────────────────────────

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IRewritableQueryable<TEntity> AsNoTracking<TEntity>(
        this IRewritableQueryable<TEntity> source)
        where TEntity : class
        => EntityFrameworkQueryableExtensions.AsNoTracking(source).WithExpressionRewrite();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IRewritableQueryable<TEntity> AsNoTrackingWithIdentityResolution<TEntity>(
        this IRewritableQueryable<TEntity> source)
        where TEntity : class
        => EntityFrameworkQueryableExtensions.AsNoTrackingWithIdentityResolution(source).WithExpressionRewrite();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IRewritableQueryable<TEntity> AsTracking<TEntity>(
        this IRewritableQueryable<TEntity> source)
        where TEntity : class
        => EntityFrameworkQueryableExtensions.AsTracking(source).WithExpressionRewrite();

    // ── Query filters ────────────────────────────────────────────────────

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IRewritableQueryable<TEntity> IgnoreAutoIncludes<TEntity>(
        this IRewritableQueryable<TEntity> source)
        where TEntity : class
        => EntityFrameworkQueryableExtensions.IgnoreAutoIncludes(source).WithExpressionRewrite();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IRewritableQueryable<TEntity> IgnoreQueryFilters<TEntity>(
        this IRewritableQueryable<TEntity> source)
        where TEntity : class
        => EntityFrameworkQueryableExtensions.IgnoreQueryFilters(source).WithExpressionRewrite();

    // ── Query tagging ────────────────────────────────────────────────────

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IRewritableQueryable<TEntity> TagWith<TEntity>(
        this IRewritableQueryable<TEntity> source,
        string tag)
        where TEntity : class
        => EntityFrameworkQueryableExtensions.TagWith(source, tag).WithExpressionRewrite();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IRewritableQueryable<TEntity> TagWithCallSite<TEntity>(
        this IRewritableQueryable<TEntity> source,
        [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        where TEntity : class
        => EntityFrameworkQueryableExtensions.TagWithCallSite(source, filePath, lineNumber).WithExpressionRewrite();

    // ── Include / ThenInclude (runtime, not intercepted) ───────────────

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IIncludableRewritableQueryable<TEntity, TProperty> Include<TEntity, TProperty>(
        this IRewritableQueryable<TEntity> source,
        Expression<Func<TEntity, TProperty>> navigationPropertyPath)
        where TEntity : class
        => new IncludableRewritableQueryableWrapper<TEntity, TProperty>(
            EntityFrameworkQueryableExtensions.Include(source, navigationPropertyPath));

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IRewritableQueryable<TEntity> Include<TEntity>(
        this IRewritableQueryable<TEntity> source,
        string navigationPropertyPath)
        where TEntity : class
        => EntityFrameworkQueryableExtensions.Include(source, navigationPropertyPath)
            .WithExpressionRewrite();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IIncludableRewritableQueryable<TEntity, TProperty>
        ThenInclude<TEntity, TPreviousProperty, TProperty>(
        this IIncludableRewritableQueryable<TEntity, TPreviousProperty> source,
        Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
        where TEntity : class
        => new IncludableRewritableQueryableWrapper<TEntity, TProperty>(
            EntityFrameworkQueryableExtensions.ThenInclude(source, navigationPropertyPath));

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IIncludableRewritableQueryable<TEntity, TProperty>
        ThenInclude<TEntity, TPreviousProperty, TProperty>(
        this IIncludableRewritableQueryable<TEntity, IEnumerable<TPreviousProperty>> source,
        Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
        where TEntity : class
        => new IncludableRewritableQueryableWrapper<TEntity, TProperty>(
            EntityFrameworkQueryableExtensions.ThenInclude(source, navigationPropertyPath));

    // ── Async predicate methods (intercepted) ────────────────────────────

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<bool> AnyAsync<TEntity>(
        this IRewritableQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        where TEntity : class
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<bool> AllAsync<TEntity>(
        this IRewritableQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        where TEntity : class
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<int> CountAsync<TEntity>(
        this IRewritableQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        where TEntity : class
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<long> LongCountAsync<TEntity>(
        this IRewritableQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        where TEntity : class
        => throw new UnreachableException(InterceptedMessage);

    // ── Async element methods (intercepted) ──────────────────────────────

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<TEntity> FirstAsync<TEntity>(
        this IRewritableQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        where TEntity : class
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<TEntity?> FirstOrDefaultAsync<TEntity>(
        this IRewritableQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        where TEntity : class
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<TEntity> LastAsync<TEntity>(
        this IRewritableQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        where TEntity : class
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<TEntity?> LastOrDefaultAsync<TEntity>(
        this IRewritableQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        where TEntity : class
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<TEntity> SingleAsync<TEntity>(
        this IRewritableQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        where TEntity : class
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<TEntity?> SingleOrDefaultAsync<TEntity>(
        this IRewritableQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        where TEntity : class
        => throw new UnreachableException(InterceptedMessage);

    // ── Async Sum (intercepted) ──────────────────────────────────────────

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<int> SumAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, int> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<int?> SumAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, int?> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<long> SumAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, long> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<long?> SumAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, long?> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<float> SumAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, float> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<float?> SumAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, float?> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<double> SumAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, double> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<double?> SumAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, double?> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<decimal> SumAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, decimal> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<decimal?> SumAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, decimal?> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    // ── Async Average (intercepted) ──────────────────────────────────────

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<double> AverageAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, int> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<double?> AverageAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, int?> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<double> AverageAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, long> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<double?> AverageAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, long?> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<float> AverageAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, float> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<float?> AverageAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, float?> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<double> AverageAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, double> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<double?> AverageAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, double?> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<decimal> AverageAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, decimal> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<decimal?> AverageAsync<TEntity>(this IRewritableQueryable<TEntity> source, Func<TEntity, decimal?> selector, CancellationToken cancellationToken = default) where TEntity : class => throw new UnreachableException(InterceptedMessage);

    // ── Async Min / Max (intercepted) ────────────────────────────────────

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<TResult> MinAsync<TEntity, TResult>(
        this IRewritableQueryable<TEntity> source,
        Func<TEntity, TResult> selector,
        CancellationToken cancellationToken = default)
        where TEntity : class
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<TResult> MaxAsync<TEntity, TResult>(
        this IRewritableQueryable<TEntity> source,
        Func<TEntity, TResult> selector,
        CancellationToken cancellationToken = default)
        where TEntity : class
        => throw new UnreachableException(InterceptedMessage);
}
