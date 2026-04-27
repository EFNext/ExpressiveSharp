using System.Linq.Expressions;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

// Each test isolates exactly one transformer so failures point directly at the
// transformer by name. Covers ConvertLoopsToLinq, FlattenBlockExpressions,
// FlattenTupleComparisons, RemoveNullConditionalPatterns.
public abstract class TransformerBehaviorTestBase : EFCoreRelationalTestBase
{
    [TestInitialize]
    public Task SeedStoreData() => Context.SeedStoreAsync();

    [TestMethod]
    public async Task ConvertLoopsToLinq_EnablesForeachInExpressive()
    {
        // Order.ItemCount() is a block-body [Expressive] using foreach:
        //   var count = 0;
        //   foreach (var item in Items) { count++; }
        //   return count;
        // ConvertLoopsToLinq must rewrite the Expression.Loop into
        // Items.Count() before EF Core sees it — otherwise EF Core throws
        // on the LoopExpression node.
        Expression<Func<Order, int>> expr = o => o.ItemCount();
        var expanded = (Expression<Func<Order, int>>)expr.ExpandExpressives();

        var results = await Context.Orders
            .OrderBy(o => o.Id)
            .Select(expanded)
            .ToListAsync();

        // Orders 1–4 have 2, 1, 0, 1 line items
        CollectionAssert.AreEqual(new[] { 2, 1, 0, 1 }, results);
    }

    [TestMethod]
    public async Task FlattenBlockExpressions_InlinesLocalVariable()
    {
        // Order.GetCategory() is a block-body [Expressive] with a local:
        //   var threshold = Quantity * 10;
        //   return threshold > 100 ? "Bulk" : "Regular";
        // FlattenBlockExpressions must inline `threshold` at its use site —
        // otherwise EF Core's funcletizer throws on the BlockExpression.
        Expression<Func<Order, string>> expr = o => o.GetCategory();
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var results = await Context.Orders
            .OrderBy(o => o.Id)
            .Select(expanded)
            .ToListAsync();

        // Quantity * 10: 20, 200, 30, 50 → Regular, Bulk, Regular, Regular
        CollectionAssert.AreEqual(
            new[] { "Regular", "Bulk", "Regular", "Regular" },
            results);
    }

    [TestMethod]
    public async Task FlattenTupleComparisons_RewritesTupleEquality()
    {
        // Order.IsPriceQuantityMatch is:
        //   (Price, Quantity) == (50.0, 5)
        // FlattenTupleComparisons must rewrite this to the flat form
        //   Price == 50.0 && Quantity == 5
        // because EF Core cannot translate ValueTuple equality directly.
        Expression<Func<Order, bool>> expr = o => o.IsPriceQuantityMatch;
        var expanded = (Expression<Func<Order, bool>>)expr.ExpandExpressives();

        var results = await Context.Orders
            .OrderBy(o => o.Id)
            .Select(expanded)
            .ToListAsync();

        // Only Order 4 has Price=50 && Quantity=5
        CollectionAssert.AreEqual(new[] { false, false, false, true }, results);
    }

    [TestMethod]
    public async Task RemoveNullConditionalPatterns_StripsQuestionDot()
    {
        // Order.CustomerName is:
        //   Customer?.Name
        // The generator emits this as a ternary:
        //   Customer == null ? null : Customer.Name
        // RemoveNullConditionalPatterns must strip the null-check for
        // providers that treat nullable navigation as a LEFT JOIN (EF Core
        // already short-circuits against null). Without the transformer,
        // EF Core may emit redundant CASE/IS NULL clauses or fail.
        Expression<Func<Order, string?>> expr = o => o.CustomerName;
        var expanded = (Expression<Func<Order, string?>>)expr.ExpandExpressives();

        var results = await Context.Orders
            .OrderBy(o => o.Id)
            .Select(expanded)
            .ToListAsync();

        // Order 1 → Alice, Order 2 → Bob, Order 3 → null (no customer),
        // Order 4 → null (customer with null Name)
        CollectionAssert.AreEqual(new[] { "Alice", "Bob", null, null }, results);
    }
}
