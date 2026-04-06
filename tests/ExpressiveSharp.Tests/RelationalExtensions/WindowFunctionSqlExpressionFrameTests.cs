// EF1001 fires on the ".Internal" namespace convention, but these are OUR internals —
// these tests deliberately exercise them via InternalsVisibleTo.
#pragma warning disable EF1001

using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.Tests.RelationalExtensions;

/// <summary>
/// Unit tests for the frame-clause rendering in <see cref="WindowFunctionSqlExpression"/>.
/// Uses aggregate function names (SUM, AVG, COUNT) since frames only apply to aggregates
/// — the SQL standard forbids frames on ranking functions.
/// </summary>
[TestClass]
public class WindowFunctionSqlExpressionFrameTests
{
    private static readonly SqlFragmentExpression ColPrice = new("[Price]");
    private static readonly SqlFragmentExpression ColCustomerId = new("[CustomerId]");

    private static string PrintExpression(WindowFunctionSqlExpression expr)
    {
        var printer = new ExpressionPrinter();
        printer.Visit(expr);
        return printer.ToString();
    }

    [TestMethod]
    public void Print_NoFrame_OmitsFrameClause()
    {
        var expr = new WindowFunctionSqlExpression(
            "SUM",
            arguments: [ColPrice],
            partitions: [],
            orderings: [new OrderingExpression(ColPrice, ascending: true)],
            type: typeof(double),
            typeMapping: null);

        var printed = PrintExpression(expr);
        Assert.AreEqual("SUM([Price]) OVER(ORDER BY [Price] ASC)", printed);
    }

    [TestMethod]
    public void Print_RowsBetweenUnboundedPrecedingAndCurrentRow()
    {
        var expr = new WindowFunctionSqlExpression(
            "SUM",
            arguments: [ColPrice],
            partitions: [],
            orderings: [new OrderingExpression(ColPrice, ascending: true)],
            type: typeof(double),
            typeMapping: null,
            frameType: WindowFrameType.Rows,
            frameStart: new WindowFrameBoundInfo(WindowFrameBoundKind.UnboundedPreceding, null),
            frameEnd: new WindowFrameBoundInfo(WindowFrameBoundKind.CurrentRow, null));

        var printed = PrintExpression(expr);
        Assert.AreEqual(
            "SUM([Price]) OVER(ORDER BY [Price] ASC ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)",
            printed);
    }

    [TestMethod]
    public void Print_RowsBetweenNumericPrecedingAndFollowing()
    {
        var expr = new WindowFunctionSqlExpression(
            "AVG",
            arguments: [ColPrice],
            partitions: [],
            orderings: [new OrderingExpression(ColPrice, ascending: false)],
            type: typeof(double),
            typeMapping: null,
            frameType: WindowFrameType.Rows,
            frameStart: new WindowFrameBoundInfo(WindowFrameBoundKind.Preceding, 3),
            frameEnd: new WindowFrameBoundInfo(WindowFrameBoundKind.Following, 3));

        var printed = PrintExpression(expr);
        Assert.AreEqual(
            "AVG([Price]) OVER(ORDER BY [Price] DESC ROWS BETWEEN 3 PRECEDING AND 3 FOLLOWING)",
            printed);
    }

    [TestMethod]
    public void Print_RangeBetweenUnboundedPrecedingAndCurrentRow()
    {
        var expr = new WindowFunctionSqlExpression(
            "SUM",
            arguments: [ColPrice],
            partitions: [],
            orderings: [new OrderingExpression(ColPrice, ascending: true)],
            type: typeof(double),
            typeMapping: null,
            frameType: WindowFrameType.Range,
            frameStart: new WindowFrameBoundInfo(WindowFrameBoundKind.UnboundedPreceding, null),
            frameEnd: new WindowFrameBoundInfo(WindowFrameBoundKind.CurrentRow, null));

        var printed = PrintExpression(expr);
        Assert.AreEqual(
            "SUM([Price]) OVER(ORDER BY [Price] ASC RANGE BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)",
            printed);
    }

    [TestMethod]
    public void Print_RowsBetweenUnboundedPrecedingAndUnboundedFollowing()
    {
        var expr = new WindowFunctionSqlExpression(
            "COUNT",
            arguments: [new SqlFragmentExpression("*")],
            partitions: [],
            orderings: [new OrderingExpression(ColPrice, ascending: true)],
            type: typeof(long),
            typeMapping: null,
            frameType: WindowFrameType.Rows,
            frameStart: new WindowFrameBoundInfo(WindowFrameBoundKind.UnboundedPreceding, null),
            frameEnd: new WindowFrameBoundInfo(WindowFrameBoundKind.UnboundedFollowing, null));

        var printed = PrintExpression(expr);
        Assert.AreEqual(
            "COUNT(*) OVER(ORDER BY [Price] ASC ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING)",
            printed);
    }

    [TestMethod]
    public void Print_WithPartitionOrderAndFrame()
    {
        var expr = new WindowFunctionSqlExpression(
            "SUM",
            arguments: [ColPrice],
            partitions: [ColCustomerId],
            orderings: [new OrderingExpression(ColPrice, ascending: true)],
            type: typeof(double),
            typeMapping: null,
            frameType: WindowFrameType.Rows,
            frameStart: new WindowFrameBoundInfo(WindowFrameBoundKind.UnboundedPreceding, null),
            frameEnd: new WindowFrameBoundInfo(WindowFrameBoundKind.CurrentRow, null));

        var printed = PrintExpression(expr);
        Assert.AreEqual(
            "SUM([Price]) OVER(PARTITION BY [CustomerId] ORDER BY [Price] ASC ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)",
            printed);
    }

    [TestMethod]
    public void Print_MinWithNumericOffsets()
    {
        var expr = new WindowFunctionSqlExpression(
            "MIN",
            arguments: [ColPrice],
            partitions: [],
            orderings: [new OrderingExpression(ColPrice, ascending: true)],
            type: typeof(double),
            typeMapping: null,
            frameType: WindowFrameType.Rows,
            frameStart: new WindowFrameBoundInfo(WindowFrameBoundKind.Preceding, 2),
            frameEnd: new WindowFrameBoundInfo(WindowFrameBoundKind.Following, 2));

        var printed = PrintExpression(expr);
        Assert.AreEqual(
            "MIN([Price]) OVER(ORDER BY [Price] ASC ROWS BETWEEN 2 PRECEDING AND 2 FOLLOWING)",
            printed);
    }

    [TestMethod]
    public void Equals_IncludesFrameFields()
    {
        WindowFunctionSqlExpression MakeExpr(WindowFrameBoundKind startKind, int? startOffset) =>
            new("SUM", [ColPrice], [], [new OrderingExpression(ColPrice, ascending: true)], typeof(double), null,
                WindowFrameType.Rows,
                new WindowFrameBoundInfo(startKind, startOffset),
                new WindowFrameBoundInfo(WindowFrameBoundKind.CurrentRow, null));

        var a = MakeExpr(WindowFrameBoundKind.Preceding, 2);
        var b = MakeExpr(WindowFrameBoundKind.Preceding, 2);
        var c = MakeExpr(WindowFrameBoundKind.Preceding, 3);

        Assert.AreEqual(a, b);
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        Assert.AreNotEqual(a, c);
    }
}
