using System.Linq.Expressions;

namespace ExpressiveSharp.FunctionalTests.Infrastructure;

public interface IExpressionConsumer : IDisposable
{
    Task AssertWhereTranslatesAsync<TEntity>(
        Expression<Func<TEntity, bool>> predicate) where TEntity : class;

    Task AssertSelectTranslatesAsync<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> selector) where TEntity : class;

    Task AssertOrderByTranslatesAsync<TEntity, TKey>(
        Expression<Func<TEntity, TKey>> keySelector) where TEntity : class;
}
