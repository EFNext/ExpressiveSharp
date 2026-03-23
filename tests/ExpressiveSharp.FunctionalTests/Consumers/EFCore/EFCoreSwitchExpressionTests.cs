using ExpressiveSharp.FunctionalTests.Infrastructure;
using ExpressiveSharp.FunctionalTests.PatternTests;

namespace ExpressiveSharp.FunctionalTests.Consumers.EFCore;

[TestClass]
public class EFCoreSwitchExpressionTests : SwitchExpressionTranslationTests
{
    protected override IExpressionConsumer CreateConsumer() => new EFCoreSqliteConsumer();
}
