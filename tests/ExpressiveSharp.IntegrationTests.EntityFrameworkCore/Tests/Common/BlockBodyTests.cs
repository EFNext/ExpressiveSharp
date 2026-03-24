using ExpressiveSharp.IntegrationTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Tests.Common;

[TestClass]
public class BlockBodyTests : Scenarios.Common.Tests.BlockBodyTests
{
    public TestContext TestContext { get; set; } = null!;

    protected override IIntegrationTestRunner CreateRunner() => new EFCoreSqliteTestRunner(logSql: TestContext.WriteLine);
}
