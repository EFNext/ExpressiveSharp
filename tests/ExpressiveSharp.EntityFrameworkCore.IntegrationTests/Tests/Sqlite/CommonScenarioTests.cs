using System.Linq.Expressions;
using ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Infrastructure;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests.Sqlite;

[TestClass]
public class CommonScenarioTests : CommonScenarioTestBase
{
    protected override IAsyncDisposable CreateContextHandle(out DbContext context)
    {
        var handle = TestContextFactories.CreateSqlite();
        context = handle.Context;
        return handle;
    }

    [TestMethod]
    public void Select_EnumSwitch_InlinesEnumConstantsIntoSql()
    {
        Expression<Func<Order, string>> expr = o => o.GetStatusLabelSwitchStatement();
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        var sql = Context.Set<Order>().Select(expanded).ToQueryString();

        StringAssert.Contains(sql, "CASE");
        Assert.IsFalse(sql.Contains('@'));
    }
}
