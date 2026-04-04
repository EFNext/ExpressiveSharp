#if TEST_POSTGRES
using ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Tests.Postgres;

[TestClass]
public class ExpressiveExpansionTests : ExpressiveExpansionTestBase
{
    private NpgsqlDataSource? _dataSource;

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
