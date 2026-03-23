using ExpressiveSharp.FunctionalTests.Infrastructure;
using ExpressiveSharp.FunctionalTests.PatternTests;

namespace ExpressiveSharp.FunctionalTests.Consumers.Visitor;

[TestClass]
public class VisitorBlockBodyTests : BlockBodyTranslationTests
{
    protected override IExpressionConsumer CreateConsumer() => new ExpressionVisitorConsumer();
}
