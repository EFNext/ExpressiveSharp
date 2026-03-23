using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.FunctionalTests.Models;

namespace ExpressiveSharp.FunctionalTests.PatternTests;

public abstract class SwitchExpressionTranslationTests : TranslationTestBase
{
    [TestMethod]
    public async Task SwitchExpression_RelationalPattern_Translates()
    {
        Expression<Func<TestOrder, string>> expr = o => o.GetGrade();
        var expanded = (Expression<Func<TestOrder, string>>)expr.ExpandExpressives();
        await AssertSelectTranslates<TestOrder, string>(expanded);
    }

    [TestMethod]
    public async Task SwitchExpression_InOrderBy_Translates()
    {
        Expression<Func<TestOrder, string>> expr = o => o.GetGrade();
        var expanded = (Expression<Func<TestOrder, string>>)expr.ExpandExpressives();
        await AssertOrderByTranslates<TestOrder, string>(expanded);
    }
}
