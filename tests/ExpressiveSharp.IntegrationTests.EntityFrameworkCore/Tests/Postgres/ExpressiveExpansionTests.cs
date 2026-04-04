#if TEST_POSTGRES
using ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Tests.Postgres;

[TestClass]
public class ExpressiveExpansionTests : ExpressiveExpansionTestBase
{
    private NpgsqlDataSource? _dataSource;

    // Npgsql 8+ shaper fails with NullReferenceException when projecting a tuple
    // where one field comes from a CASE/WHEN (switch expression). Not an ExpressiveSharp
    // issue — the same projection works on SQL Server, SQLite, and MySQL.
    [TestMethod]
    public override Task NestedExpressive_GetOrderSummaryTuple_ExpandsBothGetGradeAndTotal()
    {
        Assert.Inconclusive("Npgsql shaper does not support tuple projection containing CASE/WHEN result");
        return Task.CompletedTask;
    }

    protected override IntegrationTestDbContext CreateContext()
    {
        if (!ContainerFixture.IsDockerAvailable)
            Assert.Inconclusive("Docker not available");

        var dbName = $"test_{Guid.NewGuid():N}";
        var connStrBuilder = new NpgsqlConnectionStringBuilder(
            ContainerFixture.PostgresConnectionString!)
        {
            Database = dbName
        };

        using (var adminConn = new NpgsqlConnection(ContainerFixture.PostgresConnectionString!))
        {
            adminConn.Open();
            using var cmd = adminConn.CreateCommand();
            cmd.CommandText = $"CREATE DATABASE \"{dbName}\"";
            cmd.ExecuteNonQuery();
        }

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connStrBuilder.ConnectionString);
        dataSourceBuilder.EnableRecordsAsTuples();
        _dataSource = dataSourceBuilder.Build();

        var options = new DbContextOptionsBuilder<IntegrationTestDbContext>()
            .UseNpgsql(_dataSource)
            .UseExpressives()
            .ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning))
            .Options;

        return new IntegrationTestDbContext(options);
    }

    protected override async Task OnCleanupAsync()
    {
        if (_dataSource is not null)
            await _dataSource.DisposeAsync();
    }
}
#endif
