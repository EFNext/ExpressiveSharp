using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

namespace ExpressiveSharp.IntegrationTests.Tests;

[TestClass]
public class OrderingInterceptorTests
{
    private static readonly List<Order> _orders = new()
    {
        new Order { Id = 1, Tag = "B", Price = 100, Quantity = 2 },
        new Order { Id = 2, Tag = "A", Price = 50, Quantity = 1 },
        new Order { Id = 3, Tag = "B", Price = 25, Quantity = 4 },
        new Order { Id = 4, Tag = "A", Price = 75, Quantity = 3 },
    };

    [TestMethod]
    public void OrderBy_ThenBy_ComposesBothKeys()
    {
        var results = _orders.AsQueryable().AsExpressive()
            .OrderBy(o => o.Tag)
            .ThenBy(o => o.Id)
            .Select(o => o.Id)
            .ToList();

        CollectionAssert.AreEqual(new[] { 2, 4, 1, 3 }, results);
    }

    [TestMethod]
    public void OrderByDescending_ThenByDescending_ComposesBothKeys()
    {
        var results = _orders.AsQueryable().AsExpressive()
            .OrderByDescending(o => o.Tag)
            .ThenByDescending(o => o.Id)
            .Select(o => o.Id)
            .ToList();

        CollectionAssert.AreEqual(new[] { 3, 1, 4, 2 }, results);
    }

    [TestMethod]
    public void OrderBy_ThenBy_WithComparer_ComposesBothKeys()
    {
        var results = _orders.AsQueryable().AsExpressive()
            .OrderBy(o => o.Tag, StringComparer.Ordinal)
            .ThenBy(o => o.Id, Comparer<int>.Default)
            .Select(o => o.Id)
            .ToList();

        CollectionAssert.AreEqual(new[] { 2, 4, 1, 3 }, results);
    }

    [TestMethod]
    public void PlainOrderBy_ThenAsExpressive_StaysOrdered()
    {
        var results = _orders.AsQueryable()
            .OrderBy(o => o.Tag)
            .AsExpressive()
            .ThenBy(o => o.Id)
            .Select(o => o.Id)
            .ToList();

        CollectionAssert.AreEqual(new[] { 2, 4, 1, 3 }, results);
    }

    [TestMethod]
    public void OrderBy_UsesModernSyntaxInKeySelector()
    {
        var results = _orders.AsQueryable().AsExpressive()
            .OrderBy(o => o.Tag switch { "A" => 0, _ => 1 })
            .ThenBy(o => o.Id)
            .Select(o => o.Id)
            .ToList();

        CollectionAssert.AreEqual(new[] { 2, 4, 1, 3 }, results);
    }
}
