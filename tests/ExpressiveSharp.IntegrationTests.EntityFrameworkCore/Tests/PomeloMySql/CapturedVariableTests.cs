#if TEST_POMELO_MYSQL && !NET10_0_OR_GREATER
using ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Tests.PomeloMySql;

[TestClass]
public class CapturedVariableTests : CapturedVariableTestBase
{
    protected override IntegrationTestDbContext CreateContext()
    {
        if (!ContainerFixture.IsDockerAvailable)
            Assert.Inconclusive("Docker not available");

        var dbName = $"test_{Guid.NewGuid():N}";
        var connStrBuilder = new MySqlConnectionStringBuilder(
            ContainerFixture.MySqlConnectionString!)
        {
            Database = dbName
        };

        using (var adminConn = new MySqlConnection(ContainerFixture.MySqlConnectionString!))
        {
            adminConn.Open();
            using var cmd = adminConn.CreateCommand();
            cmd.CommandText = $"CREATE DATABASE `{dbName}`";
            cmd.ExecuteNonQuery();
        }

        var serverVersion = ServerVersion.AutoDetect(connStrBuilder.ConnectionString);
        var options = new DbContextOptionsBuilder<IntegrationTestDbContext>()
            .UseMySql(connStrBuilder.ConnectionString, serverVersion)
            .UseExpressives()
            .Options;

        return new IntegrationTestDbContext(options);
    }
}
#endif
