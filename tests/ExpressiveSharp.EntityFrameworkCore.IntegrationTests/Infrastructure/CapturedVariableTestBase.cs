using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

/// <summary>
/// Verifies that the polyfill interceptor correctly handles captured (outer-scope)
/// variables when lambdas are rewritten from delegates to expression trees.
/// </summary>
public abstract class CapturedVariableTestBase : EFCoreRelationalTestBase
{
    [TestInitialize]
    public Task SeedStoreData() => Context.SeedStoreAsync();

    [TestMethod]
    public async Task Where_CapturedParameter_TranslatesToSql()
    {
        double minPrice = 50;

        var results = await Context.Orders.AsQueryable()
            .AsExpressive()
            .Where(o => o.Price > minPrice)
            .Select(o => o.Id)
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 1, 2 }, results);
    }

    [TestMethod]
    public async Task Where_CapturedLocal_TranslatesToSql()
    {
        var tag = "RUSH";

        var results = await Context.Orders.AsQueryable()
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

        var results = await Context.Orders.AsQueryable()
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

        var results = await Context.Orders.Where(expr).Select(o => o.Id).ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 1 }, results);
    }

    [TestMethod]
    public async Task Where_CapturedInstanceField_TranslatesToSql()
    {
        var helper = new InstanceFieldCaptureHelper(Context, 50);
        var results = await helper.GetExpensiveOrderIds();

        CollectionAssert.AreEquivalent(new[] { 1, 2 }, results);
    }

    [TestMethod]
    public async Task Where_CapturedLoopVariable_TranslatesToSql()
    {
        var tags = new[] { "RUSH", "STD" };
        var allResults = new List<int>();

        foreach (var tag in tags)
        {
            var results = await Context.Orders.AsQueryable()
                .AsExpressive()
                .Where(o => o.Tag == tag)
                .Select(o => o.Id)
                .ToListAsync();
            allResults.AddRange(results);
        }

        CollectionAssert.AreEquivalent(new[] { 1, 2 }, allResults);
    }

    [TestMethod]
    public async Task Where_MultipleCapturedVariables_TranslatesToSql()
    {
        double minPrice = 10;
        double maxPrice = 100;

        var results = await Context.Orders.AsQueryable()
            .AsExpressive()
            .Where(o => o.Price > minPrice && o.Price < maxPrice)
            .Select(o => o.Id)
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 2, 4 }, results);
    }

    [TestMethod]
    public async Task Where_CapturedNullableString_TranslatesToSql()
    {
        string? expectedTag = null;

        var results = await Context.Orders.AsQueryable()
            .AsExpressive()
            .Where(o => o.Tag == expectedTag)
            .Select(o => o.Id)
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 3 }, results);
    }

    [TestMethod]
    public async Task Select_CapturedVariable_InProjection()
    {
        var multiplier = 2.0;

        var results = await Context.Orders.AsQueryable()
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

        var results = await Context.Orders.AsQueryable()
            .AsExpressive()
            .Where(o => o.Status == status)
            .Select(o => o.Id)
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 2, 4 }, results);
    }

    [TestMethod]
    public async Task Where_CapturedVariable_WithNullConditional_InSelect()
    {
        var minPrice = 50.0;

        var results = await Context.Orders.AsQueryable()
            .AsExpressive()
            .Where(o => o.Price > minPrice)
            .Select(o => o.Tag ?? "N/A")
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { "RUSH", "STD" }, results);
    }

    [TestMethod]
    public async Task Where_CapturedInstanceProperty_TranslatesToSql()
    {
        var helper = new InstancePropertyCaptureHelper(Context, "STD");
        var results = await helper.GetOrderIds();

        CollectionAssert.AreEquivalent(new[] { 2 }, results);
    }

    [TestMethod]
    public async Task Where_CapturedInstanceField_WithMultipleFields()
    {
        var helper = new MultiFieldCaptureHelper(Context, 10, 100);
        var results = await helper.GetOrderIdsInRange();

        CollectionAssert.AreEquivalent(new[] { 2, 4 }, results);
    }

    [TestMethod]
    public async Task OrderBy_CapturedVariable_InKeySelector()
    {
        var descending = -1;

        var results = await Context.Orders.AsQueryable()
            .AsExpressive()
            .OrderBy(o => o.Price * descending)
            .Select(o => o.Id)
            .ToListAsync();

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
