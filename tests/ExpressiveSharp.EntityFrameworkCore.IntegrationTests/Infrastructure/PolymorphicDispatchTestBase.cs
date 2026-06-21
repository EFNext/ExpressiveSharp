using ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;

// Verifies that a virtual [Expressive] member (Animal.Description) dispatches on the runtime
// type within a single TPH query: each row uses its own override, translated to a discriminator
// CASE in SQL.
public abstract class PolymorphicDispatchTestBase : EFCoreRelationalTestBase<PolymorphicTestDbContext>
{
    [TestInitialize]
    public async Task SeedAnimals()
    {
        Context.Animals.AddRange(
            new Animal { Id = 1, Name = "Critter" },
            new Dog { Id = 2, Name = "Rex", Breed = "Lab" },
            new Cat { Id = 3, Name = "Tom", Color = "black" });
        await Context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task Select_Description_UsesEachRowsRuntimeType()
    {
        var descriptions = await Context.Animals
            .OrderBy(a => a.Id)
            .Select(a => a.Description)
            .ToListAsync();

        CollectionAssert.AreEqual(
            new[] { "Animal: Critter", "Dog:Lab", "Cat:black" },
            descriptions);
    }

    [TestMethod]
    public async Task Where_OnDescription_TranslatesAndFiltersByRuntimeType()
    {
        // The polymorphic CASE appears in a WHERE clause; only the Dog row's branch matches.
        var ids = await Context.Animals
            .Where(a => a.Description == "Dog:Lab")
            .Select(a => a.Id)
            .ToListAsync();

        CollectionAssert.AreEqual(new[] { 2 }, ids);
    }

    [TestMethod]
    public void Select_Description_EmitsDiscriminatorCase()
    {
        var sql = Context.Animals.Select(a => a.Description).ToQueryString();

        StringAssert.Contains(sql, "CASE", "Polymorphic dispatch should translate to a SQL CASE expression. SQL:\n" + sql);
        StringAssert.Contains(sql, "Kind", "The CASE should branch on the TPH discriminator column. SQL:\n" + sql);
    }
}
