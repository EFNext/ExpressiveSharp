using ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.Sqlite;

[TestClass]
public class PolymorphicDispatchDisabledTests
{
    [TestMethod]
    public async Task DisablePolymorphicDispatch_FallsBackToStaticBaseBody()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        try
        {
            var options = new DbContextOptionsBuilder<PolymorphicTestDbContext>()
                .UseSqlite(connection)
                .UseExpressives(o => o.DisablePolymorphicDispatch())
                .Options;

            await using var context = new PolymorphicTestDbContext(options);
            await context.Database.EnsureCreatedAsync();

            context.Animals.AddRange(
                new Animal { Id = 1, Name = "Critter" },
                new Dog { Id = 2, Name = "Rex", Breed = "Lab" },
                new Cat { Id = 3, Name = "Tom", Color = "black" });
            await context.SaveChangesAsync();

            var descriptions = await context.Animals
                .OrderBy(a => a.Id)
                .Select(a => a.Description)
                .ToListAsync();

            // Dispatch disabled: every row uses the base Animal.Description body.
            CollectionAssert.AreEqual(
                new[] { "Animal: Critter", "Animal: Rex", "Animal: Tom" },
                descriptions);
        }
        finally
        {
            await connection.DisposeAsync();
        }
    }
}
