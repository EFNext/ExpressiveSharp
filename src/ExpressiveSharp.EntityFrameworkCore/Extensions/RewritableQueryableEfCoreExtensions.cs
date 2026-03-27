using System.ComponentModel;
using ExpressiveSharp;
using ExpressiveSharp.Extensions;

// ReSharper disable once CheckNamespace — intentionally in Microsoft.EntityFrameworkCore for discoverability
namespace Microsoft.EntityFrameworkCore;

/// <summary>
/// Passthrough extension methods on <see cref="IRewritableQueryable{T}"/> for EF Core query modifiers.
/// These shadow the <see cref="EntityFrameworkQueryableExtensions"/> methods to maintain the
/// <see cref="IRewritableQueryable{T}"/> chain. They are not intercepted by the source generator —
/// they delegate to the real EF Core method and re-wrap the result.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class RewritableQueryableEfCoreExtensions
{
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

}
