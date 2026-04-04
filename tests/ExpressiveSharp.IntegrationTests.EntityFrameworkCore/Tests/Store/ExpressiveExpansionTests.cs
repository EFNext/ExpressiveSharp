using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Tests.Store;

/// <summary>
/// Regression tests verifying that [Expressive] members are correctly expanded
/// through the ExpressiveQueryCompiler pipeline in real EF Core queries.
/// Covers scenarios that worked in EntityFrameworkCore.Projectables and must
/// continue to work in ExpressiveSharp.
/// </summary>
[TestClass]
public class ExpressiveExpansionTests
{
    private SqliteConnection _connection = null!;
    private IntegrationTestDbContext _context = null!;

    [TestInitialize]
    public async Task Setup()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<IntegrationTestDbContext>()
            .UseSqlite(_connection)
            .UseExpressives()
            .Options;

        _context = new IntegrationTestDbContext(options);
        _context.Database.EnsureCreated();

        _context.Set<Address>().AddRange(SeedData.Addresses);
        await _context.SaveChangesAsync();
        foreach (var c in SeedData.Customers)
            _context.Customers.Add(new Customer { Id = c.Id, Name = c.Name, Email = c.Email, AddressId = c.AddressId });
        await _context.SaveChangesAsync();
        foreach (var o in SeedData.Orders)
            _context.Orders.Add(new Order { Id = o.Id, Tag = o.Tag, Price = o.Price, Quantity = o.Quantity, Status = o.Status, CustomerId = o.CustomerId });
        await _context.SaveChangesAsync();
        _context.Set<LineItem>().AddRange(SeedData.LineItems);
        await _context.SaveChangesAsync();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        await _context.DisposeAsync();
        await _connection.DisposeAsync();
    }

    // ── Nested [Expressive] calls ───────────────────────────────────────────

    [TestMethod]
    public async Task NestedExpressive_TotalUsedInGetGrade_ExpandsRecursively()
    {
        // GetGrade() uses Price (a plain property), but this tests that
        // an [Expressive] member can be used in a projection alongside others.
        var results = await _context.Orders
            .Select(o => new { o.Id, Grade = o.GetGrade(), o.Total })
            .OrderBy(x => x.Id)
            .ToListAsync();

        Assert.AreEqual("Premium", results[0].Grade);  // Order 1: Price 120
        Assert.AreEqual(240.0, results[0].Total);       // 120 * 2
        Assert.AreEqual("Budget", results[2].Grade);    // Order 3: Price 10
    }

    [TestMethod]
    public async Task NestedExpressive_GetOrderSummaryTuple_ExpandsBothGetGradeAndTotal()
    {
        // GetOrderSummaryTuple calls both GetGrade() and Total — double nested expansion.
        // Order by Id first (plain column), then project the tuple.
        var results = await _context.Orders
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

    // ── [Expressive] in Where (filter by computed property) ─────────────────

    [TestMethod]
    public async Task ExpressiveInWhere_FilterByTotal()
    {
        // Filter using an [Expressive] property in the predicate.
        var results = await _context.Orders
            .Where(o => o.Total > 200)
            .Select(o => o.Id)
            .ToListAsync();

        // Order 1: 240 ✓, Order 2: 1500 ✓, Order 3: 30 ✗, Order 4: 250 ✓
        CollectionAssert.AreEquivalent(new[] { 1, 2, 4 }, results);
    }

    [TestMethod]
    public async Task ExpressiveInWhere_FilterByGetGrade()
    {
        // Filter using an [Expressive] method (switch expression) in Where.
        var results = await _context.Orders
            .Where(o => o.GetGrade() == "Premium")
            .Select(o => o.Id)
            .ToListAsync();

        // Only Order 1 (Price 120 >= 100) is Premium
        CollectionAssert.AreEquivalent(new[] { 1 }, results);
    }

    [TestMethod]
    public async Task ExpressiveInWhere_FilterByBlockBodyMethod()
    {
        // Filter using a block-bodied [Expressive] method in Where.
        var results = await _context.Orders
            .Where(o => o.GetCategory() == "Bulk")
            .Select(o => o.Id)
            .ToListAsync();

        // Only Order 2 (Qty 20 * 10 = 200 > 100) is Bulk
        CollectionAssert.AreEquivalent(new[] { 2 }, results);
    }

    // ── Null-conditional [Expressive] in EF Core ────────────────────────────

    [TestMethod]
    public async Task NullConditionalExpressive_CustomerName_InFilter()
    {
        // CustomerName uses ?. chain: Customer?.Name
        var results = await _context.Orders
            .Where(o => o.CustomerName == "Alice")
            .Select(o => o.Id)
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 1 }, results);
    }

    [TestMethod]
    public async Task NullConditionalExpressive_CustomerName_NullResult()
    {
        // Orders without a customer should have null CustomerName
        var results = await _context.Orders
            .Where(o => o.CustomerName == null)
            .Select(o => o.Id)
            .ToListAsync();

        // Order 3 (no customer), Order 4 (customer with null name)
        CollectionAssert.AreEquivalent(new[] { 3, 4 }, results);
    }

    [TestMethod]
    public async Task NullConditionalExpressive_MultiLevelChain_CustomerCountry()
    {
        // CustomerCountry: Customer?.Address?.Country — three-level null chain.
        var results = await _context.Orders
            .Select(o => new { o.Id, o.CustomerCountry })
            .OrderBy(x => x.Id)
            .ToListAsync();

        Assert.AreEqual("US", results[0].CustomerCountry);    // Order 1 → Alice → NY → US
        Assert.AreEqual("UK", results[1].CustomerCountry);    // Order 2 → Bob → London → UK
        Assert.IsNull(results[2].CustomerCountry);             // Order 3 → no customer
        Assert.IsNull(results[3].CustomerCountry);             // Order 4 → customer 3 → no address
    }

    // ── [Expressive] extension method (enum expansion) ──────────────────────

    [TestMethod]
    public async Task ExpressiveExtensionMethod_StatusDescription()
    {
        // StatusDescription uses GetDescription() — an [Expressive] extension method.
        var results = await _context.Orders
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
        var results = await _context.Orders
            .Where(o => o.StatusDescription == "Awaiting processing")
            .Select(o => o.Id)
            .ToListAsync();

        // Orders 2 and 4 are Pending → "Awaiting processing"
        CollectionAssert.AreEquivalent(new[] { 2, 4 }, results);
    }

    // ── Captured variable + [Expressive] combined ───────────────────────────

    [TestMethod]
    public async Task CapturedVariable_WithExpressive_InSameQuery()
    {
        // Captured variable minTotal + [Expressive] Total in the same Where.
        // This exercises both the polyfill interceptor (captured var) and
        // ExpressiveQueryCompiler ([Expressive] expansion) in one query.
        var minTotal = 200.0;

        var results = await _context.Orders.AsQueryable()
            .AsExpressive()
            .Where(o => o.Total > minTotal)
            .Select(o => o.Id)
            .ToListAsync();

        // Total: 240 ✓, 1500 ✓, 30 ✗, 250 ✓
        CollectionAssert.AreEquivalent(new[] { 1, 2, 4 }, results);
    }

    [TestMethod]
    public async Task CapturedVariable_WithExpressiveSelect_InChain()
    {
        // Captured variable in Where + [Expressive] property in Select.
        var tag = "RUSH";

        var results = await _context.Orders.AsQueryable()
            .AsExpressive()
            .Where(o => o.Tag == tag)
            .Select(o => o.Total)
            .ToListAsync();

        Assert.AreEqual(240.0, results.Single()); // Order 1: 120 * 2
    }

    // ── [Expressive] string interpolation ───────────────────────────────────

    [TestMethod]
    public async Task ExpressiveStringInterpolation_Summary()
    {
        // Summary uses string interpolation: $"Order #{Id}: {Tag ?? "N/A"}"
        var results = await _context.Orders
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
        // OrderDto constructor is [Expressive] — maps (id, description, total).
        var results = await _context.Orders
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
        // Projects multiple [Expressive] members in one anonymous type.
        var results = await _context.Orders
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

        // Order 1
        Assert.AreEqual(240.0, results[0].Total);
        Assert.AreEqual("Premium", results[0].Grade);
        Assert.AreEqual("Regular", results[0].Category);
        Assert.AreEqual("Alice", results[0].CustomerName);
        Assert.AreEqual(4, results[0].TagLength);   // "RUSH".Length

        // Order 3 (nulls)
        Assert.AreEqual(30.0, results[2].Total);
        Assert.AreEqual("Budget", results[2].Grade);
        Assert.IsNull(results[2].CustomerName);
        Assert.IsNull(results[2].TagLength);
    }
}
