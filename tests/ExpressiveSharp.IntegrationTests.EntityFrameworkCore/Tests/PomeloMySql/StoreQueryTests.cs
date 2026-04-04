#if TEST_POMELO_MYSQL && !NET10_0_OR_GREATER
using ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Infrastructure;
using ExpressiveSharp.IntegrationTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Tests.PomeloMySql;

[TestClass]
public class StoreQueryTests : Scenarios.Store.Tests.StoreQueryTests
{
    public TestContext TestContext { get; set; } = null!;

    protected override IIntegrationTestRunner CreateRunner()
    {
        if (!ContainerFixture.IsDockerAvailable)
            Assert.Inconclusive("Docker not available");
        return new EFCorePomeloMySqlTestRunner(
            ContainerFixture.MySqlConnectionString!, logSql: TestContext.WriteLine);
    }
}
#endif
