using System.Linq.Expressions;

namespace ExpressiveSharp.FunctionalTests.Infrastructure;

public sealed class ExpressionVisitorConsumer : IExpressionConsumer
{
    private readonly StrictExpressionVisitor _visitor = new();

    public Task AssertWhereTranslatesAsync<TEntity>(
        Expression<Func<TEntity, bool>> predicate) where TEntity : class
    {
        _visitor.Visit(predicate);
        return Task.CompletedTask;
    }

    public Task AssertSelectTranslatesAsync<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> selector) where TEntity : class
    {
        _visitor.Visit(selector);
        return Task.CompletedTask;
    }

    public Task AssertOrderByTranslatesAsync<TEntity, TKey>(
        Expression<Func<TEntity, TKey>> keySelector) where TEntity : class
    {
        _visitor.Visit(keySelector);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
    }
}
