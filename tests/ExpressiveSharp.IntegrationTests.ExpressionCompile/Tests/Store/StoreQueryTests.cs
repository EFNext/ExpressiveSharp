using ExpressiveSharp.IntegrationTests.Infrastructure;

namespace ExpressiveSharp.IntegrationTests.ExpressionCompile.Tests.Store;

[TestClass]
public class StoreQueryTests : Scenarios.Store.Tests.StoreQueryTestBase
{
    protected override IIntegrationTestRunner CreateRunner() => new ExpressionCompileTestRunner();
}
