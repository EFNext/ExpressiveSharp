using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.FunctionalTests.Models;

namespace ExpressiveSharp.FunctionalTests.PatternTests;

public abstract class ConstructorProjectionTranslationTests : TranslationTestBase
{
    [TestMethod]
    public async Task Constructor_MemberInit_Translates()
    {
        Expression<Func<TestOrder, TestOrderDto>> expr =
            o => new TestOrderDto(o.Id, o.Tag ?? "N/A", o.Price * o.Quantity);
        var expanded = (Expression<Func<TestOrder, TestOrderDto>>)expr.ExpandExpressives();
        await AssertSelectTranslates<TestOrder, TestOrderDto>(expanded);
    }
}
