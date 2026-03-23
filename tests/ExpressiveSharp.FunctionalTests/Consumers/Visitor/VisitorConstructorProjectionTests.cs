using ExpressiveSharp.FunctionalTests.Infrastructure;
using ExpressiveSharp.FunctionalTests.PatternTests;

namespace ExpressiveSharp.FunctionalTests.Consumers.Visitor;

[TestClass]
public class VisitorConstructorProjectionTests : ConstructorProjectionTranslationTests
{
    protected override IExpressionConsumer CreateConsumer() => new ExpressionVisitorConsumer();
}
