using ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;
using ExpressiveSharp.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.Sqlite;

/// <summary>
/// EF Core SQLite tests for <c>[ExpressiveProperty]</c>. Uses a self-contained
/// DbContext with a synthesized entity so the test doesn't depend on shared scenario models.
/// Verifies:
/// <list type="bullet">
///   <item>The synthesized property is auto-Ignored in the EF model (no column is generated).</item>
///   <item>Queries referencing the property emit SQL with the inlined formula.</item>
///   <item>Projection into <c>new T { Member = ... }</c> materializes via the <c>init</c> accessor.</item>
/// </list>
/// </summary>
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
    public async Task SynthesizedProperty_WhereClauseFiltersOnFormula()
    {
        var filtered = await Context.People
            .Where(p => p.FullName.StartsWith("Turing"))
            .ToListAsync();

        Assert.AreEqual(1, filtered.Count);
        Assert.AreEqual("Alan", filtered[0].FirstName);
    }
}

/// <summary>Self-contained entity for synthesized-property EF Core tests.</summary>
public partial class SynthesizedPerson
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";

    [ExpressiveProperty("FullName")]
    private string FullNameExpression => LastName + ", " + FirstName;
}

/// <summary>Self-contained DbContext for synthesized-property EF Core tests.</summary>
public class SynthesizedDbContext(DbContextOptions<SynthesizedDbContext> options) : DbContext(options)
{
    public DbSet<SynthesizedPerson> People => Set<SynthesizedPerson>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SynthesizedPerson>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
        });
    }
}
