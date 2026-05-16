using ExpressiveSharp.Mapping;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.Sqlite;

// Regression for https://github.com/EFNext/ExpressiveSharp/issues/64 — [ExpressiveProperty]
// bodies that SelectMany over an ICollection<T> navigation produced a redundant
// Expression.Convert(ICollection<T> -> IEnumerable<T>) in the generated tree, which
// EF Core's relational translator could not handle.
[TestClass]
public class SelectManyOnNavigationCollectionTests
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

    [TestMethod]
    public void SelectMany_OverNavigationCollection_TranslatesAndReturnsRows()
    {
        var options = new DbContextOptionsBuilder<NavTreeContext>()
            .UseSqlite(_connection)
            .UseExpressives()
            .Options;

        using var ctx = new NavTreeContext(options);
        ctx.Database.EnsureCreated();

        ctx.Grandparents.Add(new NavGrandparent
        {
            Id = 1,
            Parents =
            [
                new NavParent { Id = 10, Children = [new NavChild { Id = 100, Value = 1 }, new NavChild { Id = 101, Value = 2 }] },
                new NavParent { Id = 11, Children = [new NavChild { Id = 110, Value = 3 }] },
            ],
        });
        ctx.SaveChanges();

        var grandChildren = ctx.Grandparents
            .Select(gp => gp.GrandChildren)
            .ToList();

        Assert.AreEqual(1, grandChildren.Count);
        var values = grandChildren[0].Select(c => c.Value).OrderBy(v => v).ToList();
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, values);
    }
}

public class NavChild
{
    public int Id { get; set; }
    public int NavParentId { get; set; }
    public int Value { get; set; }
}

public class NavParent
{
    public int Id { get; set; }
    public int NavGrandparentId { get; set; }
    public virtual ICollection<NavChild> Children { get; set; } = [];
}

public partial class NavGrandparent
{
    public int Id { get; set; }
    public virtual ICollection<NavParent> Parents { get; set; } = [];

    [ExpressiveProperty("GrandChildren")]
    private IEnumerable<NavChild> GrandChildrenExpr =>
        Parents.SelectMany(p => p.Children);
}

public class NavTreeContext(DbContextOptions<NavTreeContext> options) : DbContext(options)
{
    public DbSet<NavGrandparent> Grandparents => Set<NavGrandparent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NavGrandparent>().Property(x => x.Id).ValueGeneratedNever();
        modelBuilder.Entity<NavParent>().Property(x => x.Id).ValueGeneratedNever();
        modelBuilder.Entity<NavChild>().Property(x => x.Id).ValueGeneratedNever();
    }
}
