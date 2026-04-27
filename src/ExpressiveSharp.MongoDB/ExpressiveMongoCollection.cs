using ExpressiveSharp.MongoDB.Extensions;
using ExpressiveSharp.MongoDB.Infrastructure;
using ExpressiveSharp.Services;
using MongoDB.Driver;

namespace ExpressiveSharp.MongoDB;

/// <summary>
/// Wraps an <see cref="IMongoCollection{TDocument}"/> to expose an
/// <see cref="IExpressiveMongoQueryable{T}"/> with automatic <c>[Expressive]</c> expansion.
/// Analogous to <c>ExpressiveDbSet&lt;TEntity&gt;</c> in the EF Core integration.
/// </summary>
public class ExpressiveMongoCollection<TDocument>
{
    private readonly IMongoCollection<TDocument> _inner;
    private readonly ExpressiveOptions _options;

    public ExpressiveMongoCollection(IMongoCollection<TDocument> inner, ExpressiveOptions? options = null)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _options = options ?? MongoExpressiveOptions.CreateDefault();

        // Belt-and-braces for code paths that access `Inner` directly for writes
        // (InsertOne/ReplaceOne/…) without ever going through `AsQueryable`.
        // The `AsExpressive` extension registers the convention on the query path.
        ExpressiveMongoIgnoreConvention.EnsureRegistered();
    }

    public IMongoCollection<TDocument> Inner => _inner;

    public IExpressiveMongoQueryable<TDocument> AsQueryable(AggregateOptions? aggregateOptions = null)
        => _inner.AsExpressive(_options, aggregateOptions);
}
