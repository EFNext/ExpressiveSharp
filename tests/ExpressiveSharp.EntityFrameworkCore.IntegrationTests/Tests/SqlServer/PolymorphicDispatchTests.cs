#if TEST_SQLSERVER
using ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.SqlServer;

[TestClass]
public class PolymorphicDispatchTests : PolymorphicDispatchTestBase
{
    protected override IAsyncDisposable CreateContextHandle(out DbContext context)
    {
        if (!ContainerFixture.IsDockerAvailable)
            Assert.Inconclusive("Docker not available");
        var handle = TestContextFactories.CreateSqlServerPolymorphic();
        context = handle.Context;
        return handle;
    }
}
#endif
