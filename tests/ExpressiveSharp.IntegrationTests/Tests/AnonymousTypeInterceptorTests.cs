using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

namespace ExpressiveSharp.IntegrationTests.Tests;

// Regression coverage for GitHub issues #8 (Join anonymous result) and #9 (anonymous element type).
[TestClass]
public class AnonymousTypeInterceptorTests
{
    private static readonly List<Order> _orders = new()
    {
        new Order { Id = 1, Tag = "RUSH", Price = 120, Quantity = 2 },
        new Order { Id = 2, Tag = "STD", Price = 75, Quantity = 20 },
        new Order { Id = 3, Tag = null, Price = 10, Quantity = 3 },
    };

    // Issue #9: Where after Select into anonymous type — interceptor with generic TElem parameter.
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

    // Issue #8: Join with anonymous result selector — interceptor with generic type parameters.
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

    // Issue #9: DistinctBy after Select into anonymous type — exercises EmitGenericSingleLambda
    // with anonymous element type.
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
