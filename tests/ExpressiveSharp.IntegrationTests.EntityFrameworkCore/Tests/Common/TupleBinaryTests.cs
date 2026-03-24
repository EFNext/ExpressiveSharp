using ExpressiveSharp.IntegrationTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Tests.Common;

[TestClass]
public class TupleBinaryTests : Scenarios.Common.Tests.TupleBinaryTests
{
    public TestContext TestContext { get; set; } = null!;

    protected override IIntegrationTestRunner CreateRunner() => new EFCoreSqliteTestRunner(logSql: TestContext.WriteLine);

    [Ignore("EF Core cannot translate ValueTuple field access (Item1/Item2) in WHERE clauses")]
    [TestMethod]
    public override async Task Where_IsPriceQuantityMatch_FiltersCorrectly()
        => await base.Where_IsPriceQuantityMatch_FiltersCorrectly();
}
