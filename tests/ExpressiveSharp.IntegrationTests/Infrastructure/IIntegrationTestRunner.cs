using System.Linq.Expressions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

namespace ExpressiveSharp.IntegrationTests.Infrastructure;

public interface IIntegrationTestRunner : IAsyncDisposable
{
    Task SeedAsync(
        IReadOnlyList<Address> addresses,
        IReadOnlyList<Customer> customers,
        IReadOnlyList<Order> orders,
        IReadOnlyList<LineItem> lineItems);

    Task<List<TEntity>> WhereAsync<TEntity>(
        Expression<Func<TEntity, bool>> predicate) where TEntity : class;

    Task<List<TResult>> SelectAsync<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> selector) where TEntity : class;

    Task<List<TResult>> OrderBySelectAsync<TEntity, TKey, TResult>(
        Expression<Func<TEntity, TKey>> keySelector,
        Expression<Func<TEntity, TResult>> resultSelector) where TEntity : class;

    Task<List<TResult>> OrderByDescendingSelectAsync<TEntity, TKey, TResult>(
        Expression<Func<TEntity, TKey>> keySelector,
        Expression<Func<TEntity, TResult>> resultSelector) where TEntity : class;

    Task<List<TResult>> OrderByThenBySelectAsync<TEntity, TKey1, TKey2, TResult>(
        Expression<Func<TEntity, TKey1>> primaryKeySelector,
        Expression<Func<TEntity, TKey2>> secondaryKeySelector,
        Expression<Func<TEntity, TResult>> resultSelector) where TEntity : class;

    Task<List<KeyValuePair<TKey, int>>> GroupByCountAsync<TEntity, TKey>(
        Expression<Func<TEntity, TKey>> keySelector) where TEntity : class;
}
