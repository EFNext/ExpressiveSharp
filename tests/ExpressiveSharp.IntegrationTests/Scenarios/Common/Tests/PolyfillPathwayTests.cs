using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Common.Tests;

public abstract class PolyfillPathwayTests : StoreTestBase
{
    [TestMethod]
    public async Task Polyfill_SimpleCondition_FiltersCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Price > 50);

        var results = await Runner.WhereAsync(expr);

        var ids = results.Select(o => o.Id).OrderBy(id => id).ToList();
        // Order 1: 120 > 50 ✓, Order 2: 75 > 50 ✓, Order 3: 10 > 50 ✗, Order 4: 50 > 50 ✗
        CollectionAssert.AreEqual(new[] { 1, 2 }, ids);
    }

    [TestMethod]
    public async Task Polyfill_Arithmetic_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Price * o.Quantity);

        var results = await Runner.SelectAsync<Order, double>(expr);

        CollectionAssert.AreEquivalent(
            new[] { 240.0, 1500.0, 30.0, 250.0 },
            results);
    }

    [TestMethod]
    public async Task Polyfill_NullConditional_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Customer != null ? o.Customer.Name : null);

        var results = await Runner.SelectAsync<Order, string?>(expr);

        CollectionAssert.AreEquivalent(
            new[] { "Alice", "Bob", null, null },
            results);
    }

    [TestMethod]
    public async Task Polyfill_NullCoalescing_ProjectsCorrectly()
    {
        var expr = ExpressionPolyfill.Create((Order o) => o.Tag ?? "N/A");

        var results = await Runner.SelectAsync<Order, string>(expr);

        CollectionAssert.AreEquivalent(
            new[] { "RUSH", "STD", "N/A", "SPECIAL" },
            results);
    }
}
