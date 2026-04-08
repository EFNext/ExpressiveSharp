using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

namespace ExpressiveSharp.IntegrationTests.Tests;

/// <summary>
/// Integration tests for anonymous type scenarios that verify the generated
/// interceptors compile and produce correct results at runtime.
/// Covers fixes for GitHub issues #8 (Join anonymous result) and #9 (anonymous element type).
/// </summary>
[TestClass]
public class AnonymousTypeInterceptorTests
{
    private static readonly List<Order> _orders = new()
    {
        new Order { Id = 1, Tag = "RUSH", Price = 120, Quantity = 2 },
        new Order { Id = 2, Tag = "STD", Price = 75, Quantity = 20 },
        new Order { Id = 3, Tag = null, Price = 10, Quantity = 3 },
    };

    /// <summary>
    /// Issue #9: Where after Select into anonymous type should produce
    /// a working interceptor with generic TElem parameter.
    /// </summary>
    [TestMethod]
    public void Select_AnonymousType_ThenWhere_CompilesAndRuns()
    {
        var results = _orders.AsQueryable()
            .AsExpressive()
            .Select(o => new { o.Id, Total = o.Price * o.Quantity })
            .Where(x => x.Total > 100)
            .ToList();

        Assert.AreEqual(2, results.Count);
        Assert.IsTrue(results.Any(r => r.Id == 1)); // 120*2 = 240 > 100
        Assert.IsTrue(results.Any(r => r.Id == 2)); // 75*20 = 1500 > 100
    }

    /// <summary>
    /// Issue #9: OrderByDescending after Select into anonymous type.
    /// </summary>
    [TestMethod]
    public void Select_AnonymousType_ThenOrderByDescending_CompilesAndRuns()
    {
        var results = _orders.AsQueryable()
            .AsExpressive()
            .Select(o => new { o.Id, Total = o.Price * o.Quantity })
            .OrderByDescending(x => x.Total)
            .ToList();

        Assert.AreEqual(3, results.Count);
        Assert.AreEqual(2, results[0].Id); // 1500
        Assert.AreEqual(1, results[1].Id); // 240
        Assert.AreEqual(3, results[2].Id); // 30
    }

    /// <summary>
    /// Issue #9: Select from anonymous element type to a concrete type.
    /// </summary>
    [TestMethod]
    public void Select_AnonymousType_ThenSelectConcrete_CompilesAndRuns()
    {
        var results = _orders.AsQueryable()
            .AsExpressive()
            .Select(o => new { o.Id, o.Tag })
            .Select(x => x.Id)
            .ToList();

        CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, results);
    }

    /// <summary>
    /// Issue #8: Join with anonymous result selector should produce
    /// a working interceptor with generic type parameters.
    /// </summary>
    [TestMethod]
    public void Join_AnonymousResultSelector_CompilesAndRuns()
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
                  (o, li) => new { OrderTag = o.Tag, li.ProductName })
            .ToList();

        Assert.AreEqual(2, results.Count);
        Assert.IsTrue(results.Any(r => r.OrderTag == "RUSH" && r.ProductName == "Widget"));
        Assert.IsTrue(results.Any(r => r.OrderTag == "STD" && r.ProductName == "Gadget"));
    }

    /// <summary>
    /// Issue #9: DistinctBy after Select into anonymous type, exercising
    /// EmitGenericSingleLambda with anonymous element type.
    /// </summary>
    [TestMethod]
    public void Select_AnonymousType_ThenDistinctBy_CompilesAndRuns()
    {
        var orders = new List<Order>
        {
            new() { Id = 1, Tag = "A", Price = 100, Quantity = 1 },
            new() { Id = 2, Tag = "A", Price = 200, Quantity = 1 },
            new() { Id = 3, Tag = "B", Price = 300, Quantity = 1 },
        };

        var results = orders.AsQueryable()
            .AsExpressive()
            .Select(o => new { o.Id, o.Tag })
            .DistinctBy(x => x.Tag)
            .ToList();

        Assert.AreEqual(2, results.Count);
    }
}
