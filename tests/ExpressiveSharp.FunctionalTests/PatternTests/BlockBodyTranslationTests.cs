using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.FunctionalTests.Models;
using ExpressiveSharp.Transformers;

namespace ExpressiveSharp.FunctionalTests.PatternTests;

public abstract class BlockBodyTranslationTests : TranslationTestBase
{
    [TestMethod]
    public async Task BlockBody_IfElse_Translates()
    {
        Expression<Func<TestOrder, string>> expr = o => o.GetCategory();
        var expanded = (Expression<Func<TestOrder, string>>)expr.ExpandExpressives(
            new FlattenBlockExpressions());
        await AssertSelectTranslates<TestOrder, string>(expanded);
    }

    [TestMethod]
    public async Task BlockBody_InWhere_Translates()
    {
        Expression<Func<TestOrder, string>> catExpr = o => o.GetCategory();
        var expanded = (Expression<Func<TestOrder, string>>)catExpr.ExpandExpressives(
            new FlattenBlockExpressions());

        var param = expanded.Parameters[0];
        var body = Expression.Equal(expanded.Body, Expression.Constant("Bulk"));
        var predicate = Expression.Lambda<Func<TestOrder, bool>>(body, param);

        await AssertWhereTranslates(predicate);
    }
}
