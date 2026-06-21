using ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.Sqlite;

[TestClass]
public class PolymorphicDispatchTests : PolymorphicDispatchTestBase
{
    protected override IAsyncDisposable CreateContextHandle(out DbContext context)
    {
        var handle = TestContextFactories.CreateSqlitePolymorphic();
        context = handle.Context;
        return handle;
    }
}
