#if TEST_SQLSERVER && !NET10_0_OR_GREATER
using ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.SqlServer;

[TestClass]
public class ExecuteUpdateTests : ExecuteUpdateTestBase
{
    protected override IAsyncDisposable CreateContextHandle(out DbContext context)
    {
        if (!ContainerFixture.IsDockerAvailable)
            Assert.Inconclusive("Docker not available");
        var handle = TestContextFactories.CreateSqlServer();
        context = handle.Context;
        return handle;
    }
}
#endif
