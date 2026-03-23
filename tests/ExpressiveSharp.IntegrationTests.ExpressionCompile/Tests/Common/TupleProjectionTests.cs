using ExpressiveSharp.IntegrationTests.Infrastructure;

namespace ExpressiveSharp.IntegrationTests.ExpressionCompile.Tests.Common;

[TestClass]
public class TupleProjectionTests : Scenarios.Common.Tests.TupleProjectionTests
{
    protected override IIntegrationTestRunner CreateRunner() => new ExpressionCompileTestRunner();
}
