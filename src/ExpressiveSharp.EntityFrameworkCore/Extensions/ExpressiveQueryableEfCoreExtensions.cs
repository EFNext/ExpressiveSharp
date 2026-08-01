using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using ExpressiveSharp;
using ExpressiveSharp.EntityFrameworkCore;
using ExpressiveSharp.EntityFrameworkCore.Infrastructure;


// ReSharper disable once CheckNamespace — intentionally in Microsoft.EntityFrameworkCore for discoverability
namespace Microsoft.EntityFrameworkCore;

/// <summary>
/// Extension methods on <see cref="IExpressiveQueryable{T}"/> for EF Core operations.
/// Async lambda stubs are intercepted by the source generator via <see cref="PolyfillTargetAttribute"/>
/// and forwarded to <see cref="EntityFrameworkQueryableExtensions"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ExpressiveQueryableEfCoreExtensions
{
    private const string InterceptedMessage =
        "This method must be intercepted by the ExpressiveSharp source generator. " +
        "Ensure the generator package is installed and the InterceptorsNamespaces MSBuild property is configured.";

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IExpressiveQueryable<TEntity> AsNoTracking<TEntity>(
        this IExpressiveQueryable<TEntity> source)
        where TEntity : class
        => EntityFrameworkQueryableExtensions.AsNoTracking(source).AsExpressive();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IExpressiveQueryable<TEntity> AsNoTrackingWithIdentityResolution<TEntity>(
        this IExpressiveQueryable<TEntity> source)
        where TEntity : class
        => EntityFrameworkQueryableExtensions.AsNoTrackingWithIdentityResolution(source).AsExpressive();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IExpressiveQueryable<TEntity> AsTracking<TEntity>(
        this IExpressiveQueryable<TEntity> source)
        where TEntity : class
        => EntityFrameworkQueryableExtensions.AsTracking(source).AsExpressive();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IExpressiveQueryable<TEntity> IgnoreAutoIncludes<TEntity>(
        this IExpressiveQueryable<TEntity> source)
        where TEntity : class
        => EntityFrameworkQueryableExtensions.IgnoreAutoIncludes(source).AsExpressive();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IExpressiveQueryable<TEntity> IgnoreQueryFilters<TEntity>(
        this IExpressiveQueryable<TEntity> source)
        where TEntity : class
        => EntityFrameworkQueryableExtensions.IgnoreQueryFilters(source).AsExpressive();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IExpressiveQueryable<TEntity> TagWith<TEntity>(
        this IExpressiveQueryable<TEntity> source,
        string tag)
        => EntityFrameworkQueryableExtensions.TagWith(source, tag).AsExpressive();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IExpressiveQueryable<TEntity> TagWithCallSite<TEntity>(
        this IExpressiveQueryable<TEntity> source,
        [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        => EntityFrameworkQueryableExtensions.TagWithCallSite(source, filePath, lineNumber).AsExpressive();

    // Include / ThenInclude run at runtime, not intercepted.

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IIncludableExpressiveQueryable<TEntity, TProperty> Include<TEntity, TProperty>(
        this IExpressiveQueryable<TEntity> source,
        Expression<Func<TEntity, TProperty>> navigationPropertyPath)
        where TEntity : class
        => new IncludableExpressiveQueryableWrapper<TEntity, TProperty>(
            EntityFrameworkQueryableExtensions.Include(source, navigationPropertyPath));

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IExpressiveQueryable<TEntity> Include<TEntity>(
        this IExpressiveQueryable<TEntity> source,
        string navigationPropertyPath)
        where TEntity : class
        => EntityFrameworkQueryableExtensions.Include(source, navigationPropertyPath)
            .AsExpressive();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IIncludableExpressiveQueryable<TEntity, TProperty>
        ThenInclude<TEntity, TPreviousProperty, TProperty>(
        this IIncludableExpressiveQueryable<TEntity, TPreviousProperty> source,
        Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
        where TEntity : class
        => new IncludableExpressiveQueryableWrapper<TEntity, TProperty>(
            EntityFrameworkQueryableExtensions.ThenInclude(source, navigationPropertyPath));

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IIncludableExpressiveQueryable<TEntity, TProperty>
        ThenInclude<TEntity, TPreviousProperty, TProperty>(
        this IIncludableExpressiveQueryable<TEntity, IEnumerable<TPreviousProperty>> source,
        Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
        where TEntity : class
        => new IncludableExpressiveQueryableWrapper<TEntity, TProperty>(
            EntityFrameworkQueryableExtensions.ThenInclude(source, navigationPropertyPath));

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<bool> AnyAsync<TEntity>(
        this IExpressiveQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<bool> AllAsync<TEntity>(
        this IExpressiveQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<int> CountAsync<TEntity>(
        this IExpressiveQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<long> LongCountAsync<TEntity>(
        this IExpressiveQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<TEntity> FirstAsync<TEntity>(
        this IExpressiveQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<TEntity?> FirstOrDefaultAsync<TEntity>(
        this IExpressiveQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<TEntity> LastAsync<TEntity>(
        this IExpressiveQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<TEntity?> LastOrDefaultAsync<TEntity>(
        this IExpressiveQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<TEntity> SingleAsync<TEntity>(
        this IExpressiveQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<TEntity?> SingleOrDefaultAsync<TEntity>(
        this IExpressiveQueryable<TEntity> source,
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<int> SumAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, int> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<int?> SumAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, int?> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<long> SumAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, long> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<long?> SumAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, long?> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<float> SumAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, float> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<float?> SumAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, float?> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<double> SumAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, double> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<double?> SumAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, double?> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<decimal> SumAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, decimal> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<decimal?> SumAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, decimal?> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<double> AverageAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, int> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<double?> AverageAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, int?> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<double> AverageAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, long> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<double?> AverageAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, long?> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<float> AverageAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, float> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<float?> AverageAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, float?> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<double> AverageAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, double> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<double?> AverageAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, double?> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<decimal> AverageAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, decimal> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<decimal?> AverageAsync<TEntity>(this IExpressiveQueryable<TEntity> source, Func<TEntity, decimal?> selector, CancellationToken cancellationToken = default) => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<TResult> MinAsync<TEntity, TResult>(
        this IExpressiveQueryable<TEntity> source,
        Func<TEntity, TResult> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<TResult> MaxAsync<TEntity, TResult>(
        this IExpressiveQueryable<TEntity> source,
        Func<TEntity, TResult> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);
}
