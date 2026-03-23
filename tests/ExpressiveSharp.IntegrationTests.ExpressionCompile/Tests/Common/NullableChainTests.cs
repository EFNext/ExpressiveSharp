using ExpressiveSharp.IntegrationTests.Infrastructure;

namespace ExpressiveSharp.IntegrationTests.ExpressionCompile.Tests.Common;

[TestClass]
public class NullableChainTests : Scenarios.Common.Tests.NullableChainTests
{
    protected override IIntegrationTestRunner CreateRunner() => new ExpressionCompileTestRunner();
}
