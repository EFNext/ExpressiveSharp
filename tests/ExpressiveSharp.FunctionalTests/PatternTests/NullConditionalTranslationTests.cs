using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.FunctionalTests.Models;

namespace ExpressiveSharp.FunctionalTests.PatternTests;

public abstract class NullConditionalTranslationTests : TranslationTestBase
{
    [TestMethod]
    public async Task NullConditional_MemberAccess_Translates()
    {
        Expression<Func<TestOrder, string?>> expr = o => o.CustomerName;
        var expanded = (Expression<Func<TestOrder, string?>>)expr.ExpandExpressives();
        await AssertSelectTranslates<TestOrder, string?>(expanded);
    }

    [TestMethod]
    public async Task NullConditional_ValueType_Translates()
    {
        Expression<Func<TestOrder, int?>> expr = o => o.TagLength;
        var expanded = (Expression<Func<TestOrder, int?>>)expr.ExpandExpressives();
        await AssertSelectTranslates<TestOrder, int?>(expanded);
    }

    [TestMethod]
    public async Task NullConditional_InWhere_Translates()
    {
        Expression<Func<TestOrder, string?>> nameExpr = o => o.CustomerName;
        var expandedName = (Expression<Func<TestOrder, string?>>)nameExpr.ExpandExpressives();

        var param = expandedName.Parameters[0];
        var body = Expression.Equal(expandedName.Body, Expression.Constant("Alice", typeof(string)));
        var predicate = Expression.Lambda<Func<TestOrder, bool>>(body, param);

        await AssertWhereTranslates(predicate);
    }
}
