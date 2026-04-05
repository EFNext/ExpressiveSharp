#if TEST_POSTGRES
using ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.Postgres;

[TestClass]
public class ExpressiveExpansionTests : ExpressiveExpansionTestBase
{
    protected override IAsyncDisposable CreateContextHandle(out DbContext context)
    {
        if (!ContainerFixture.IsDockerAvailable)
            Assert.Inconclusive("Docker not available");
        var handle = TestContextFactories.CreatePostgres();
        context = handle.Context;
        return handle;
    }

    // Npgsql 8+ shaper fails with NullReferenceException when projecting a tuple
    // where one field comes from a CASE/WHEN (switch expression). Not an ExpressiveSharp
    // issue — the same projection works on SQL Server, SQLite, and MySQL.
    [TestMethod]
    public override Task NestedExpressive_GetOrderSummaryTuple_ExpandsBothGetGradeAndTotal()
    {
        Assert.Inconclusive("Npgsql shaper does not support tuple projection containing CASE/WHEN result");
        return Task.CompletedTask;
    }
}
#endif
