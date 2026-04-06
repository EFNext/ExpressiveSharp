#if TEST_SQLSERVER
using ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.SqlServer;

[TestClass]
public class QueryFilterTests : QueryFilterTestBase
{
    protected override IAsyncDisposable CreateContextHandle(out DbContext context)
    {
        if (!ContainerFixture.IsDockerAvailable)
            Assert.Inconclusive("Docker not available");
        var handle = TestContextFactories.CreateSqlServerQueryFilter();
        context = handle.Context;
        return handle;
    }
}
#endif
