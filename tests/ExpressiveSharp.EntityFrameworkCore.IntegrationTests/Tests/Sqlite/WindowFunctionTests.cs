using ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.Sqlite;

[TestClass]
public class WindowFunctionTests : WindowFunctionTestBase
{
    protected override IAsyncDisposable CreateContextHandle(out DbContext context)
    {
        var handle = TestContextFactories.CreateSqlite();
        context = handle.Context;
        return handle;
    }
}
