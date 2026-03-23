using ExpressiveSharp.FunctionalTests.Infrastructure;
using ExpressiveSharp.FunctionalTests.PatternTests;

namespace ExpressiveSharp.FunctionalTests.Consumers.Visitor;

[TestClass]
public class VisitorPolyfillPathwayTests : PolyfillPathwayTranslationTests
{
    protected override IExpressionConsumer CreateConsumer() => new ExpressionVisitorConsumer();
}
