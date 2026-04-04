#if TEST_SQLSERVER
using ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Tests.SqlServer;

[TestClass]
public class CapturedVariableTests : CapturedVariableTestBase
{
    protected override IntegrationTestDbContext CreateContext()
    {
        if (!ContainerFixture.IsDockerAvailable)
            Assert.Inconclusive("Docker not available");

        var dbName = $"test_{Guid.NewGuid():N}";
        var connStr = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(
            ContainerFixture.SqlServerConnectionString!)
        {
            InitialCatalog = dbName
        }.ConnectionString;

        var options = new DbContextOptionsBuilder<IntegrationTestDbContext>()
            .UseSqlServer(connStr)
            .UseExpressives()
            .Options;

        return new IntegrationTestDbContext(options);
    }
}
#endif
