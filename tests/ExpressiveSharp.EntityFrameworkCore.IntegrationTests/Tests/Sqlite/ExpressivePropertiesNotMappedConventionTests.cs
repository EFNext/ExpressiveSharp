using ExpressiveSharp.Mapping;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.Sqlite;

[TestClass]
public class ExpressivePropertiesNotMappedConventionTests
{
    private SqliteConnection _connection = null!;

    [TestInitialize]
    public void Setup()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
    }

    [TestCleanup]
    public void Cleanup() => _connection.Dispose();

    private DbContextOptions<TContext> CreateOptions<TContext>() where TContext : DbContext
        => new DbContextOptionsBuilder<TContext>()
            .UseSqlite(_connection)
            .UseExpressives()
            .Options;

    [TestMethod]
    public void PropertyWithExpressiveAttribute_IsIgnored()
    {
        using var ctx = new NotMappedTestContext(CreateOptions<NotMappedTestContext>());

        var entity = ctx.Model.FindEntityType(typeof(NotMappedItem))!;

        // The [Expressive] property must not appear as a mapped property.
        Assert.IsNull(entity.FindProperty(nameof(NotMappedItem.DoubledValue)),
            "[Expressive] properties should be ignored by the convention.");

        // Baseline: regular scalar properties should still be mapped.
        Assert.IsNotNull(entity.FindProperty(nameof(NotMappedItem.Value)));
    }

    [TestMethod]
    public void PropertyTargetedByExpressiveFor_IsIgnored()
    {
        using var ctx = new NotMappedTestContext(CreateOptions<NotMappedTestContext>());

        var entity = ctx.Model.FindEntityType(typeof(NotMappedItem))!;

        // The property `DescribedValue` is targeted by an [ExpressiveFor] stub
        // declared on the same class (single-arg + instance-stub form).
        Assert.IsNull(entity.FindProperty(nameof(NotMappedItem.DescribedValue)),
            "Properties targeted by [ExpressiveFor] should be ignored by the convention.");
    }

    [TestMethod]
    public void PropertyStubIsIgnored()
    {
        using var ctx = new NotMappedTestContext(CreateOptions<NotMappedTestContext>());

        var entity = ctx.Model.FindEntityType(typeof(NotMappedItem))!;

        // A property decorated with [ExpressiveFor] is itself a stub — it must not be mapped
        // as a column (otherwise EF would try to materialize/persist its value).
        Assert.IsNull(entity.FindProperty(nameof(NotMappedItem.DescribedValueExpression)),
            "Property stubs carrying [ExpressiveFor] should be ignored by the convention.");
    }

    [TestMethod]
    public void PropertyTargetedByExternalExpressiveFor_IsIgnored()
    {
        using var ctx = new NotMappedTestContext(CreateOptions<NotMappedTestContext>());

        var entity = ctx.Model.FindEntityType(typeof(NotMappedItem))!;

        // `ExternalDescribedValue` is targeted by a stub in an unrelated static class —
        // the cross-assembly-style mapping that was already supported for [Expressive].
        Assert.IsNull(entity.FindProperty(nameof(NotMappedItem.ExternalDescribedValue)),
            "Properties targeted by [ExpressiveFor] stubs in other classes should also be ignored.");
    }

    [TestMethod]
    public async Task QueryReferencingIgnoredProperty_UsesExpressiveFormulaNotColumn()
    {
        using var ctx = new NotMappedTestContext(CreateOptions<NotMappedTestContext>());
        ctx.Database.EnsureCreated();

        ctx.Items.Add(new NotMappedItem { Id = 1, Value = 5 });
        ctx.Items.Add(new NotMappedItem { Id = 2, Value = 7 });
        await ctx.SaveChangesAsync();

        // The formula for DoubledValue comes from the [Expressive] getter; no DB column.
        var results = await ctx.Items
            .OrderBy(x => x.Id)
            .Select(x => new { x.Id, x.DoubledValue })
            .ToListAsync();

        Assert.AreEqual(2, results.Count);
        Assert.AreEqual(10, results[0].DoubledValue);
        Assert.AreEqual(14, results[1].DoubledValue);
    }

    public class NotMappedItem
    {
        public int Id { get; set; }
        public int Value { get; set; }

        [Expressive]
        public int DoubledValue => Value * 2;

        // Targeted by a co-located property stub (single-arg [ExpressiveFor] form).
        public string DescribedValue { get; set; } = "";

        [ExpressiveFor(nameof(DescribedValue))]
        public string DescribedValueExpression => "value=" + Value;

        // Targeted by a stub declared in an external static class.
        public string ExternalDescribedValue { get; set; } = "";
    }

    private static class NotMappedItemMappings
    {
        [ExpressiveFor(typeof(NotMappedItem), nameof(NotMappedItem.ExternalDescribedValue))]
        static string ExternalDescribedValue(NotMappedItem item) => "ext=" + item.Value;
    }

    public class NotMappedTestContext : DbContext
    {
        public DbSet<NotMappedItem> Items => Set<NotMappedItem>();

        public NotMappedTestContext(DbContextOptions<NotMappedTestContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotMappedItem>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedNever();
            });
        }
    }
}
