using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Common.Tests;

public abstract class NullableChainTests : StoreTestBase
{
    [TestMethod]
    public async Task Select_CustomerCountry_TwoLevelChain()
    {
        Expression<Func<Order, string?>> expr = o => o.CustomerCountry;
        var expanded = (Expression<Func<Order, string?>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, string?>(expanded);

        // Order 1: Customer 1 → Address 1 → "US"
        // Order 2: Customer 2 → Address 2 → "UK"
        // Order 3: no customer → null
        // Order 4: Customer 3 → no address → null
        CollectionAssert.AreEquivalent(
            new[] { "US", "UK", null, null },
            results);
    }

    [TestMethod]
    public async Task Where_CustomerCountryEquals_FiltersCorrectly()
    {
        Expression<Func<Order, string?>> countryExpr = o => o.CustomerCountry;
        var expanded = (Expression<Func<Order, string?>>)countryExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.Equal(expanded.Body, Expression.Constant("US", typeof(string)));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = await Runner.WhereAsync(predicate);

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(1, results[0].Id);
    }

    [TestMethod]
    public async Task Select_CustomerCity_ViaCustomerExpressive()
    {
        // Test Customer.City which is Customer?.Address?.City (single-level on Customer)
        var expr = ExpressionPolyfill.Create((Customer c) => c.Address != null ? c.Address.City : null);

        var results = await Runner.SelectAsync<Customer, string?>(expr);

        CollectionAssert.AreEquivalent(
            new[] { "New York", "London", null },
            results);
    }
}
