using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Tests.Common;

/// <summary>
/// Verifies that the polyfill interceptor correctly handles captured (outer-scope)
/// variables when lambdas are rewritten from delegates to expression trees.
/// </summary>
[TestClass]
public class CapturedVariableTests
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

        // Seed via tracked entities (avoiding navigation conflicts)
        _context.Set<Address>().AddRange(SeedData.Addresses);
        await _context.SaveChangesAsync();
        foreach (var c in SeedData.Customers)
            _context.Customers.Add(new Customer { Id = c.Id, Name = c.Name, Email = c.Email, AddressId = c.AddressId });
        await _context.SaveChangesAsync();
        foreach (var o in SeedData.Orders)
            _context.Orders.Add(new Order { Id = o.Id, Tag = o.Tag, Price = o.Price, Quantity = o.Quantity, Status = o.Status, CustomerId = o.CustomerId });
        await _context.SaveChangesAsync();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        await _context.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [TestMethod]
    public async Task Where_CapturedParameter_TranslatesToSql()
    {
        double minPrice = 50;

        var results = await _context.Orders.AsQueryable()
            .AsExpressive()
            .Where(o => o.Price > minPrice)
            .Select(o => o.Id)
            .ToListAsync();

        // Orders: 120 > 50 ✓, 75 > 50 ✓, 10 > 50 ✗, 50 > 50 ✗
        CollectionAssert.AreEquivalent(new[] { 1, 2 }, results);
    }

    [TestMethod]
    public async Task Where_CapturedLocal_TranslatesToSql()
    {
        var tag = "RUSH";

        var results = await _context.Orders.AsQueryable()
            .AsExpressive()
            .Where(o => o.Tag == tag)
            .Select(o => o.Id)
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 1 }, results);
    }

    [TestMethod]
    public async Task Where_CapturedVariable_ChainedWithSelect_TranslatesToSql()
    {
        double minPrice = 50;

        var results = await _context.Orders.AsQueryable()
            .AsExpressive()
            .Where(o => o.Price > minPrice)
            .Select(o => o.Price)
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 120.0, 75.0 }, results);
    }

    [TestMethod]
    public async Task Polyfill_CapturedVariable_ProducesCorrectExpression()
    {
        double threshold = 75;

        var expr = ExpressionPolyfill.Create((Order o) => o.Price > threshold);

        var results = await _context.Orders.Where(expr).Select(o => o.Id).ToListAsync();

        // Only order 1 (120) is > 75
        CollectionAssert.AreEquivalent(new[] { 1 }, results);
    }

    [TestMethod]
    public async Task Where_CapturedInstanceField_TranslatesToSql()
    {
        // Exercises the `this` capture path: _minPrice is accessed via this._minPrice,
        // which the compiler stores in the closure under a generated field name.
        var helper = new InstanceFieldCaptureHelper(_context, 50);
        var results = await helper.GetExpensiveOrderIds();

        // Orders: 120 > 50 ✓, 75 > 50 ✓, 10 > 50 ✗, 50 > 50 ✗
        CollectionAssert.AreEquivalent(new[] { 1, 2 }, results);
    }

    [TestMethod]
    public async Task Where_CapturedLoopVariable_TranslatesToSql()
    {
        // Exercises nested closures: the loop variable lives on a different closure
        // level than outer-scope variables, causing the compiler to generate
        // nested display classes.
        var tags = new[] { "RUSH", "STD" };
        var allResults = new List<int>();

        foreach (var tag in tags)
        {
            var results = await _context.Orders.AsQueryable()
                .AsExpressive()
                .Where(o => o.Tag == tag)
                .Select(o => o.Id)
                .ToListAsync();
            allResults.AddRange(results);
        }

        // RUSH → Order 1, STD → Order 2
        CollectionAssert.AreEquivalent(new[] { 1, 2 }, allResults);
    }

    [TestMethod]
    public async Task Where_MultipleCapturedVariables_TranslatesToSql()
    {
        double minPrice = 10;
        double maxPrice = 100;

        var results = await _context.Orders.AsQueryable()
            .AsExpressive()
            .Where(o => o.Price > minPrice && o.Price < maxPrice)
            .Select(o => o.Id)
            .ToListAsync();

        // 120 out of range, 75 ✓, 10 ✗ (not >), 50 ✓
        CollectionAssert.AreEquivalent(new[] { 2, 4 }, results);
    }

    [TestMethod]
    public async Task Where_CapturedNullableString_TranslatesToSql()
    {
        string? expectedTag = null;

        var results = await _context.Orders.AsQueryable()
            .AsExpressive()
            .Where(o => o.Tag == expectedTag)
            .Select(o => o.Id)
            .ToListAsync();

        // Only order 3 has null tag
        CollectionAssert.AreEquivalent(new[] { 3 }, results);
    }

    [TestMethod]
    public async Task Select_CapturedVariable_InProjection()
    {
        var multiplier = 2.0;

        var results = await _context.Orders.AsQueryable()
            .AsExpressive()
            .Where(o => o.Id == 1)
            .Select(o => o.Price * multiplier)
            .ToListAsync();

        Assert.AreEqual(240.0, results.Single());
    }

    [TestMethod]
    public async Task Where_CapturedEnum_TranslatesToSql()
    {
        var status = OrderStatus.Pending;

        var results = await _context.Orders.AsQueryable()
            .AsExpressive()
            .Where(o => o.Status == status)
            .Select(o => o.Id)
            .ToListAsync();

        // Orders 2 and 4 are Pending
        CollectionAssert.AreEquivalent(new[] { 2, 4 }, results);
    }

    [TestMethod]
    public async Task Where_CapturedVariable_WithNullConditional_InSelect()
    {
        // The original scenario from the bug report: captured variable in Where
        // combined with ?. in Select — both lambdas are intercepted.
        var minPrice = 50.0;

        var results = await _context.Orders.AsQueryable()
            .AsExpressive()
            .Where(o => o.Price > minPrice)
            .Select(o => o.Tag ?? "N/A")
            .ToListAsync();

        // Orders 1 (RUSH) and 2 (STD) match
        CollectionAssert.AreEquivalent(new[] { "RUSH", "STD" }, results);
    }

    [TestMethod]
    public async Task Where_CapturedInstanceProperty_TranslatesToSql()
    {
        var helper = new InstancePropertyCaptureHelper(_context, "STD");
        var results = await helper.GetOrderIds();

        CollectionAssert.AreEquivalent(new[] { 2 }, results);
    }

    [TestMethod]
    public async Task Where_CapturedInstanceField_WithMultipleFields()
    {
        var helper = new MultiFieldCaptureHelper(_context, 10, 100);
        var results = await helper.GetOrderIdsInRange();

        // 120 out of range, 75 ✓, 10 ✗ (not >), 50 ✓
        CollectionAssert.AreEquivalent(new[] { 2, 4 }, results);
    }

    [TestMethod]
    public async Task OrderBy_CapturedVariable_InKeySelector()
    {
        // Captured variable used in a non-Where LINQ method
        var descending = -1;

        var results = await _context.Orders.AsQueryable()
            .AsExpressive()
            .OrderBy(o => o.Price * descending)
            .Select(o => o.Id)
            .ToListAsync();

        // Sorted by Price descending: 120, 75, 50, 10
        Assert.AreEqual(1, results[0]);
        Assert.AreEqual(2, results[1]);
    }

    /// <summary>
    /// Helper that captures <c>this._minPrice</c> (an instance field) in a lambda.
    /// Must be internal (not private) because the generated interceptor references the type.
    /// </summary>
    internal class InstanceFieldCaptureHelper
    {
        private readonly IntegrationTestDbContext _context;
        private readonly double _minPrice;

        public InstanceFieldCaptureHelper(IntegrationTestDbContext context, double minPrice)
        {
            _context = context;
            _minPrice = minPrice;
        }

        public async Task<List<int>> GetExpensiveOrderIds()
        {
            return await _context.Orders.AsQueryable()
                .AsExpressive()
                .Where(o => o.Price > _minPrice)
                .Select(o => o.Id)
                .ToListAsync();
        }
    }

    internal class InstancePropertyCaptureHelper
    {
        private readonly IntegrationTestDbContext _context;
        private string TargetTag { get; }

        public InstancePropertyCaptureHelper(IntegrationTestDbContext context, string tag)
        {
            _context = context;
            TargetTag = tag;
        }

        public async Task<List<int>> GetOrderIds()
        {
            return await _context.Orders.AsQueryable()
                .AsExpressive()
                .Where(o => o.Tag == TargetTag)
                .Select(o => o.Id)
                .ToListAsync();
        }
    }

    internal class MultiFieldCaptureHelper
    {
        private readonly IntegrationTestDbContext _context;
        private readonly double _min;
        private readonly double _max;

        public MultiFieldCaptureHelper(IntegrationTestDbContext context, double min, double max)
        {
            _context = context;
            _min = min;
            _max = max;
        }

        public async Task<List<int>> GetOrderIdsInRange()
        {
            return await _context.Orders.AsQueryable()
                .AsExpressive()
                .Where(o => o.Price > _min && o.Price < _max)
                .Select(o => o.Id)
                .ToListAsync();
        }
    }
}
