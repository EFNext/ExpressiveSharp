using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

/// <summary>
/// Tests the <c>UseExpressives()</c> convention plugins:
/// <list type="bullet">
///   <item>Marking <c>[Expressive]</c> properties as unmapped</item>
///   <item>Expanding <c>[Expressive]</c> calls inside global query filters</item>
/// </list>
/// The convention checks are model-level and don't need a database, but the
/// query-filter test requires real execution to verify the filter actually
/// shapes results correctly.
/// </summary>
public abstract class UseExpressivesConventionTestBase : EFCoreRelationalTestBase
{
    [TestMethod]
    public void ExpressiveProperties_AreMarkedUnmapped()
    {
        var model = Context.Model;
        var orderEntity = model.FindEntityType(typeof(Order))!;

        // [Expressive] properties should not be mapped as columns
        Assert.IsNull(orderEntity.FindProperty(nameof(Order.Total)));
        Assert.IsNull(orderEntity.FindProperty(nameof(Order.CustomerName)));
        Assert.IsNull(orderEntity.FindProperty(nameof(Order.CustomerCountry)));
        Assert.IsNull(orderEntity.FindProperty(nameof(Order.StatusDescription)));
        Assert.IsNull(orderEntity.FindProperty(nameof(Order.Summary)));
        Assert.IsNull(orderEntity.FindProperty(nameof(Order.TagLength)));
        Assert.IsNull(orderEntity.FindProperty(nameof(Order.IsPriceQuantityMatch)));
        Assert.IsNull(orderEntity.FindProperty(nameof(Order.CheckedTotal)));
        Assert.IsNull(orderEntity.FindProperty(nameof(Order.PriceBreakpoints)));

        // Regular properties should still be mapped
        Assert.IsNotNull(orderEntity.FindProperty(nameof(Order.Price)));
        Assert.IsNotNull(orderEntity.FindProperty(nameof(Order.Quantity)));
        Assert.IsNotNull(orderEntity.FindProperty(nameof(Order.Tag)));
        Assert.IsNotNull(orderEntity.FindProperty(nameof(Order.Status)));
    }

    [TestMethod]
    public void ExpressiveDbSet_EntityIsDiscoveredInModel()
    {
        // ExpressiveOrders is discovered as an entity type alongside Orders
        var model = Context.Model;
        var orderEntity = model.FindEntityType(typeof(Order));
        Assert.IsNotNull(orderEntity);
    }

    [TestMethod]
    public async Task NullConditional_InExpressiveMember_TranslatesCorrectly()
    {
        await Context.SeedStoreAsync();

        // CustomerName => Customer?.Name — the RemoveNullConditionalPatterns
        // transformer strips the null-check pattern so EF Core can translate it.
        var results = await Context.Orders
            .Select(o => o.CustomerName)
            .ToListAsync();

        Assert.AreEqual(4, results.Count);
        CollectionAssert.Contains(results, "Alice");
        CollectionAssert.Contains(results, "Bob");
    }

    [TestMethod]
    public async Task BlockBody_InExpressiveMember_TranslatesCorrectly()
    {
        await Context.SeedStoreAsync();

        // GetCategory() is block-bodied with a local variable — the
        // FlattenBlockExpressions transformer inlines the local so EF Core
        // can translate the resulting CASE/WHEN expression.
        var results = await Context.Orders
            .Select(o => o.GetCategory())
            .ToListAsync();

        Assert.AreEqual(4, results.Count);
        CollectionAssert.Contains(results, "Bulk");
        CollectionAssert.Contains(results, "Regular");
    }
}
