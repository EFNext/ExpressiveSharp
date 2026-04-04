using ExpressiveSharp.IntegrationTests.Infrastructure;

namespace ExpressiveSharp.IntegrationTests.ExpressionCompile.Tests.Common;

[TestClass]
public class CommonScenarioTests : Scenarios.Common.Tests.CommonScenarioTestBase
{
    protected override IIntegrationTestRunner CreateRunner() => new ExpressionCompileTestRunner();
}
