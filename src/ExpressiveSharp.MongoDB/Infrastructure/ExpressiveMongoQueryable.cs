using System.Collections;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace ExpressiveSharp.MongoDB.Infrastructure;

/// <summary>
/// Internal wrapper that adapts an <see cref="IQueryable{T}"/> backed by MongoDB's LINQ provider
/// to <see cref="IExpressiveMongoQueryable{T}"/>, enabling delegate-based LINQ overloads
/// with modern C# syntax via source generator interception.
/// </summary>
/// <remarks>
/// Also implements <see cref="IOrderedQueryable{T}"/> so that <c>ThenBy</c>/<c>ThenByDescending</c>
/// interceptors can cast the wrapper without a runtime exception, and
/// <see cref="IAsyncCursorSource{TDocument}"/> so that <c>MongoQueryable.ToListAsync</c> /
/// <c>ToCursorAsync</c> (which cast the source directly, not the provider) accept the wrapper.
/// </remarks>
internal sealed class ExpressiveMongoQueryable<T> : IExpressiveMongoQueryable<T>, IOrderedQueryable<T>, IAsyncCursorSource<T>
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

    public IEnumerator<T> GetEnumerator() => _source.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_source).GetEnumerator();

    public IAsyncCursor<T> ToCursor(CancellationToken cancellationToken = default)
        => ((IAsyncCursorSource<T>)ExpandedInnerQueryable()).ToCursor(cancellationToken);

    public Task<IAsyncCursor<T>> ToCursorAsync(CancellationToken cancellationToken = default)
        => ((IAsyncCursorSource<T>)ExpandedInnerQueryable()).ToCursorAsync(cancellationToken);

    // Expand [Expressive] members in the expression tree and rebuild a fresh inner queryable
    // bound to MongoDB's native provider, which implements IAsyncCursorSource<T>.
    private IQueryable<T> ExpandedInnerQueryable()
        => _source.Provider.CreateQuery<T>(_provider.Expand(_source.Expression));
}
