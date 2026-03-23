using ExpressiveSharp.IntegrationTests.Infrastructure;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Tests.Common;

[TestClass]
public class PatternMatchingTests : Scenarios.Common.Tests.PatternMatchingTests
{
    protected override IIntegrationTestRunner CreateRunner() => new EFCoreSqliteTestRunner();
}
