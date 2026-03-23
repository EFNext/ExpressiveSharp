using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.FunctionalTests.Models;

namespace ExpressiveSharp.FunctionalTests.PatternTests;

public abstract class StringOperationTranslationTests : TranslationTestBase
{
    [TestMethod]
    public async Task StringInterpolation_Translates()
    {
        Expression<Func<TestOrder, string>> expr = o => o.Summary;
        var expanded = (Expression<Func<TestOrder, string>>)expr.ExpandExpressives();
        await AssertSelectTranslates<TestOrder, string>(expanded);
    }
}
