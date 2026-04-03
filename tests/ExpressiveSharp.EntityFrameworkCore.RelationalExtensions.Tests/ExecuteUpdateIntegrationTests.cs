#if !NET10_0_OR_GREATER
using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Tests.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Tests;

/// <summary>
/// End-to-end integration tests for ExecuteUpdate via IRewritableQueryable.
/// These prove that modern C# syntax (null-conditional, switch expressions)
/// inside SetProperty value expressions works with real SQL execution — functionality
/// that is impossible with normal C# expression trees.
/// </summary>
[TestClass]
public class ExecuteUpdateIntegrationTests
{
    private SqliteConnection _connection = null!;

    [TestInitialize]
    public void Setup()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _connection.Dispose();
    }

    private ExecuteUpdateTestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ExecuteUpdateTestDbContext>()
            .UseSqlite(_connection)
            .UseExpressives(o => o.UseRelationalExtensions())
            .Options;
        var ctx = new ExecuteUpdateTestDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    private static void SeedProducts(ExecuteUpdateTestDbContext ctx)
    {
        ctx.Products.AddRange(
            new Product { Id = 1, Name = "Widget", Category = "A", Tag = "", Price = 150, Quantity = 10 },
            new Product { Id = 2, Name = "Gadget", Category = "B", Tag = "", Price = 75, Quantity = 5 },
            new Product { Id = 3, Name = "Doohickey", Category = null, Tag = "", Price = 30, Quantity = 20 });
        ctx.SaveChanges();
    }

    /// <summary>
    /// Basic test: verify the generator intercepts ExecuteUpdate and forwards to EF Core.
    /// </summary>
    [TestMethod]
    public void ExecuteUpdate_BasicConstant_Works()
    {
        using var ctx = CreateContext();
        SeedProducts(ctx);

        var affected = ctx.ExpressiveProducts
            .ExecuteUpdate(s => s.SetProperty(p => p.Tag, "basic"));

        Assert.AreEqual(3, affected);
        // ExecuteUpdate bypasses change tracker — use AsNoTracking to see actual DB state
        var products = ctx.Products.AsNoTracking().OrderBy(p => p.Id).ToList();
        Assert.AreEqual("basic", products[0].Tag);
        Assert.AreEqual("basic", products[1].Tag);
        Assert.AreEqual("basic", products[2].Tag);
    }

    /// <summary>
    /// Proves new capability: switch expression inside SetProperty value lambda.
    /// <c>o.Price switch { > 100 => "premium", > 50 => "standard", _ => "budget" }</c>
    /// is impossible in a normal C# expression tree context.
    /// </summary>
    [TestMethod]
    public void ExecuteUpdate_SwitchExpression_TranslatesToSql()
    {
        using var ctx = CreateContext();
        SeedProducts(ctx);

        ctx.ExpressiveProducts
            .ExecuteUpdate(s => s.SetProperty(
                p => p.Tag,
                p => p.Price switch
                {
                    > 100 => "premium",
                    > 50 => "standard",
                    _ => "budget"
                }));

        var products = ctx.Products.AsNoTracking().OrderBy(p => p.Id).ToList();
        Assert.AreEqual("premium", products[0].Tag);   // Price=150
        Assert.AreEqual("standard", products[1].Tag);   // Price=75
        Assert.AreEqual("budget", products[2].Tag);      // Price=30
    }

    /// <summary>
    /// Proves new capability: null-coalescing operator inside SetProperty value lambda.
    /// </summary>
    [TestMethod]
    public void ExecuteUpdate_NullCoalescing_TranslatesToSql()
    {
        using var ctx = CreateContext();
        SeedProducts(ctx);

        ctx.ExpressiveProducts
            .ExecuteUpdate(s => s.SetProperty(
                p => p.Tag,
                p => p.Category ?? "UNKNOWN"));

        var products = ctx.Products.AsNoTracking().OrderBy(p => p.Id).ToList();
        Assert.AreEqual("A", products[0].Tag);       // Category="A"
        Assert.AreEqual("B", products[1].Tag);       // Category="B"
        Assert.AreEqual("UNKNOWN", products[2].Tag); // Category=null
    }

    /// <summary>
    /// Proves that multiple SetProperty calls with modern C# syntax work together,
    /// producing multiple SET clauses in a single SQL UPDATE statement.
    /// </summary>
    [TestMethod]
    public void ExecuteUpdate_MultipleProperties_WithModernSyntax()
    {
        using var ctx = CreateContext();
        SeedProducts(ctx);

        ctx.ExpressiveProducts
            .ExecuteUpdate(s => s
                .SetProperty(p => p.Tag, p => p.Price switch
                {
                    > 100 => "expensive",
                    _ => "moderate"
                })
                .SetProperty(p => p.Category, "updated"));

        var products = ctx.Products.AsNoTracking().OrderBy(p => p.Id).ToList();
        Assert.AreEqual("expensive", products[0].Tag);    // Price=150
        Assert.AreEqual("updated", products[0].Category);
        Assert.AreEqual("moderate", products[1].Tag);      // Price=75
        Assert.AreEqual("updated", products[1].Category);
        Assert.AreEqual("moderate", products[2].Tag);      // Price=30
        Assert.AreEqual("updated", products[2].Category);
    }

    /// <summary>
    /// Proves async variant works end-to-end with modern C# syntax.
    /// </summary>
    [TestMethod]
    public async Task ExecuteUpdateAsync_SwitchExpression_TranslatesToSql()
    {
        using var ctx = CreateContext();
        SeedProducts(ctx);

        await ctx.ExpressiveProducts
            .ExecuteUpdateAsync(s => s.SetProperty(
                p => p.Tag,
                p => p.Price switch
                {
                    > 100 => "premium",
                    > 50 => "standard",
                    _ => "budget"
                }));

        var products = await ctx.Products.AsNoTracking().OrderBy(p => p.Id).ToListAsync();
        Assert.AreEqual("premium", products[0].Tag);
        Assert.AreEqual("standard", products[1].Tag);
        Assert.AreEqual("budget", products[2].Tag);
    }
}
#endif
