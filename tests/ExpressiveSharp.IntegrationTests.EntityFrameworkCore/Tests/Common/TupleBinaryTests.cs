using ExpressiveSharp.IntegrationTests.Infrastructure;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Tests.Common;

[TestClass]
public class TupleBinaryTests : Scenarios.Common.Tests.TupleBinaryTests
{
    protected override IIntegrationTestRunner CreateRunner() => new EFCoreSqliteTestRunner();

    [Ignore("EF Core cannot translate ValueTuple field access (Item1/Item2) in WHERE clauses")]
    [TestMethod]
    public override async Task Where_IsPriceQuantityMatch_FiltersCorrectly()
        => await base.Where_IsPriceQuantityMatch_FiltersCorrectly();
}
