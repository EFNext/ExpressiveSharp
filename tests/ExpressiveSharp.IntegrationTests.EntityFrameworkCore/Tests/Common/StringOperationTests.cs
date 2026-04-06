using System.Linq.Expressions;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.IntegrationTests.Infrastructure;
using ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.EntityFrameworkCore.Tests.Common;

[TestClass]
public class StringOperationTests : Scenarios.Common.Tests.StringOperationTests
{
    public TestContext TestContext { get; set; } = null!;

    protected override IIntegrationTestRunner CreateRunner() => new EFCoreSqliteTestRunner(logSql: TestContext.WriteLine);

    [TestMethod]
    public async Task Select_FormattedPrice_UsesToStringWithFormat()
    {
        // ToString("F2") is not translatable to SQL. In a Select projection EF Core
        // falls back to client evaluation. In Where/OrderBy this would throw.
        Expression<Func<Order, string>> expr = o => o.FormattedPrice;
        var expanded = (Expression<Func<Order, string>>)expr.ExpandExpressives();

        // Verify the expanded tree contains a ToString(string) call
        var body = ((LambdaExpression)expanded).Body;
        var hasToStringWithFormat = body is MethodCallExpression call
            && call.Method.Name == "ToString"
            && call.Method.GetParameters().Length == 1
            && call.Method.GetParameters()[0].ParameterType == typeof(string);
        Assert.IsTrue(hasToStringWithFormat, "Expected ToString(string) call in expanded expression");

        var results = await Runner.SelectAsync<Order, string>(expanded);

        Assert.AreEqual(4, results.Count);
    }

    [TestMethod]
    public async Task Where_Summary_TranslatesToSql()
    {
        // Summary uses string.Concat(string, string, string, string).
        // This verifies the 4-arg overload translates to SQL (Where throws if not).
        Expression<Func<Order, bool>> predicate = o => o.Summary == "Order #1: RUSH";
        var expanded = (Expression<Func<Order, bool>>)predicate.ExpandExpressives();

        var results = await Runner.WhereAsync(expanded);

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(1, results[0].Id);
    }

    [TestMethod]
    public async Task Where_DetailedSummary_ConcatArrayTranslatesToSql()
    {
        // DetailedSummary has 7 string parts, so the emitter produces string.Concat(string[]).
        // FlattenConcatArrayCalls rewrites it to chained Concat calls for EF Core.
        // Using Where (not Select) proves it translates server-side — Where throws if not.
        Expression<Func<Order, bool>> predicate = o => o.DetailedSummary.StartsWith("Order #1: RUSH");
        var expanded = (Expression<Func<Order, bool>>)predicate.ExpandExpressives();

        var results = await Runner.WhereAsync(expanded);

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(1, results[0].Id);
    }
}
