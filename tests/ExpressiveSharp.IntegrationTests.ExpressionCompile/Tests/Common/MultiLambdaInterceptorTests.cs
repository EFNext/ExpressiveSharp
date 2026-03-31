using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

namespace ExpressiveSharp.IntegrationTests.ExpressionCompile.Tests.Common;

[TestClass]
public class MultiLambdaInterceptorTests
{
    private static readonly List<Order> _orders = new()
    {
        new Order
        {
            Id = 1, Tag = "A", Price = 100, Quantity = 2,
            Items = new List<LineItem>
            {
                new() { Id = 1, OrderId = 1, ProductName = "Widget", UnitPrice = 50, Quantity = 2 },
                new() { Id = 2, OrderId = 1, ProductName = "Gadget", UnitPrice = 75, Quantity = 1 },
            }
        },
        new Order
        {
            Id = 2, Tag = "B", Price = 50, Quantity = 1,
            Items = new List<LineItem>
            {
                new() { Id = 3, OrderId = 2, ProductName = "Doohickey", UnitPrice = 25, Quantity = 3 },
            }
        },
    };

    /// <summary>
    /// SelectMany with result selector uses two lambdas that share the parameter name 'o'.
    /// This tests that the generator emits unique variable names for each lambda body.
    /// </summary>
    [TestMethod]
    public void SelectMany_WithResultSelector_SharedParamName_CompilesAndRuns()
    {
        var source = _orders.AsQueryable();

        // Both lambdas use 'o' — this triggers the duplicate variable name bug
        // if the generator doesn't prefix local variables per lambda.
        var results = source.AsExpressive()
            .SelectMany(o => o.Items, (o, item) => o.Tag + ": " + item.ProductName)
            .ToList();

        Assert.AreEqual(3, results.Count);
        CollectionAssert.Contains(results, "A: Widget");
        CollectionAssert.Contains(results, "A: Gadget");
        CollectionAssert.Contains(results, "B: Doohickey");
    }

    /// <summary>
    /// Join with three lambdas where outer and inner key selectors share parameter patterns.
    /// Tests unique variable naming across all three lambda bodies.
    /// Uses non-nullable int keys (Id) to avoid nullable type mismatch issues.
    /// </summary>
    [TestMethod]
    public void Join_ThreeLambdas_CompilesAndRuns()
    {
        var orders = _orders.AsQueryable();
        var lineItems = new List<LineItem>
        {
            new() { Id = 1, OrderId = 1, ProductName = "Widget", UnitPrice = 50, Quantity = 2 },
            new() { Id = 2, OrderId = 2, ProductName = "Gadget", UnitPrice = 25, Quantity = 3 },
        };

        var results = orders.AsExpressive()
            .Join(lineItems,
                  o => o.Id,
                  li => li.OrderId,
                  (o, li) => o.Tag + ": " + li.ProductName)
            .ToList();

        Assert.AreEqual(2, results.Count);
        CollectionAssert.Contains(results, "A: Widget");
        CollectionAssert.Contains(results, "B: Gadget");
    }

    /// <summary>
    /// Join where the outer key is nullable (int?) but the inner key is non-nullable (int).
    /// The emitter must insert an implicit conversion so the expression tree compiles.
    /// </summary>
    [TestMethod]
    public void Join_NullableOuterKey_NonNullableInnerKey_CompilesAndRuns()
    {
        var orders = _orders.AsQueryable();
        var customers = new List<Customer>
        {
            new() { Id = 1, Name = "Alice" },
            new() { Id = 2, Name = "Bob" },
        };

        // Order.CustomerId is int? — Customer.Id is int — TKey resolves to int?
        _orders[0].CustomerId = 1;
        _orders[1].CustomerId = 2;

        var results = orders.AsExpressive()
            .Join(customers,
                  o => o.CustomerId,
                  c => c.Id,
                  (o, c) => c.Name)
            .ToList();

        Assert.AreEqual(2, results.Count);
        CollectionAssert.Contains(results, "Alice");
        CollectionAssert.Contains(results, "Bob");
    }
}
