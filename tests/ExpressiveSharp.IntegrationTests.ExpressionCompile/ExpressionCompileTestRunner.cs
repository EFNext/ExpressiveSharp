using System.Linq.Expressions;
using ExpressiveSharp.IntegrationTests.Infrastructure;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

namespace ExpressiveSharp.IntegrationTests.ExpressionCompile;

public sealed class ExpressionCompileTestRunner : IIntegrationTestRunner
{
    private List<Order> _orders = new();
    private List<Customer> _customers = new();

    public Task SeedAsync(
        IReadOnlyList<Address> addresses,
        IReadOnlyList<Customer> customers,
        IReadOnlyList<Order> orders,
        IReadOnlyList<LineItem> lineItems)
    {
        var addressList = addresses.ToList();
        _customers = customers.Select(c => new Customer
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            AddressId = c.AddressId,
            Address = addressList.FirstOrDefault(a => a.Id == c.AddressId),
        }).ToList();

        _orders = orders.Select(o => new Order
        {
            Id = o.Id,
            Tag = o.Tag,
            Price = o.Price,
            Quantity = o.Quantity,
            Status = o.Status,
            CustomerId = o.CustomerId,
            Customer = _customers.FirstOrDefault(c => c.Id == o.CustomerId),
            Items = lineItems.Where(li => li.OrderId == o.Id).ToList(),
        }).ToList();

        return Task.CompletedTask;
    }

    public Task<List<TEntity>> WhereAsync<TEntity>(
        Expression<Func<TEntity, bool>> predicate) where TEntity : class
    {
        var compiled = predicate.Compile();
        var source = GetSource<TEntity>();
        return Task.FromResult(source.Where(compiled).ToList());
    }

    public Task<List<TResult>> SelectAsync<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> selector) where TEntity : class
    {
        var compiled = selector.Compile();
        var source = GetSource<TEntity>();
        return Task.FromResult(source.Select(compiled).ToList());
    }

    public Task<List<TResult>> OrderBySelectAsync<TEntity, TKey, TResult>(
        Expression<Func<TEntity, TKey>> keySelector,
        Expression<Func<TEntity, TResult>> resultSelector) where TEntity : class
    {
        var compiledKey = keySelector.Compile();
        var compiledResult = resultSelector.Compile();
        var source = GetSource<TEntity>();
        return Task.FromResult(source.OrderBy(compiledKey).Select(compiledResult).ToList());
    }

    public Task<List<TResult>> OrderByDescendingSelectAsync<TEntity, TKey, TResult>(
        Expression<Func<TEntity, TKey>> keySelector,
        Expression<Func<TEntity, TResult>> resultSelector) where TEntity : class
    {
        var compiledKey = keySelector.Compile();
        var compiledResult = resultSelector.Compile();
        var source = GetSource<TEntity>();
        return Task.FromResult(source.OrderByDescending(compiledKey).Select(compiledResult).ToList());
    }

    public Task<List<TResult>> OrderByThenBySelectAsync<TEntity, TKey1, TKey2, TResult>(
        Expression<Func<TEntity, TKey1>> primaryKeySelector,
        Expression<Func<TEntity, TKey2>> secondaryKeySelector,
        Expression<Func<TEntity, TResult>> resultSelector) where TEntity : class
    {
        var compiledPrimary = primaryKeySelector.Compile();
        var compiledSecondary = secondaryKeySelector.Compile();
        var compiledResult = resultSelector.Compile();
        var source = GetSource<TEntity>();
        return Task.FromResult(source.OrderBy(compiledPrimary).ThenBy(compiledSecondary).Select(compiledResult).ToList());
    }

    public Task<List<KeyValuePair<TKey, int>>> GroupByCountAsync<TEntity, TKey>(
        Expression<Func<TEntity, TKey>> keySelector) where TEntity : class
    {
        var compiledKey = keySelector.Compile();
        var source = GetSource<TEntity>();
        return Task.FromResult(
            source.GroupBy(compiledKey)
                .Select(g => new KeyValuePair<TKey, int>(g.Key, g.Count()))
                .ToList());
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private List<TEntity> GetSource<TEntity>() where TEntity : class
    {
        if (typeof(TEntity) == typeof(Order))
            return (List<TEntity>)(object)_orders;
        if (typeof(TEntity) == typeof(Customer))
            return (List<TEntity>)(object)_customers;

        throw new NotSupportedException($"Unknown entity type: {typeof(TEntity).Name}");
    }
}
