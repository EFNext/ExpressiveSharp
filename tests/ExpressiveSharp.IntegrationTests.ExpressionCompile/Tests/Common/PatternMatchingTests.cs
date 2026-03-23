using ExpressiveSharp.IntegrationTests.Infrastructure;

namespace ExpressiveSharp.IntegrationTests.ExpressionCompile.Tests.Common;

[TestClass]
public class PatternMatchingTests : Scenarios.Common.Tests.PatternMatchingTests
{
    protected override IIntegrationTestRunner CreateRunner() => new ExpressionCompileTestRunner();
}
