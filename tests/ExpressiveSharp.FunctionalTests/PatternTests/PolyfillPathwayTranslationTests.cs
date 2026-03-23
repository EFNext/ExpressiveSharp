using ExpressiveSharp.FunctionalTests.Models;

namespace ExpressiveSharp.FunctionalTests.PatternTests;

public abstract class PolyfillPathwayTranslationTests : TranslationTestBase
{
    [TestMethod]
    public async Task Polyfill_Create_SimpleCondition_Translates()
    {
        var expr = ExpressionPolyfill.Create((TestOrder o) => o.Price > 50);
        await AssertWhereTranslates(expr);
    }

    [TestMethod]
    public async Task Polyfill_Create_NullConditional_Translates()
    {
        var expr = ExpressionPolyfill.Create((TestOrder o) => o.Tag != null ? o.Tag.Length : (int?)null);
        await AssertSelectTranslates<TestOrder, int?>(expr);
    }

    [TestMethod]
    public async Task Polyfill_Create_Arithmetic_Translates()
    {
        var expr = ExpressionPolyfill.Create((TestOrder o) => o.Price * o.Quantity);
        await AssertSelectTranslates<TestOrder, double>(expr);
    }
}
