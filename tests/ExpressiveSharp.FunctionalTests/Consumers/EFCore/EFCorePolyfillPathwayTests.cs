using ExpressiveSharp.FunctionalTests.Infrastructure;
using ExpressiveSharp.FunctionalTests.PatternTests;

namespace ExpressiveSharp.FunctionalTests.Consumers.EFCore;

[TestClass]
public class EFCorePolyfillPathwayTests : PolyfillPathwayTranslationTests
{
    protected override IExpressionConsumer CreateConsumer() => new EFCoreSqliteConsumer();
}
