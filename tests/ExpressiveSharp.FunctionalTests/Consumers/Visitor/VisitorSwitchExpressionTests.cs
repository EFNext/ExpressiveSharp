using ExpressiveSharp.FunctionalTests.Infrastructure;
using ExpressiveSharp.FunctionalTests.PatternTests;

namespace ExpressiveSharp.FunctionalTests.Consumers.Visitor;

[TestClass]
public class VisitorSwitchExpressionTests : SwitchExpressionTranslationTests
{
    protected override IExpressionConsumer CreateConsumer() => new ExpressionVisitorConsumer();
}
