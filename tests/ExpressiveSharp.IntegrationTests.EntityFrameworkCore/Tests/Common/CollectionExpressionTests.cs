using ExpressiveSharp.IntegrationTests.Infrastructure;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Tests.Common;

[TestClass]
public class CollectionExpressionTests : Scenarios.Common.Tests.CollectionExpressionTests
{
    protected override IIntegrationTestRunner CreateRunner() => new EFCoreSqliteTestRunner();
}
