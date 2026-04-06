using System.Linq.Expressions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

namespace ExpressiveSharp.IntegrationTests.Infrastructure;

/// <summary>
/// In-memory helper used by the ExpressionCompile test classes. Compiles
/// expression trees to delegates and runs them against seeded
/// <see cref="List{T}"/> collections — no database involved.
///
/// Validates that expanded expression trees are behaviorally correct at
/// runtime (verification level 3 in docs/advanced/testing-strategy.md) without
/// relying on any ORM-specific translation.
/// </summary>
internal sealed class ExpressionCompileRunner
{
    private List<Order> _orders = new();
    private List<Customer> _customers = new();

    public void Seed(
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
    }

    public List<TEntity> Where<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
    {
        var compiled = predicate.Compile();
        return GetSource<TEntity>().Where(compiled).ToList();
    }

    public List<TResult> Select<TEntity, TResult>(Expression<Func<TEntity, TResult>> selector) where TEntity : class
    {
        var compiled = selector.Compile();
        return GetSource<TEntity>().Select(compiled).ToList();
    }

    public List<TResult> OrderBySelect<TEntity, TKey, TResult>(
        Expression<Func<TEntity, TKey>> keySelector,
        Expression<Func<TEntity, TResult>> resultSelector) where TEntity : class
    {
        var compiledKey = keySelector.Compile();
        var compiledResult = resultSelector.Compile();
        return GetSource<TEntity>().OrderBy(compiledKey).Select(compiledResult).ToList();
    }

    public List<TResult> OrderByDescendingSelect<TEntity, TKey, TResult>(
        Expression<Func<TEntity, TKey>> keySelector,
        Expression<Func<TEntity, TResult>> resultSelector) where TEntity : class
    {
        var compiledKey = keySelector.Compile();
        var compiledResult = resultSelector.Compile();
        return GetSource<TEntity>().OrderByDescending(compiledKey).Select(compiledResult).ToList();
    }

    public List<TResult> OrderByThenBySelect<TEntity, TKey1, TKey2, TResult>(
        Expression<Func<TEntity, TKey1>> primaryKeySelector,
        Expression<Func<TEntity, TKey2>> secondaryKeySelector,
        Expression<Func<TEntity, TResult>> resultSelector) where TEntity : class
    {
        var compiledPrimary = primaryKeySelector.Compile();
        var compiledSecondary = secondaryKeySelector.Compile();
        var compiledResult = resultSelector.Compile();
        return GetSource<TEntity>().OrderBy(compiledPrimary).ThenBy(compiledSecondary).Select(compiledResult).ToList();
    }

    public List<KeyValuePair<TKey, int>> GroupByCount<TEntity, TKey>(
        Expression<Func<TEntity, TKey>> keySelector) where TEntity : class
    {
        var compiledKey = keySelector.Compile();
        return GetSource<TEntity>()
            .GroupBy(compiledKey)
            .Select(g => new KeyValuePair<TKey, int>(g.Key, g.Count()))
            .ToList();
    }

    private List<TEntity> GetSource<TEntity>() where TEntity : class
    {
        if (typeof(TEntity) == typeof(Order))
            return (List<TEntity>)(object)_orders;
        if (typeof(TEntity) == typeof(Customer))
            return (List<TEntity>)(object)_customers;

        throw new NotSupportedException($"Unknown entity type: {typeof(TEntity).Name}");
    }
}
