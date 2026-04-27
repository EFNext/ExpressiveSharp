using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using ExpressiveSharp;
using MongoDB.Driver.Linq;

// ReSharper disable once CheckNamespace — intentionally in MongoDB.Driver namespace for discoverability
namespace MongoDB.Driver;

/// <summary>
/// Delegate-based async stubs intercepted at compile time and forwarded to
/// <see cref="MongoQueryable"/> extension methods with expression tree arguments.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ExpressiveQueryableMongoExtensions
{
    private const string InterceptedMessage =
        "This method must be intercepted by the ExpressiveSharp source generator. " +
        "Ensure the generator package is installed and the InterceptorsNamespaces MSBuild property is configured.";

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<bool> AnyAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, bool> predicate,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<int> CountAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, bool> predicate,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<long> LongCountAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, bool> predicate,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<T> FirstAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, bool> predicate,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<T?> FirstOrDefaultAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, bool> predicate,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<T> SingleAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, bool> predicate,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<T?> SingleOrDefaultAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, bool> predicate,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<TResult> MinAsync<T, TResult>(
        this IExpressiveQueryable<T> source,
        Func<T, TResult> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<TResult> MaxAsync<T, TResult>(
        this IExpressiveQueryable<T> source,
        Func<T, TResult> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<int> SumAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, int> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<long> SumAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, long> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double> SumAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, double> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<decimal> SumAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, decimal> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<float> SumAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, float> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<int?> SumAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, int?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<long?> SumAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, long?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double?> SumAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, double?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<decimal?> SumAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, decimal?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<float?> SumAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, float?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double> AverageAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, int> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double> AverageAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, long> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double> AverageAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, double> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<decimal> AverageAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, decimal> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<float> AverageAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, float> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double?> AverageAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, int?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double?> AverageAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, long?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double?> AverageAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, double?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<decimal?> AverageAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, decimal?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<float?> AverageAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, float?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double> StandardDeviationPopulationAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, int> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double?> StandardDeviationPopulationAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, int?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double> StandardDeviationPopulationAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, long> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double?> StandardDeviationPopulationAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, long?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<float> StandardDeviationPopulationAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, float> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<float?> StandardDeviationPopulationAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, float?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double> StandardDeviationPopulationAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, double> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double?> StandardDeviationPopulationAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, double?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<decimal> StandardDeviationPopulationAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, decimal> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<decimal?> StandardDeviationPopulationAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, decimal?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double> StandardDeviationSampleAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, int> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double?> StandardDeviationSampleAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, int?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double> StandardDeviationSampleAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, long> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double?> StandardDeviationSampleAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, long?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<float> StandardDeviationSampleAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, float> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<float?> StandardDeviationSampleAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, float?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double> StandardDeviationSampleAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, double> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<double?> StandardDeviationSampleAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, double?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<decimal> StandardDeviationSampleAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, decimal> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [PolyfillTarget(typeof(MongoQueryable))]
    public static Task<decimal?> StandardDeviationSampleAsync<T>(
        this IExpressiveQueryable<T> source,
        Func<T, decimal?> selector,
        CancellationToken cancellationToken = default)
        => throw new UnreachableException(InterceptedMessage);
}
