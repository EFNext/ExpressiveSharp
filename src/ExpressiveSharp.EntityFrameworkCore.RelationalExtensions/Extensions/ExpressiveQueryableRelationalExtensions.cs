#if !NET10_0_OR_GREATER
using System.ComponentModel;
using System.Diagnostics;
using ExpressiveSharp;
using Microsoft.EntityFrameworkCore.Query;

// ReSharper disable once CheckNamespace — intentionally in Microsoft.EntityFrameworkCore for discoverability
namespace Microsoft.EntityFrameworkCore;

/// <summary>
/// Extension methods on <see cref="IRewritableQueryable{T}"/> for EF Core bulk update operations.
/// These stubs are intercepted by the ExpressiveSharp source generator via <see cref="PolyfillTargetAttribute"/>
/// to forward to the appropriate EF Core ExecuteUpdate method.
/// </summary>
/// <remarks>
/// Only available on EF Core 8/9. In EF Core 10+, <c>ExecuteUpdate</c> uses <c>Action&lt;UpdateSettersBuilder&lt;T&gt;&gt;</c>
/// which natively supports modern C# syntax in the outer lambda. For inner <c>SetProperty</c> value expressions,
/// use <c>ExpressionPolyfill.Create()</c> to enable modern C# syntax.
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class RewritableQueryableRelationalExtensions
{
    private const string InterceptedMessage =
        "This method must be intercepted by the ExpressiveSharp source generator. " +
        "Ensure the generator package is installed and the InterceptorsNamespaces MSBuild property is configured.";

    // ── Bulk update methods (intercepted) ────────────────────────────────
    // EF Core 8: ExecuteUpdate lives on RelationalQueryableExtensions (Relational package)
    // EF Core 9: ExecuteUpdate moved to EntityFrameworkQueryableExtensions (Core package)

#if NET9_0
    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
#else
    [PolyfillTarget(typeof(RelationalQueryableExtensions))]
#endif
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static int ExecuteUpdate<TSource>(
        this IRewritableQueryable<TSource> source,
        Func<SetPropertyCalls<TSource>, SetPropertyCalls<TSource>> setPropertyCalls)
        where TSource : class
        => throw new UnreachableException(InterceptedMessage);

#if NET9_0
    [PolyfillTarget(typeof(EntityFrameworkQueryableExtensions))]
#else
    [PolyfillTarget(typeof(RelationalQueryableExtensions))]
#endif
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<int> ExecuteUpdateAsync<TSource>(
        this IRewritableQueryable<TSource> source,
        Func<SetPropertyCalls<TSource>, SetPropertyCalls<TSource>> setPropertyCalls,
        CancellationToken cancellationToken = default)
        where TSource : class
        => throw new UnreachableException(InterceptedMessage);
}
#endif
