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
}
