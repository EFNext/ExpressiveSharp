using ExpressiveSharp.IntegrationTests.Infrastructure;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Tests.Common;

[Ignore("Requires loop-to-LINQ transformer — EF Core cannot translate Expression.Loop")]
[TestClass]
public class LoopTests : Scenarios.Common.Tests.LoopTests
{
    protected override IIntegrationTestRunner CreateRunner() => new EFCoreSqliteTestRunner();
}
