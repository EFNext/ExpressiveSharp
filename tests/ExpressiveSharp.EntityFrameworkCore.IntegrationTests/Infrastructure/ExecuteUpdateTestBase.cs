#if !NET10_0_OR_GREATER
using ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

/// <summary>
/// Integration tests for <c>ExecuteUpdate</c> via <c>IRewritableQueryable</c>.
/// Proves that modern C# syntax (switch expressions, null-coalescing) inside
/// <c>SetProperty</c> value lambdas translates to real SQL — a capability
/// impossible with normal C# expression trees.
///
/// EF Core 10 changed the bulk-update API, so this base is conditional.
/// </summary>
public abstract class ExecuteUpdateTestBase : EFCoreRelationalTestBase
{
    [TestInitialize]
    public async Task SeedProducts()
    {
        Context.Products.AddRange(
            new Product { Id = 1, Name = "Widget", Category = "A", Tag = "", Price = 150, Quantity = 10 },
            new Product { Id = 2, Name = "Gadget", Category = "B", Tag = "", Price = 75, Quantity = 5 },
            new Product { Id = 3, Name = "Doohickey", Category = null, Tag = "", Price = 30, Quantity = 20 });
        await Context.SaveChangesAsync();
    }

    [TestMethod]
    public void ExecuteUpdate_BasicConstant_Works()
    {
        var affected = Context.ExpressiveProducts
            .ExecuteUpdate(s => s.SetProperty(p => p.Tag, "basic"));

        Assert.AreEqual(3, affected);

        var products = Context.Products.AsNoTracking().OrderBy(p => p.Id).ToList();
        Assert.AreEqual("basic", products[0].Tag);
        Assert.AreEqual("basic", products[1].Tag);
        Assert.AreEqual("basic", products[2].Tag);
    }

    [TestMethod]
    public void ExecuteUpdate_SwitchExpression_TranslatesToSql()
    {
        Context.ExpressiveProducts
            .ExecuteUpdate(s => s.SetProperty(
                p => p.Tag,
                p => p.Price switch
                {
                    > 100 => "premium",
                    > 50 => "standard",
                    _ => "budget",
                }));

        var products = Context.Products.AsNoTracking().OrderBy(p => p.Id).ToList();
        Assert.AreEqual("premium", products[0].Tag);   // Price=150
        Assert.AreEqual("standard", products[1].Tag);  // Price=75
        Assert.AreEqual("budget", products[2].Tag);    // Price=30
    }

    [TestMethod]
    public void ExecuteUpdate_NullCoalescing_TranslatesToSql()
    {
        Context.ExpressiveProducts
            .ExecuteUpdate(s => s.SetProperty(
                p => p.Tag,
                p => p.Category ?? "UNKNOWN"));

        var products = Context.Products.AsNoTracking().OrderBy(p => p.Id).ToList();
        Assert.AreEqual("A", products[0].Tag);
        Assert.AreEqual("B", products[1].Tag);
        Assert.AreEqual("UNKNOWN", products[2].Tag);
    }

    [TestMethod]
    public void ExecuteUpdate_MultipleProperties_WithModernSyntax()
    {
        Context.ExpressiveProducts
            .ExecuteUpdate(s => s
                .SetProperty(p => p.Tag, p => p.Price switch
                {
                    > 100 => "expensive",
                    _ => "moderate",
                })
                .SetProperty(p => p.Category, "updated"));

        var products = Context.Products.AsNoTracking().OrderBy(p => p.Id).ToList();
        Assert.AreEqual("expensive", products[0].Tag);
        Assert.AreEqual("updated", products[0].Category);
        Assert.AreEqual("moderate", products[1].Tag);
        Assert.AreEqual("updated", products[1].Category);
        Assert.AreEqual("moderate", products[2].Tag);
        Assert.AreEqual("updated", products[2].Category);
    }

    [TestMethod]
    public async Task ExecuteUpdateAsync_SwitchExpression_TranslatesToSql()
    {
        await Context.ExpressiveProducts
            .ExecuteUpdateAsync(s => s.SetProperty(
                p => p.Tag,
                p => p.Price switch
                {
                    > 100 => "premium",
                    > 50 => "standard",
                    _ => "budget",
                }));

        var products = await Context.Products.AsNoTracking().OrderBy(p => p.Id).ToListAsync();
        Assert.AreEqual("premium", products[0].Tag);
        Assert.AreEqual("standard", products[1].Tag);
        Assert.AreEqual("budget", products[2].Tag);
    }
}
#endif
