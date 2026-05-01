using ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;
using ExpressiveSharp.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.Sqlite;

[TestClass]
public class SynthesizedExpressiveSqlTests
{
    private TestContextFactories.SqliteContextHandle<SynthesizedDbContext> _handle = null!;

    private SynthesizedDbContext Context => _handle.Context;

    [TestInitialize]
    public async Task InitContext()
    {
        _handle = TestContextFactories.CreateSqlite<SynthesizedDbContext>(o => new SynthesizedDbContext(o));
        await Context.Database.EnsureCreatedAsync();
        Context.People.AddRange(
            new SynthesizedPerson { Id = 1, FirstName = "Ada",  LastName = "Lovelace" },
            new SynthesizedPerson { Id = 2, FirstName = "Alan", LastName = "Turing" });
        await Context.SaveChangesAsync();
    }

    [TestCleanup]
    public async Task CleanupContext() => await _handle.DisposeAsync();

    [TestMethod]
    public void SynthesizedProperty_IsAutoIgnored_NoColumnInModel()
    {
        // The ExpressivePropertiesNotMappedConvention calls Ignore() for every member backed by
        // a registry expression. This is load-bearing for synthesized properties because they
        // have writable accessors — without the Ignore, EF would try to create a real column.
        var entity = Context.Model.FindEntityType(typeof(SynthesizedPerson))!;
        Assert.IsNull(entity.FindProperty(nameof(SynthesizedPerson.FullName)),
            "Synthesized property must not be mapped as a column");
    }

    [TestMethod]
    public async Task SynthesizedProperty_SelectInlinesFormulaIntoSql()
    {
        var labels = await Context.People
            .OrderBy(p => p.Id)
            .Select(p => p.FullName)
            .ToListAsync();

        Assert.AreEqual(2, labels.Count);
        Assert.AreEqual("Lovelace, Ada", labels[0]);
        Assert.AreEqual("Turing, Alan", labels[1]);
    }

    [TestMethod]
    public async Task SynthesizedProperty_MemberInitProjection_MaterializesStoredValue()
    {
        // The HotChocolate / AutoMapper projection pattern: `new T { Member = src.Member }`.
        // The ExpressiveReplacer rewrites `p.FullName` on the RHS to the formula, EF emits
        // the formula as SQL, the result is written via the init accessor, and the stored
        // value is returned on subsequent reads.
        var projected = await Context.People
            .OrderBy(p => p.Id)
            .Select(p => new SynthesizedPerson
            {
                Id = p.Id,
                FullName = p.FullName,
            })
            .ToListAsync();

        Assert.AreEqual(2, projected.Count);
        Assert.AreEqual("Lovelace, Ada", projected[0].FullName);
        Assert.AreEqual("Turing, Alan", projected[1].FullName);
    }

    [TestMethod]
    public async Task SynthesizedProperty_BodyUsesStaticHelperIQueryable_TranslatesToCorrelatedSubquery()
    {
        // Issue #50 follow-up: [ExpressiveProperty] body uses IQueryable<T> from a static helper.
        GroupedRowQueryContext.Db = Context;
        Context.Rows.AddRange(
            new GroupedRow { Id = 1, GroupId = 1, CreatedAt = new DateTime(2026, 1, 1) },
            new GroupedRow { Id = 2, GroupId = 1, CreatedAt = new DateTime(2026, 1, 2) },
            new GroupedRow { Id = 3, GroupId = 1, CreatedAt = new DateTime(2026, 1, 3) },
            new GroupedRow { Id = 4, GroupId = 2, CreatedAt = new DateTime(2026, 1, 1) });
        await Context.SaveChangesAsync();

        var pairs = await Context.Rows
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, r.PreviousId })
            .ToListAsync();

        Assert.AreEqual(4, pairs.Count);
        Assert.AreEqual(0, pairs[0].PreviousId);
        Assert.AreEqual(1, pairs[1].PreviousId);
        Assert.AreEqual(2, pairs[2].PreviousId);
        Assert.AreEqual(0, pairs[3].PreviousId);
    }

    [TestMethod]
    public async Task SynthesizedProperty_WhereClauseFiltersOnFormula()
    {
        var filtered = await Context.People
            .Where(p => p.FullName.StartsWith("Turing"))
            .ToListAsync();

        Assert.AreEqual(1, filtered.Count);
        Assert.AreEqual("Alan", filtered[0].FirstName);
    }
}

public partial class SynthesizedPerson
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";

    [ExpressiveProperty("FullName")]
    private string FullNameExpression => LastName + ", " + FirstName;
}

public class SynthesizedDbContext(DbContextOptions<SynthesizedDbContext> options) : DbContext(options)
{
    public DbSet<SynthesizedPerson> People => Set<SynthesizedPerson>();
    public DbSet<GroupedRow> Rows => Set<GroupedRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SynthesizedPerson>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
        });
        modelBuilder.Entity<GroupedRow>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
        });
    }
}

internal static class GroupedRowQueryContext
{
    public static SynthesizedDbContext Db { get; set; } = null!;
    public static IQueryable<T> Query<T>() where T : class => Db.Set<T>();
}

public partial class GroupedRow
{
    public int Id { get; init; }
    public int GroupId { get; init; }
    public DateTime CreatedAt { get; init; }

    [ExpressiveProperty("PreviousId")]
    private int PreviousIdExpr =>
        GroupedRowQueryContext.Query<GroupedRow>()
            .Where(r => r.GroupId == GroupId && r.CreatedAt < CreatedAt)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => r.Id)
            .FirstOrDefault();
}
