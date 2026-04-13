using ExpressiveSharp.MongoDB.Infrastructure;
using ExpressiveSharp.Services;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ExpressiveSharp.MongoDB.Extensions;

/// <summary>
/// Entry-point extension methods for wrapping MongoDB queryables and collections
/// with ExpressiveSharp expression expansion.
/// </summary>
public static class MongoExpressiveExtensions
{
    /// <summary>
    /// Wraps an <see cref="IQueryable{T}"/> backed by MongoDB's LINQ provider in an
    /// <see cref="IExpressiveMongoQueryable{T}"/> that automatically expands
    /// <c>[Expressive]</c> members before query execution.
    /// </summary>
    /// <param name="source">The MongoDB queryable source (typically from <c>collection.AsQueryable()</c>).</param>
    /// <param name="options">
    /// Optional <see cref="ExpressiveOptions"/> controlling the transformer pipeline.
    /// When <c>null</c>, <see cref="MongoExpressiveOptions.CreateDefault"/> is used.
    /// </param>
    public static IExpressiveMongoQueryable<T> AsExpressive<T>(
        this IQueryable<T> source,
        ExpressiveOptions? options = null)
    {
        // Idempotent; cheap on subsequent calls via an Interlocked.Exchange guard.
        // Registering here (rather than in ExpressiveMongoCollection's constructor)
        // guarantees the BSON class-map convention fires on the common
        // `collection.AsExpressive()` entry point too.
        ExpressiveMongoIgnoreConvention.EnsureRegistered();

        var mongoProvider = source.Provider as IMongoQueryProvider
            ?? throw new ArgumentException(
                "The source queryable's Provider must implement IMongoQueryProvider. " +
                "Use collection.AsQueryable() to obtain a MongoDB-backed queryable.",
                nameof(source));

        var effectiveOptions = options ?? MongoExpressiveOptions.CreateDefault();
        var provider = new ExpressiveMongoQueryProvider(mongoProvider, effectiveOptions);
        return new ExpressiveMongoQueryable<T>(source, provider);
    }

    /// <summary>
    /// Creates an <see cref="IExpressiveMongoQueryable{T}"/> directly from an
    /// <see cref="IMongoCollection{TDocument}"/>, combining <c>AsQueryable()</c>
    /// with ExpressiveSharp expression expansion.
    /// </summary>
    /// <param name="collection">The MongoDB collection.</param>
    /// <param name="options">
    /// Optional <see cref="ExpressiveOptions"/> controlling the transformer pipeline.
    /// When <c>null</c>, <see cref="MongoExpressiveOptions.CreateDefault"/> is used.
    /// </param>
    /// <param name="aggregateOptions">Optional MongoDB aggregation options.</param>
    public static IExpressiveMongoQueryable<T> AsExpressive<T>(
        this IMongoCollection<T> collection,
        ExpressiveOptions? options = null,
        AggregateOptions? aggregateOptions = null)
    {
        // MongoDB's AsQueryable() builds the BSON class map eagerly to prepare the
        // aggregation pipeline. We must register our ignore convention *before* that
        // call, otherwise the class map for T is constructed with the default auto-map
        // behavior that includes writable [Expressive] properties as BSON fields —
        // persisting backing-field state to the document.
        ExpressiveMongoIgnoreConvention.EnsureRegistered();

        var queryable = aggregateOptions is not null
            ? collection.AsQueryable(aggregateOptions)
            : collection.AsQueryable();

        return queryable.AsExpressive(options);
    }
}
