// EF1001 fires on the ".Internal" namespace convention, but these are OUR internals
// (ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal) —
// these tests deliberately exercise them via InternalsVisibleTo.
#pragma warning disable EF1001

using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.Tests.RelationalExtensions;

/// <summary>
/// Unit tests for the internal <see cref="WindowFrameBoundSqlExpression"/> intermediate
/// node and the <see cref="WindowFrameBoundInfo"/> struct's SQL fragment formatting.
/// </summary>
[TestClass]
public class WindowFrameBoundSqlExpressionTests
{
    [TestMethod]
    public void ToSqlFragment_FormatsEachBoundKindCorrectly()
    {
        Assert.AreEqual("UNBOUNDED PRECEDING",
            new WindowFrameBoundInfo(WindowFrameBoundKind.UnboundedPreceding, null).ToSqlFragment());
        Assert.AreEqual("3 PRECEDING",
            new WindowFrameBoundInfo(WindowFrameBoundKind.Preceding, 3).ToSqlFragment());
        Assert.AreEqual("CURRENT ROW",
            new WindowFrameBoundInfo(WindowFrameBoundKind.CurrentRow, null).ToSqlFragment());
        Assert.AreEqual("5 FOLLOWING",
            new WindowFrameBoundInfo(WindowFrameBoundKind.Following, 5).ToSqlFragment());
        Assert.AreEqual("UNBOUNDED FOLLOWING",
            new WindowFrameBoundInfo(WindowFrameBoundKind.UnboundedFollowing, null).ToSqlFragment());
    }

    [TestMethod]
    public void Equals_ComparesBoundInfo()
    {
        var a = new WindowFrameBoundSqlExpression(new WindowFrameBoundInfo(WindowFrameBoundKind.Preceding, 3));
        var b = new WindowFrameBoundSqlExpression(new WindowFrameBoundInfo(WindowFrameBoundKind.Preceding, 3));
        var c = new WindowFrameBoundSqlExpression(new WindowFrameBoundInfo(WindowFrameBoundKind.Preceding, 5));
        var d = new WindowFrameBoundSqlExpression(new WindowFrameBoundInfo(WindowFrameBoundKind.Following, 3));

        Assert.AreEqual(a, b);
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        Assert.AreNotEqual(a, c);
        Assert.AreNotEqual(a, d);
    }

    [TestMethod]
    public void BoundInfo_PropertyRoundtrips()
    {
        var info = new WindowFrameBoundInfo(WindowFrameBoundKind.Following, 7);
        var expr = new WindowFrameBoundSqlExpression(info);
        Assert.AreEqual(info, expr.BoundInfo);
    }
}
