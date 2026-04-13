using ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.Sqlite;

/// <summary>
/// EF Core SQLite tests for <c>[Expressive(Projectable = true)]</c>. Uses a self-contained
/// DbContext with a Projectable entity so the test doesn't depend on shared scenario models.
/// Verifies:
/// <list type="bullet">
///   <item>The Projectable property is auto-Ignored in the EF model (no column is generated).</item>
///   <item>Queries referencing the property emit SQL with the inlined formula.</item>
///   <item>Projection into <c>new T { Member = ... }</c> materializes via the <c>init</c> accessor.</item>
/// </list>
/// </summary>
[TestClass]
public class ProjectableExpressiveSqlTests
{
    private TestContextFactories.SqliteContextHandle<ProjectableDbContext> _handle = null!;

    private ProjectableDbContext Context => _handle.Context;

    [TestInitialize]
    public async Task InitContext()
    {
        _handle = TestContextFactories.CreateSqlite<ProjectableDbContext>(o => new ProjectableDbContext(o));
        await Context.Database.EnsureCreatedAsync();
        Context.People.AddRange(
            new ProjectablePerson { Id = 1, FirstName = "Ada",  LastName = "Lovelace" },
            new ProjectablePerson { Id = 2, FirstName = "Alan", LastName = "Turing" });
        await Context.SaveChangesAsync();
    }

    [TestCleanup]
    public async Task CleanupContext() => await _handle.DisposeAsync();

    [TestMethod]
    public void ProjectableProperty_IsAutoIgnored_NoColumnInModel()
    {
        // The ExpressivePropertiesNotMappedConvention calls Ignore() for every [Expressive]
        // member. This is load-bearing for Projectable properties because they have writable
        // accessors — without the Ignore, EF would try to create a real column and migrations
        // would include a FullName column.
        var entity = Context.Model.FindEntityType(typeof(ProjectablePerson))!;
        Assert.IsNull(entity.FindProperty(nameof(ProjectablePerson.FullName)),
            "Projectable property must not be mapped as a column");
    }

    [TestMethod]
    public async Task ProjectableProperty_SelectInlinesFormulaIntoSql()
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
    public async Task ProjectableProperty_MemberInitProjection_MaterializesStoredValue()
    {
        // The HotChocolate / AutoMapper projection pattern: `new T { Member = src.Member }`.
        // The ExpressiveReplacer rewrites `p.FullName` on the RHS to the formula, EF emits
        // the formula as SQL, the result is written via the init accessor, and the stored
        // value is returned on subsequent reads.
        var projected = await Context.People
            .OrderBy(p => p.Id)
            .Select(p => new ProjectablePerson
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
    public async Task ProjectableProperty_WhereClauseFiltersOnFormula()
    {
        // The Projectable property can appear in Where clauses — after rewriting, EF evaluates
        // the formula server-side and filters rows.
        var filtered = await Context.People
            .Where(p => p.FullName.StartsWith("Turing"))
            .ToListAsync();

        Assert.AreEqual(1, filtered.Count);
        Assert.AreEqual("Alan", filtered[0].FirstName);
    }
}

/// <summary>Self-contained entity for Projectable EF Core tests.</summary>
public class ProjectablePerson
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";

    [Expressive(Projectable = true)]
    public string FullName
    {
        get => field ?? (LastName + ", " + FirstName);
        init => field = value;
    }
}

/// <summary>Self-contained DbContext for Projectable EF Core tests.</summary>
public class ProjectableDbContext(DbContextOptions<ProjectableDbContext> options) : DbContext(options)
{
    public DbSet<ProjectablePerson> People => Set<ProjectablePerson>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectablePerson>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
        });
    }
}
