using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.FunctionalTests.Models;

namespace ExpressiveSharp.FunctionalTests.PatternTests;

public abstract class EnumExpansionTranslationTests : TranslationTestBase
{
    [TestMethod]
    public async Task EnumExpansion_Select_Translates()
    {
        Expression<Func<TestOrder, string>> expr = o => o.StatusDescription;
        var expanded = (Expression<Func<TestOrder, string>>)expr.ExpandExpressives();
        await AssertSelectTranslates<TestOrder, string>(expanded);
    }

    [TestMethod]
    public async Task EnumExpansion_InWhere_Translates()
    {
        Expression<Func<TestOrder, string>> descExpr = o => o.StatusDescription;
        var expanded = (Expression<Func<TestOrder, string>>)descExpr.ExpandExpressives();

        var param = expanded.Parameters[0];
        var body = Expression.Equal(expanded.Body, Expression.Constant("Order approved"));
        var predicate = Expression.Lambda<Func<TestOrder, bool>>(body, param);

        await AssertWhereTranslates(predicate);
    }
}
