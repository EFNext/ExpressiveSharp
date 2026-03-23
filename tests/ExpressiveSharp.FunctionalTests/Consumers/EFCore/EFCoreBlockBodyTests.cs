using ExpressiveSharp.FunctionalTests.Infrastructure;
using ExpressiveSharp.FunctionalTests.PatternTests;

namespace ExpressiveSharp.FunctionalTests.Consumers.EFCore;

[TestClass]
public class EFCoreBlockBodyTests : BlockBodyTranslationTests
{
    protected override IExpressionConsumer CreateConsumer() => new EFCoreSqliteConsumer();
}
