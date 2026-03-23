using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Common.Tests;

public abstract class NullConditionalTests : StoreTestBase
{
    [TestMethod]
    public async Task Select_CustomerName_ReturnsCorrectNullableValues()
    {
        Expression<Func<Order, string?>> expr = o => o.CustomerName;
        var expanded = (Expression<Func<Order, string?>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, string?>(expanded);

        // Order 1: Alice, Order 2: Bob, Order 3: null (no customer), Order 4: null (customer.Name is null)
        CollectionAssert.AreEquivalent(
            new[] { "Alice", "Bob", null, null },
            results);
    }

    [TestMethod]
    public async Task Select_TagLength_ReturnsCorrectNullableValues()
    {
        Expression<Func<Order, int?>> expr = o => o.TagLength;
        var expanded = (Expression<Func<Order, int?>>)expr.ExpandExpressives();

        var results = await Runner.SelectAsync<Order, int?>(expanded);

        // Order 1: "RUSH".Length=4, Order 2: "STD".Length=3, Order 3: null, Order 4: "SPECIAL".Length=7
        CollectionAssert.AreEquivalent(
            new int?[] { 4, 3, null, 7 },
            results);
    }

    [TestMethod]
    public async Task Where_CustomerNameEquals_FiltersCorrectly()
    {
        Expression<Func<Order, string?>> nameExpr = o => o.CustomerName;
        var expanded = (Expression<Func<Order, string?>>)nameExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.Equal(expanded.Body, Expression.Constant("Alice", typeof(string)));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = await Runner.WhereAsync(predicate);

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(1, results[0].Id);
    }

    [TestMethod]
    public async Task Where_CustomerNameIsNull_FiltersCorrectly()
    {
        Expression<Func<Order, string?>> nameExpr = o => o.CustomerName;
        var expanded = (Expression<Func<Order, string?>>)nameExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.Equal(expanded.Body, Expression.Constant(null, typeof(string)));
        var predicate = Expression.Lambda<Func<Order, bool>>(body, param);

        var results = await Runner.WhereAsync(predicate);

        // Order 3: no customer, Order 4: customer with null Name
        var ids = results.Select(o => o.Id).OrderBy(id => id).ToList();
        CollectionAssert.AreEqual(new[] { 3, 4 }, ids);
    }

    [TestMethod]
    public async Task OrderBy_TagLength_NullsAppearFirst()
    {
        Expression<Func<Order, int?>> tagLenExpr = o => o.TagLength;
        var expanded = (Expression<Func<Order, int?>>)tagLenExpr.ExpandExpressives();

        Expression<Func<Order, int>> idExpr = o => o.Id;

        var results = await Runner.OrderBySelectAsync<Order, int?, int>(expanded, idExpr);

        // null sorts first, then 3 ("STD"), 4 ("RUSH"), 7 ("SPECIAL")
        Assert.AreEqual(3, results[0]); // Order 3 has null Tag
    }
}
