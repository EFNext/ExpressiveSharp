#if TEST_COSMOS
using ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Infrastructure;
using ExpressiveSharp.IntegrationTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Tests.Cosmos;

[TestClass]
public class CommonScenarioTests : Scenarios.Common.Tests.CommonScenarioTests
{
    public TestContext TestContext { get; set; } = null!;

    protected override IIntegrationTestRunner CreateRunner()
    {
        if (!ContainerFixture.IsDockerAvailable)
            Assert.Inconclusive("Docker not available");

        var dbName = $"test_{Guid.NewGuid():N}";
        return new EFCoreCosmosTestRunner(
            ContainerFixture.CosmosConnectionString!, dbName, logSql: TestContext.WriteLine);
    }

    // Cosmos DB does not support GROUP BY on computed expressions
    [TestMethod]
    public override Task GroupBy_StatusDescription_CountsCorrectly()
    {
        Assert.Inconclusive("Cosmos DB does not support GROUP BY on computed expressions");
        return Task.CompletedTask;
    }

    [TestMethod]
    public override Task GroupBy_GetGrade_CountsCorrectly()
    {
        Assert.Inconclusive("Cosmos DB does not support GROUP BY on computed expressions");
        return Task.CompletedTask;
    }

    // Cosmos DB does not support queries across entity types (Customer as separate set)
    [TestMethod]
    public override Task Select_CustomerCity_ViaCustomerExpressive()
    {
        Assert.Inconclusive("Cosmos DB does not support cross-entity queries");
        return Task.CompletedTask;
    }

    // Cosmos DB collection expressions (arrays) are not translatable
    [TestMethod]
    public override Task Select_PriceBreakpoints_ReturnsArrayLiteral()
    {
        Assert.Inconclusive("Cosmos DB does not support array literal projection");
        return Task.CompletedTask;
    }
}
#endif
