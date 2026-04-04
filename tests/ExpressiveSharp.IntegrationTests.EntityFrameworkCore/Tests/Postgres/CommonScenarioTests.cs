#if TEST_POSTGRES
using ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Infrastructure;
using ExpressiveSharp.IntegrationTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Tests.Postgres;

[TestClass]
public class CommonScenarioTests : Scenarios.Common.Tests.CommonScenarioTests
{
    public TestContext TestContext { get; set; } = null!;

    protected override IIntegrationTestRunner CreateRunner()
    {
        if (!ContainerFixture.IsDockerAvailable)
            Assert.Inconclusive("Docker not available");
        return new EFCorePostgresTestRunner(
            ContainerFixture.PostgresConnectionString!, logSql: TestContext.WriteLine);
    }
}
#endif
