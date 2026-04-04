#if TEST_SQLSERVER
using ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Infrastructure;
using ExpressiveSharp.IntegrationTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Tests.SqlServer;

[TestClass]
public class CommonScenarioTests : Scenarios.Common.Tests.CommonScenarioTests
{
    public TestContext TestContext { get; set; } = null!;

    protected override IIntegrationTestRunner CreateRunner()
    {
        if (!ContainerFixture.IsDockerAvailable)
            Assert.Inconclusive("Docker not available");
        return new EFCoreSqlServerTestRunner(
            ContainerFixture.SqlServerConnectionString!, logSql: TestContext.WriteLine);
    }
}
#endif
