using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.Sqlite;

// Convention-plugin tests; provider-agnostic so SQLite-only.
[TestClass]
public class ExpressiveDbSetDiscoveryTests
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

    private DbContextOptions<TContext> CreateOptionsReversed<TContext>() where TContext : DbContext
        => new DbContextOptionsBuilder<TContext>()
            .UseExpressives()
            .UseSqlite(_connection)
            .Options;

    [TestMethod]
    public void ExpressiveOnlyContext_EntityIsDiscoveredInModel()
    {
        using var ctx = new ExpressiveOnlyContext(CreateOptions<ExpressiveOnlyContext>());
        ctx.Database.EnsureCreated();

        var entityType = ctx.Model.FindEntityType(typeof(DiscoveryItem));
        Assert.IsNotNull(entityType,
            "Entity type should be discovered from ExpressiveDbSet property");
    }

    [TestMethod]
    public void ExpressiveOnlyContext_TableNameMatchesPropertyName()
    {
        using var ctx = new ExpressiveOnlyContext(CreateOptions<ExpressiveOnlyContext>());
        ctx.Database.EnsureCreated();

        var tableName = ctx.Model.FindEntityType(typeof(DiscoveryItem))!.GetTableName();
        Assert.AreEqual("Items", tableName);
    }

    [TestMethod]
    public async Task ExpressiveOnlyContext_QueryExecutesAgainstDiscoveredTable()
    {
        using var ctx = new ExpressiveOnlyContext(CreateOptions<ExpressiveOnlyContext>());
        ctx.Database.EnsureCreated();

        ctx.Items.Add(new DiscoveryItem { Name = "Widget" });
        ctx.Items.Add(new DiscoveryItem { Name = "Gadget" });
        await ctx.SaveChangesAsync();

        var items = await ctx.Items.Where(i => i.Name == "Widget").ToListAsync();
        Assert.AreEqual(1, items.Count);
        Assert.AreEqual("Widget", items[0].Name);
    }

    [TestMethod]
    public void ExpressiveOnlyContext_UseExpressivesBeforeProvider_EntityStillDiscovered()
    {
        // Deferred decoration path: UseExpressives() called before UseSqlite().
        using var ctx = new ExpressiveOnlyContext(CreateOptionsReversed<ExpressiveOnlyContext>());
        ctx.Database.EnsureCreated();

        var entityType = ctx.Model.FindEntityType(typeof(DiscoveryItem));
        Assert.IsNotNull(entityType);
    }

    [TestMethod]
    public void MixedContext_DbSetTableNameWinsOverExpressiveDbSet()
    {
        using var ctx = new MixedDbSetContext(CreateOptions<MixedDbSetContext>());
        ctx.Database.EnsureCreated();

        var tableName = ctx.Model.FindEntityType(typeof(DiscoveryItem))!.GetTableName();
        Assert.AreEqual("Items", tableName,
            "When both DbSet and ExpressiveDbSet exist, the DbSet table name should be used");
    }

    [TestMethod]
    public void HistoryTableCreateScript_DoesNotIncludeExpressiveDbSetEntities()
    {
        using var ctx = new MigratingContext(CreateOptions<MigratingContext>());

        var script = ctx.GetService<IHistoryRepository>().GetCreateScript();

        Assert.IsFalse(script.Contains("\"Items\"", StringComparison.Ordinal));
    }

    [TestMethod]
    public async Task ExpressiveOnlyContext_MigrateAsync_CreatesSchema()
    {
        await using var ctx = new MigratingContext(CreateOptions<MigratingContext>());
        await ctx.Database.MigrateAsync();

        ctx.Items.Add(new DiscoveryItem { Name = "Widget" });
        await ctx.SaveChangesAsync();

        Assert.AreEqual(1, await ctx.Items.CountAsync());
        CollectionAssert.AreEqual(new[] { InitialMigration.Id }, (await ctx.Database.GetAppliedMigrationsAsync()).ToArray());
    }

    public class DiscoveryItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class ExpressiveOnlyContext : DbContext
    {
        public ExpressiveDbSet<DiscoveryItem> Items => this.ExpressiveSet<DiscoveryItem>();
        public ExpressiveOnlyContext(DbContextOptions<ExpressiveOnlyContext> options) : base(options) { }
    }

    public class MixedDbSetContext : DbContext
    {
        public DbSet<DiscoveryItem> Items => Set<DiscoveryItem>();
        public ExpressiveDbSet<DiscoveryItem> ExpressiveItems => this.ExpressiveSet<DiscoveryItem>();
        public MixedDbSetContext(DbContextOptions<MixedDbSetContext> options) : base(options) { }
    }

    public class MigratingContext : DbContext
    {
        public ExpressiveDbSet<DiscoveryItem> Items => this.ExpressiveSet<DiscoveryItem>();
        public MigratingContext(DbContextOptions<MigratingContext> options) : base(options) { }
    }

    [DbContext(typeof(MigratingContext))]
    [Migration(Id)]
    public class InitialMigration : Migration
    {
        public const string Id = "20260101000000_Init";

        protected override void Up(MigrationBuilder migrationBuilder)
            => migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                },
                constraints: table => table.PrimaryKey("PK_Items", x => x.Id));
    }

    [DbContext(typeof(MigratingContext))]
    public class MigratingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
            => modelBuilder.Entity<DiscoveryItem>(b =>
            {
                b.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnType("INTEGER");
                b.Property(x => x.Name).IsRequired().HasColumnType("TEXT");
                b.HasKey(x => x.Id);
                b.ToTable("Items");
            });
    }
}
