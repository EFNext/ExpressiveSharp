using ExpressiveSharp.MongoDB.Extensions;
using ExpressiveSharp.Services;
using MongoDB.Driver;

namespace ExpressiveSharp.MongoDB;

/// <summary>
/// A wrapper around <see cref="IMongoCollection{TDocument}"/> that provides an
/// <see cref="IExpressiveMongoQueryable{T}"/> for delegate-based LINQ queries
/// with automatic <c>[Expressive]</c> member expansion.
/// </summary>
/// <remarks>
/// Analogous to <c>ExpressiveDbSet&lt;TEntity&gt;</c> in the EF Core integration.
/// CRUD operations delegate directly to the inner collection.
/// </remarks>
/// <example>
/// <code>
/// var orders = new ExpressiveMongoCollection&lt;Order&gt;(collection);
/// var results = await orders.AsQueryable()
///     .Where(o => o.Customer?.Name == "Alice")
///     .ToListAsync();
/// </code>
/// </example>
public class ExpressiveMongoCollection<TDocument>
{
    private readonly IMongoCollection<TDocument> _inner;
    private readonly ExpressiveOptions _options;

    /// <summary>
    /// Creates a new <see cref="ExpressiveMongoCollection{TDocument}"/> wrapping the specified collection.
    /// </summary>
    /// <param name="inner">The underlying MongoDB collection.</param>
    /// <param name="options">
    /// Optional <see cref="ExpressiveOptions"/> controlling the transformer pipeline.
    /// When <c>null</c>, <see cref="MongoExpressiveOptions.CreateDefault"/> is used.
    /// </param>
    public ExpressiveMongoCollection(IMongoCollection<TDocument> inner, ExpressiveOptions? options = null)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _options = options ?? MongoExpressiveOptions.CreateDefault();
    }

    /// <summary>
    /// Gets the underlying <see cref="IMongoCollection{TDocument}"/> for direct access
    /// to non-LINQ operations (inserts, updates, deletes, aggregation pipeline, etc.).
    /// </summary>
    public IMongoCollection<TDocument> Inner => _inner;

    /// <summary>
    /// Returns an <see cref="IExpressiveMongoQueryable{T}"/> that supports delegate-based
    /// LINQ with modern C# syntax and automatic <c>[Expressive]</c> expansion.
    /// </summary>
    /// <param name="aggregateOptions">Optional MongoDB aggregation options.</param>
    public IExpressiveMongoQueryable<TDocument> AsQueryable(AggregateOptions? aggregateOptions = null)
        => _inner.AsExpressive(_options, aggregateOptions);
}
