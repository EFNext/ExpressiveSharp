using System.Linq.Expressions;
using ExpressiveSharp.IntegrationTests.Infrastructure;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Infrastructure;

public abstract class EFCoreTestRunnerBase : IIntegrationTestRunner
{
    protected DbContext Context { get; }

    private readonly Action<string>? _logSql;
    private bool _loggingEnabled;

    protected EFCoreTestRunnerBase(DbContext context, Action<string>? logSql = null)
    {
        Context = context;
        _logSql = logSql;
    }

    protected void EnableLogging() => _loggingEnabled = true;

    protected bool ShouldLog => _loggingEnabled && _logSql is not null;

    protected void Log(string message)
    {
        if (_loggingEnabled)
            _logSql?.Invoke(message);
    }

    public abstract Task SeedAsync(
        IReadOnlyList<Address> addresses,
        IReadOnlyList<Customer> customers,
        IReadOnlyList<Order> orders,
        IReadOnlyList<LineItem> lineItems);

    public async Task<List<TEntity>> WhereAsync<TEntity>(
        Expression<Func<TEntity, bool>> predicate) where TEntity : class
        => await Context.Set<TEntity>().Where(predicate).ToListAsync();

    public async Task<List<TResult>> SelectAsync<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> selector) where TEntity : class
        => await Context.Set<TEntity>().Select(selector).ToListAsync();

    public async Task<List<TResult>> OrderBySelectAsync<TEntity, TKey, TResult>(
        Expression<Func<TEntity, TKey>> keySelector,
        Expression<Func<TEntity, TResult>> resultSelector) where TEntity : class
        => await Context.Set<TEntity>().OrderBy(keySelector).Select(resultSelector).ToListAsync();

    public async Task<List<TResult>> OrderByDescendingSelectAsync<TEntity, TKey, TResult>(
        Expression<Func<TEntity, TKey>> keySelector,
        Expression<Func<TEntity, TResult>> resultSelector) where TEntity : class
        => await Context.Set<TEntity>().OrderByDescending(keySelector).Select(resultSelector).ToListAsync();

    public async Task<List<TResult>> OrderByThenBySelectAsync<TEntity, TKey1, TKey2, TResult>(
        Expression<Func<TEntity, TKey1>> primaryKeySelector,
        Expression<Func<TEntity, TKey2>> secondaryKeySelector,
        Expression<Func<TEntity, TResult>> resultSelector) where TEntity : class
        => await Context.Set<TEntity>().OrderBy(primaryKeySelector).ThenBy(secondaryKeySelector).Select(resultSelector).ToListAsync();

    public async Task<List<KeyValuePair<TKey, int>>> GroupByCountAsync<TEntity, TKey>(
        Expression<Func<TEntity, TKey>> keySelector) where TEntity : class
        => await Context.Set<TEntity>().GroupBy(keySelector)
            .Select(g => new KeyValuePair<TKey, int>(g.Key, g.Count()))
            .ToListAsync();

    public abstract ValueTask DisposeAsync();
}
