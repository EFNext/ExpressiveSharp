#if TEST_POSTGRES
using ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Tests.Postgres;

[TestClass]
public class CapturedVariableTests : CapturedVariableTestBase
{
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

        var options = new DbContextOptionsBuilder<IntegrationTestDbContext>()
            .UseNpgsql(connStrBuilder.ConnectionString)
            .UseExpressives()
            .Options;

        return new IntegrationTestDbContext(options);
    }
}
#endif
