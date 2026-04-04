using ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Tests.Sqlite;

[TestClass]
public class ExpressiveExpansionTests : ExpressiveExpansionTestBase
{
    private SqliteConnection _connection = null!;

    protected override IntegrationTestDbContext CreateContext()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<IntegrationTestDbContext>()
            .UseSqlite(_connection)
            .UseExpressives()
            .Options;

        return new IntegrationTestDbContext(options);
    }

    protected override async Task OnCleanupAsync()
    {
        await _connection.DisposeAsync();
    }
}
