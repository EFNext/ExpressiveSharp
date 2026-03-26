using ExpressiveSharp.IntegrationTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.ExpressionCompile.Tests.Common;

[TestClass]
public class ExpressiveForMappingTests : Scenarios.Common.Tests.ExpressiveForMappingTests
{
    protected override IIntegrationTestRunner CreateRunner() => new ExpressionCompileTestRunner();
}
