#if TEST_POMELO_MYSQL && !NET10_0_OR_GREATER
using ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.PomeloMySql;

[TestClass]
public class UseExpressivesConventionTests : UseExpressivesConventionTestBase
{
    protected override IAsyncDisposable CreateContextHandle(out DbContext context)
    {
        if (!ContainerFixture.IsDockerAvailable)
            Assert.Inconclusive("Docker not available");
        var handle = TestContextFactories.CreatePomeloMySql();
        context = handle.Context;
        return handle;
    }
}
#endif
