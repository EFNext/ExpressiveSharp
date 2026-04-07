using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.Services;
using MongoDB.Bson;
using MongoDB.Driver.Linq;

namespace ExpressiveSharp.MongoDB.Infrastructure;

/// <summary>
/// Decorates MongoDB's <see cref="IMongoQueryProvider"/> to automatically expand
/// <see cref="ExpressiveAttribute"/> member references before query execution.
/// </summary>
/// <remarks>
/// <see cref="CreateQuery{TElement}"/> returns an <see cref="ExpressiveMongoQueryable{T}"/>
/// wrapper so that chained operations continue to use this provider.
/// <see cref="Execute{TResult}"/> and <see cref="ExecuteAsync{TResult}"/> call
/// <see cref="ExpressionExtensions.ExpandExpressives(Expression, ExpressiveOptions)"/>
/// on the expression before delegating to the inner provider.
/// </remarks>
internal sealed class ExpressiveMongoQueryProvider : IMongoQueryProvider
{
    private readonly IMongoQueryProvider _inner;
    private readonly ExpressiveOptions _options;

    public ExpressiveMongoQueryProvider(IMongoQueryProvider inner, ExpressiveOptions options)
    {
        _inner = inner;
        _options = options;
    }

    public BsonDocument[] LoggedStages => _inner.LoggedStages;

    public IQueryable CreateQuery(Expression expression)
    {
        var inner = _inner.CreateQuery(expression);
        // Wrap in our queryable to maintain provider chain
        var elementType = inner.ElementType;
        var wrapperType = typeof(ExpressiveMongoQueryable<>).MakeGenericType(elementType);
        return (IQueryable)Activator.CreateInstance(wrapperType, inner, this)!;
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        var inner = _inner.CreateQuery<TElement>(expression);
        return new ExpressiveMongoQueryable<TElement>(inner, this);
    }

    public object? Execute(Expression expression)
        => _inner.Execute(Expand(expression));

    public TResult Execute<TResult>(Expression expression)
        => _inner.Execute<TResult>(Expand(expression));

    public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        => _inner.ExecuteAsync<TResult>(Expand(expression), cancellationToken);

    internal Expression Expand(Expression expression)
        => expression.ExpandExpressives(_options);
}
