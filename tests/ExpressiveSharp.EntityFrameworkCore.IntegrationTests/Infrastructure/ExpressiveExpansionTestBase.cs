using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

/// <summary>
/// Regression tests verifying that [Expressive] members are correctly expanded
/// through the ExpressiveQueryCompiler pipeline in real EF Core queries.
/// </summary>
public abstract class ExpressiveExpansionTestBase : EFCoreRelationalTestBase
{
    [TestInitialize]
    public Task SeedStoreData() => Context.SeedStoreAsync();

    // ── Nested [Expressive] calls ───────────────────────────────────────────

    [TestMethod]
    public async Task NestedExpressive_TotalUsedInGetGrade_ExpandsRecursively()
    {
        var results = await Context.Orders
            .Select(o => new { o.Id, Grade = o.GetGrade(), o.Total })
            .OrderBy(x => x.Id)
            .ToListAsync();

        Assert.AreEqual("Premium", results[0].Grade);
        Assert.AreEqual(240.0, results[0].Total);
        Assert.AreEqual("Budget", results[2].Grade);
    }

    [TestMethod]
    public virtual async Task NestedExpressive_GetOrderSummaryTuple_ExpandsBothGetGradeAndTotal()
    {
        var results = await Context.Orders
            .OrderBy(o => o.Id)
            .Select(o => o.GetOrderSummaryTuple())
            .ToListAsync();

        Assert.AreEqual(1, results[0].Id);
        Assert.AreEqual("Premium", results[0].Grade);
        Assert.AreEqual(240.0, results[0].Total);

        Assert.AreEqual(3, results[2].Id);
        Assert.AreEqual("Budget", results[2].Grade);
        Assert.AreEqual(30.0, results[2].Total);
    }

    // ── [Expressive] in Where ───────────────────────────────────────────────

    [TestMethod]
    public async Task ExpressiveInWhere_FilterByTotal()
    {
        var results = await Context.Orders
            .Where(o => o.Total > 200)
            .Select(o => o.Id)
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 1, 2, 4 }, results);
    }

    [TestMethod]
    public async Task ExpressiveInWhere_FilterByGetGrade()
    {
        var results = await Context.Orders
            .Where(o => o.GetGrade() == "Premium")
            .Select(o => o.Id)
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 1 }, results);
    }

    [TestMethod]
    public async Task ExpressiveInWhere_FilterByBlockBodyMethod()
    {
        var results = await Context.Orders
            .Where(o => o.GetCategory() == "Bulk")
            .Select(o => o.Id)
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 2 }, results);
    }

    // ── Null-conditional [Expressive] ───────────────────────────────────────

    [TestMethod]
    public async Task NullConditionalExpressive_CustomerName_InFilter()
    {
        var results = await Context.Orders
            .Where(o => o.CustomerName == "Alice")
            .Select(o => o.Id)
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 1 }, results);
    }

    [TestMethod]
    public async Task NullConditionalExpressive_CustomerName_NullResult()
    {
        var results = await Context.Orders
            .Where(o => o.CustomerName == null)
            .Select(o => o.Id)
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 3, 4 }, results);
    }

    [TestMethod]
    public async Task NullConditionalExpressive_MultiLevelChain_CustomerCountry()
    {
        var results = await Context.Orders
            .Select(o => new { o.Id, o.CustomerCountry })
            .OrderBy(x => x.Id)
            .ToListAsync();

        Assert.AreEqual("US", results[0].CustomerCountry);
        Assert.AreEqual("UK", results[1].CustomerCountry);
        Assert.IsNull(results[2].CustomerCountry);
        Assert.IsNull(results[3].CustomerCountry);
    }

    // ── [Expressive] extension method (enum expansion) ──────────────────────

    [TestMethod]
    public async Task ExpressiveExtensionMethod_StatusDescription()
    {
        var results = await Context.Orders
            .Select(o => new { o.Id, o.StatusDescription })
            .OrderBy(x => x.Id)
            .ToListAsync();

        Assert.AreEqual("Order approved", results[0].StatusDescription);
        Assert.AreEqual("Awaiting processing", results[1].StatusDescription);
        Assert.AreEqual("Order rejected", results[2].StatusDescription);
    }

    [TestMethod]
    public async Task ExpressiveExtensionMethod_InWhere()
    {
        var results = await Context.Orders
            .Where(o => o.StatusDescription == "Awaiting processing")
            .Select(o => o.Id)
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 2, 4 }, results);
    }

    // ── Captured variable + [Expressive] combined ───────────────────────────

    [TestMethod]
    public async Task CapturedVariable_WithExpressive_InSameQuery()
    {
        var minTotal = 200.0;

        var results = await Context.Orders.AsQueryable()
            .AsExpressive()
            .Where(o => o.Total > minTotal)
            .Select(o => o.Id)
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 1, 2, 4 }, results);
    }

    [TestMethod]
    public async Task CapturedVariable_WithExpressiveSelect_InChain()
    {
        var tag = "RUSH";

        var results = await Context.Orders.AsQueryable()
            .AsExpressive()
            .Where(o => o.Tag == tag)
            .Select(o => o.Total)
            .ToListAsync();

        Assert.AreEqual(240.0, results.Single());
    }

    // ── [Expressive] string interpolation ───────────────────────────────────

    [TestMethod]
    public async Task ExpressiveStringInterpolation_Summary()
    {
        var results = await Context.Orders
            .Select(o => o.Summary)
            .OrderBy(s => s)
            .ToListAsync();

        Assert.IsTrue(results.Any(s => s.Contains("RUSH")));
        Assert.IsTrue(results.Any(s => s.Contains("N/A")));
    }

    // ── [Expressive] constructor projection ─────────────────────────────────

    [TestMethod]
    public async Task ExpressiveConstructor_OrderDto_ProjectsCorrectly()
    {
        var results = await Context.Orders
            .Select(o => new OrderDto(o.Id, o.Tag ?? "N/A", o.Total))
            .OrderBy(d => d.Id)
            .ToListAsync();

        Assert.AreEqual(4, results.Count);
        Assert.AreEqual("RUSH", results[0].Description);
        Assert.AreEqual(240.0, results[0].Total);
        Assert.AreEqual("N/A", results[2].Description);
    }

    // ── Multiple [Expressive] in single projection ──────────────────────────

    [TestMethod]
    public async Task MultipleExpressives_InSingleSelect()
    {
        var results = await Context.Orders
            .Select(o => new
            {
                o.Id,
                o.Total,
                Grade = o.GetGrade(),
                Category = o.GetCategory(),
                o.CustomerName,
                o.TagLength,
            })
            .OrderBy(x => x.Id)
            .ToListAsync();

        Assert.AreEqual(240.0, results[0].Total);
        Assert.AreEqual("Premium", results[0].Grade);
        Assert.AreEqual("Regular", results[0].Category);
        Assert.AreEqual("Alice", results[0].CustomerName);
        Assert.AreEqual(4, results[0].TagLength);

        Assert.AreEqual(30.0, results[2].Total);
        Assert.AreEqual("Budget", results[2].Grade);
        Assert.IsNull(results[2].CustomerName);
        Assert.IsNull(results[2].TagLength);
    }
}
