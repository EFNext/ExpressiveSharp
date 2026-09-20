using System.Collections;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace ExpressiveSharp.MongoDB.Infrastructure;

/// <remarks>
/// Implements <see cref="IOrderedExpressiveQueryable{T}"/> so <c>AsExpressive</c> returns this
/// instance unchanged after an ordering operator; re-wrapping would drop
/// <see cref="IAsyncCursorSource{TDocument}"/>, which <c>MongoQueryable.ToListAsync</c> /
/// <c>ToCursorAsync</c> require because they cast the source rather than the provider.
/// </remarks>
internal sealed class ExpressiveMongoQueryable<T>
    : IExpressiveMongoQueryable<T>, IOrderedExpressiveQueryable<T>, IAsyncCursorSource<T>
{
    private readonly IQueryable<T> _source;
    private readonly ExpressiveMongoQueryProvider _provider;

    public ExpressiveMongoQueryable(IQueryable<T> source, ExpressiveMongoQueryProvider provider)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public Type ElementType => _source.ElementType;
    public Expression Expression => _source.Expression;
    public IQueryProvider Provider => _provider;

    public IEnumerator<T> GetEnumerator()
        => _provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Returns the MongoDB aggregation pipeline (MQL) for this query without executing it.
    /// </summary>
    public override string ToString()
        => ExpandedInnerQueryable().ToString() ?? base.ToString()!;

    public IAsyncCursor<T> ToCursor(CancellationToken cancellationToken = default)
        => ((IAsyncCursorSource<T>)ExpandedInnerQueryable()).ToCursor(cancellationToken);

    public Task<IAsyncCursor<T>> ToCursorAsync(CancellationToken cancellationToken = default)
        => ((IAsyncCursorSource<T>)ExpandedInnerQueryable()).ToCursorAsync(cancellationToken);

    // Expand [Expressive] members and rebuild a fresh inner queryable bound to MongoDB's
    // native provider, which implements IAsyncCursorSource<T>.
    private IQueryable<T> ExpandedInnerQueryable()
        => _source.Provider.CreateQuery<T>(_provider.Expand(_source.Expression));
}
