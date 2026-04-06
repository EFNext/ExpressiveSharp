// EF1001 fires on the ".Internal" namespace convention, but these are OUR internals —
// these tests deliberately exercise them via InternalsVisibleTo.
#pragma warning disable EF1001

using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.Tests.RelationalExtensions;

/// <summary>
/// Unit tests for the frame-related extensions to <see cref="WindowSpecSqlExpression"/>:
/// the <c>WithFrame</c> builder method and the equality/hash-code behaviour for the
/// new frame fields.
/// </summary>
[TestClass]
public class WindowSpecSqlExpressionFrameTests
{
    private static readonly SqlFragmentExpression ColA = new("[a]");
    private static readonly SqlFragmentExpression ColB = new("[b]");

    [TestMethod]
    public void Constructor_DefaultsFrameFieldsToNull()
    {
        var spec = new WindowSpecSqlExpression([ColA], [new OrderingExpression(ColB, ascending: true)], typeMapping: null);

        Assert.IsNull(spec.FrameType);
        Assert.IsNull(spec.FrameStart);
        Assert.IsNull(spec.FrameEnd);
    }

    [TestMethod]
    public void WithFrame_PreservesPartitionsAndOrderings()
    {
        var spec = new WindowSpecSqlExpression(
            [ColA],
            [new OrderingExpression(ColB, ascending: false)],
            typeMapping: null);

        var framed = spec.WithFrame(
            WindowFrameType.Rows,
            new WindowFrameBoundInfo(WindowFrameBoundKind.UnboundedPreceding, null),
            new WindowFrameBoundInfo(WindowFrameBoundKind.CurrentRow, null));

        Assert.AreEqual(1, framed.Partitions.Count);
        Assert.AreSame(ColA, framed.Partitions[0]);
        Assert.AreEqual(1, framed.Orderings.Count);
        Assert.AreSame(ColB, framed.Orderings[0].Expression);
        Assert.IsFalse(framed.Orderings[0].IsAscending);
        Assert.AreEqual(WindowFrameType.Rows, framed.FrameType);
        Assert.AreEqual(WindowFrameBoundKind.UnboundedPreceding, framed.FrameStart!.Value.Kind);
        Assert.AreEqual(WindowFrameBoundKind.CurrentRow, framed.FrameEnd!.Value.Kind);
    }

    [TestMethod]
    public void WithFrame_ReturnsNewInstance()
    {
        var spec = new WindowSpecSqlExpression([], [new OrderingExpression(ColA, ascending: true)], typeMapping: null);
        var framed = spec.WithFrame(
            WindowFrameType.Range,
            new WindowFrameBoundInfo(WindowFrameBoundKind.Preceding, 3),
            new WindowFrameBoundInfo(WindowFrameBoundKind.Following, 3));

        Assert.AreNotSame(spec, framed);
        Assert.IsNull(spec.FrameType);
    }

    [TestMethod]
    public void Equals_IncludesFrameFields()
    {
        var orderings = new[] { new OrderingExpression(ColA, ascending: true) };
        var a = new WindowSpecSqlExpression([], orderings, typeMapping: null)
            .WithFrame(WindowFrameType.Rows,
                new WindowFrameBoundInfo(WindowFrameBoundKind.UnboundedPreceding, null),
                new WindowFrameBoundInfo(WindowFrameBoundKind.CurrentRow, null));
        var b = new WindowSpecSqlExpression([], orderings, typeMapping: null)
            .WithFrame(WindowFrameType.Rows,
                new WindowFrameBoundInfo(WindowFrameBoundKind.UnboundedPreceding, null),
                new WindowFrameBoundInfo(WindowFrameBoundKind.CurrentRow, null));
        var differentFrameType = new WindowSpecSqlExpression([], orderings, typeMapping: null)
            .WithFrame(WindowFrameType.Range,
                new WindowFrameBoundInfo(WindowFrameBoundKind.UnboundedPreceding, null),
                new WindowFrameBoundInfo(WindowFrameBoundKind.CurrentRow, null));
        var differentBound = new WindowSpecSqlExpression([], orderings, typeMapping: null)
            .WithFrame(WindowFrameType.Rows,
                new WindowFrameBoundInfo(WindowFrameBoundKind.Preceding, 2),
                new WindowFrameBoundInfo(WindowFrameBoundKind.CurrentRow, null));

        Assert.AreEqual(a, b);
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        Assert.AreNotEqual(a, differentFrameType);
        Assert.AreNotEqual(a, differentBound);
    }

    [TestMethod]
    public void WithPartition_PreservesFrame()
    {
        var spec = new WindowSpecSqlExpression([], [new OrderingExpression(ColA, ascending: true)], typeMapping: null)
            .WithFrame(WindowFrameType.Rows,
                new WindowFrameBoundInfo(WindowFrameBoundKind.UnboundedPreceding, null),
                new WindowFrameBoundInfo(WindowFrameBoundKind.CurrentRow, null));

        var withPartition = spec.WithPartition(ColB);

        Assert.AreEqual(WindowFrameType.Rows, withPartition.FrameType);
        Assert.AreEqual(1, withPartition.Partitions.Count);
    }

    [TestMethod]
    public void WithOrdering_PreservesFrame()
    {
        var spec = new WindowSpecSqlExpression([], [new OrderingExpression(ColA, ascending: true)], typeMapping: null)
            .WithFrame(WindowFrameType.Range,
                new WindowFrameBoundInfo(WindowFrameBoundKind.UnboundedPreceding, null),
                new WindowFrameBoundInfo(WindowFrameBoundKind.UnboundedFollowing, null));

        var withOrdering = spec.WithOrdering(ColB, ascending: false);

        Assert.AreEqual(WindowFrameType.Range, withOrdering.FrameType);
        Assert.AreEqual(2, withOrdering.Orderings.Count);
    }
}
