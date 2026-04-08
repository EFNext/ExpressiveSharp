#if TEST_POSTGRES
using System.Linq.Expressions;
using ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.Postgres;

[TestClass]
public class CommonScenarioTests : CommonScenarioTestBase
{
    protected override IAsyncDisposable CreateContextHandle(out DbContext context)
    {
        if (!ContainerFixture.IsDockerAvailable)
            Assert.Inconclusive("Docker not available");
        var handle = TestContextFactories.CreatePostgres();
        context = handle.Context;
        return handle;
    }

    // PostgreSQL sorts NULLs LAST on ASC by default, unlike SQLite / SQL Server /
    // MySQL which sort NULLs FIRST. Override the test to assert the Postgres
    // convention — this is a well-documented provider difference, not a bug.
    public override async Task OrderBy_TagLength_NullsAppearFirst()
    {
        Expression<Func<Order, int?>> tagLenExpr = o => o.TagLength;
        var expanded = (Expression<Func<Order, int?>>)tagLenExpr.ExpandExpressives();

        Expression<Func<Order, int>> idExpr = o => o.Id;

        var results = await Context.Set<Order>().OrderBy(expanded).Select(idExpr).ToListAsync();

        Assert.AreEqual(4, results.Count);
        Assert.AreEqual(3, results[^1]); // Order 3 has null Tag → sorts LAST on Postgres
    }
}
#endif
