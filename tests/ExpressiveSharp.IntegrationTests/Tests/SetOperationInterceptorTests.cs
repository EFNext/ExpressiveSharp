using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

namespace ExpressiveSharp.IntegrationTests.Tests;

[TestClass]
public class SetOperationInterceptorTests
{
    private static readonly List<Order> _orders = new()
    {
        new Order { Id = 1, Tag = "A", Price = 100, Quantity = 2 },
        new Order { Id = 2, Tag = "B", Price = 50, Quantity = 1 },
        new Order { Id = 3, Tag = "C", Price = 25, Quantity = 4 },
    };

    [TestMethod]
    public void UnionBy_WithElementSequence_CompilesAndRuns()
    {
        var source = _orders.AsQueryable();
        var extra = new List<Order>
        {
            new() { Id = 4, Tag = "B", Price = 999, Quantity = 9 },
            new() { Id = 5, Tag = "D", Price = 10, Quantity = 1 },
        };

        var results = source.AsExpressive()
            .UnionBy(extra, o => o.Tag)
            .ToList();

        var tags = results.Select(o => o.Tag).ToList();
        Assert.AreEqual(4, results.Count);
        CollectionAssert.AreEquivalent(new[] { "A", "B", "C", "D" }, tags);
        Assert.AreEqual(2, results.Single(o => o.Tag == "B").Id);
    }

    [TestMethod]
    public void ExceptBy_WithKeySequence_CompilesAndRuns()
    {
        var source = _orders.AsQueryable();
        var excluded = new[] { "B" };

        var results = source.AsExpressive()
            .ExceptBy(excluded, o => o.Tag)
            .ToList();

        CollectionAssert.AreEquivalent(new[] { "A", "C" }, results.Select(o => o.Tag).ToList());
    }

    [TestMethod]
    public void IntersectBy_WithKeySequence_CompilesAndRuns()
    {
        var source = _orders.AsQueryable();
        var included = new[] { "B", "C", "X" };

        var results = source.AsExpressive()
            .IntersectBy(included, o => o.Tag)
            .ToList();

        CollectionAssert.AreEquivalent(new[] { "B", "C" }, results.Select(o => o.Tag).ToList());
    }
}
