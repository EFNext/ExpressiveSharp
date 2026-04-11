using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using ExpressiveSharp.MongoDB.IntegrationTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver.Linq;

namespace ExpressiveSharp.MongoDB.IntegrationTests.Tests;

/// <summary>
/// Verifies that delegate-based lambdas with modern C# syntax are rewritten to
/// expression trees by the source generator and correctly forwarded through
/// the MongoDB provider. These features would normally fail in raw expression trees.
/// </summary>
[TestClass]
public class PolyfillInterceptorTests : MongoTestBase
{
    // ── Null-conditional (?.) ───────────────────────────────────────────

    [TestMethod]
    public async Task NullConditional_PropertyAccess()
    {
        var results = await Query
            .OrderBy(o => o.Id)
            .Select(o => o.Customer?.Name)
            .ToListAsync();

        Assert.AreEqual("Alice", results[0]);
        Assert.AreEqual("Bob", results[1]);
        Assert.IsNull(results[2]); // Order 3: no customer
        Assert.IsNull(results[3]); // Order 4: customer with null name
    }

    [TestMethod]
    public async Task NullConditional_ChainedAccess()
    {
        var results = await Query
            .OrderBy(o => o.Id)
            .Select(o => o.Customer?.Address?.Country)
            .ToListAsync();

        Assert.AreEqual("US", results[0]);
        Assert.AreEqual("UK", results[1]);
        Assert.IsNull(results[2]); // No customer
        Assert.IsNull(results[3]); // Customer has no address
    }

    [TestMethod]
    public async Task NullConditional_MethodCall()
    {
        var results = await Query
            .OrderBy(o => o.Id)
            .Select(o => o.Tag?.ToUpper())
            .ToListAsync();

        Assert.AreEqual("RUSH", results[0]);
        Assert.AreEqual("STD", results[1]);
        Assert.IsNull(results[2]);
        Assert.AreEqual("SPECIAL", results[3]);
    }

    [TestMethod]
    public async Task NullConditional_InWherePredicate()
    {
        var results = await Query
            .Where(o => o.Customer?.Address?.Country == "US")
            .Select(o => o.Id)
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 1 }, results);
    }

    // ── Null-coalescing (??) ────────────────────────────────────────────

    [TestMethod]
    public async Task NullCoalescing_WithFallback()
    {
        var results = await Query
            .OrderBy(o => o.Id)
            .Select(o => o.Tag ?? "NONE")
            .ToListAsync();

        CollectionAssert.AreEqual(new[] { "RUSH", "STD", "NONE", "SPECIAL" }, results);
    }

    [TestMethod]
    public async Task NullCoalescing_ChainedWithNullConditional()
    {
        var results = await Query
            .OrderBy(o => o.Id)
            .Select(o => o.Customer?.Name ?? "Unknown")
            .ToListAsync();

        CollectionAssert.AreEqual(new[] { "Alice", "Bob", "Unknown", "Unknown" }, results);
    }

    // ── Switch expressions ──────────────────────────────────────────────

    [TestMethod]
    public async Task SwitchExpression_OnEnum()
    {
        var results = await Query
            .OrderBy(o => o.Id)
            .Select(o => o.Status switch
            {
                OrderStatus.Approved => "OK",
                OrderStatus.Pending => "WAIT",
                _ => "NO",
            })
            .ToListAsync();

        // Order 1: Approved, Order 2: Pending, Order 3: Rejected, Order 4: Pending
        CollectionAssert.AreEqual(new[] { "OK", "WAIT", "NO", "WAIT" }, results);
    }

    [TestMethod]
    public async Task SwitchExpression_OnNumericRange()
    {
        var results = await Query
            .OrderBy(o => o.Id)
            .Select(o => o.Price switch
            {
                >= 100 => "Premium",
                >= 50 => "Standard",
                _ => "Budget",
            })
            .ToListAsync();

        // Prices: 120, 75, 10, 50
        CollectionAssert.AreEqual(new[] { "Premium", "Standard", "Budget", "Standard" }, results);
    }

    // ── Pattern matching ────────────────────────────────────────────────

    [TestMethod]
    public async Task PatternMatching_IsNotNull()
    {
        var results = await Query
            .Where(o => o.Customer is not null)
            .OrderBy(o => o.Id)
            .Select(o => o.Id)
            .ToListAsync();

        CollectionAssert.AreEqual(new[] { 1, 2, 4 }, results);
    }

    [TestMethod]
    public async Task PatternMatching_IsNull()
    {
        var results = await Query
            .Where(o => o.Customer is null)
            .Select(o => o.Id)
            .ToListAsync();

        CollectionAssert.AreEquivalent(new[] { 3 }, results);
    }

    // ── Conditional (ternary) ───────────────────────────────────────────

    [TestMethod]
    public async Task Ternary_InSelect()
    {
        var results = await Query
            .OrderBy(o => o.Id)
            .Select(o => o.Price > 50 ? "Expensive" : "Affordable")
            .ToListAsync();

        // Prices: 120(>50), 75(>50), 10(≤50), 50(≤50)
        CollectionAssert.AreEqual(
            new[] { "Expensive", "Expensive", "Affordable", "Affordable" }, results);
    }

    // ── Compound queries ────────────────────────────────────────────────

    [TestMethod]
    public async Task Compound_NullConditional_WithCoalescing_InWhere()
    {
        // Filter orders where the customer's country is unknown (null) then default
        var results = await Query
            .Where(o => (o.Customer?.Address?.Country ?? "Unknown") == "Unknown")
            .OrderBy(o => o.Id)
            .Select(o => o.Id)
            .ToListAsync();

        // Order 3: no customer, Order 4: customer but no address
        CollectionAssert.AreEqual(new[] { 3, 4 }, results);
    }

    [TestMethod]
    public async Task Compound_SwitchExpression_InWhere()
    {
        var results = await Query
            .Where(o => (o.Status switch
            {
                OrderStatus.Approved => 1,
                OrderStatus.Pending => 2,
                _ => 0,
            }) == 2)
            .OrderBy(o => o.Price)
            .Select(o => o.Id)
            .ToListAsync();

        // Pending orders: 2 (75), 4 (50) → sorted by price: 4, 2
        CollectionAssert.AreEqual(new[] { 4, 2 }, results);
    }
}
