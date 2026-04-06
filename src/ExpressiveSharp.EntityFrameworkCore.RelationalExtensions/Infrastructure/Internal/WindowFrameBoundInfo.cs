namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;

/// <summary>
/// Kind of a SQL window frame boundary — maps 1:1 to the five forms of
/// the SQL:2003 frame-bound grammar.
/// </summary>
internal enum WindowFrameBoundKind
{
    UnboundedPreceding,
    Preceding,
    CurrentRow,
    Following,
    UnboundedFollowing,
}

/// <summary>
/// Fully-resolved description of a single frame boundary carried through the
/// translation pipeline. The <see cref="Offset"/> is only populated for
/// <see cref="WindowFrameBoundKind.Preceding"/> and <see cref="WindowFrameBoundKind.Following"/>;
/// it is stored as a literal integer because SQL requires literal constants for
/// frame-bound offsets (parameters are not allowed in the frame clause).
/// </summary>
internal readonly record struct WindowFrameBoundInfo(WindowFrameBoundKind Kind, int? Offset)
{
    /// <summary>Emits the SQL fragment for this boundary (e.g. <c>3 PRECEDING</c>, <c>CURRENT ROW</c>).</summary>
    public string ToSqlFragment() => Kind switch
    {
        WindowFrameBoundKind.UnboundedPreceding => "UNBOUNDED PRECEDING",
        WindowFrameBoundKind.Preceding => $"{Offset} PRECEDING",
        WindowFrameBoundKind.CurrentRow => "CURRENT ROW",
        WindowFrameBoundKind.Following => $"{Offset} FOLLOWING",
        WindowFrameBoundKind.UnboundedFollowing => "UNBOUNDED FOLLOWING",
        _ => throw new InvalidOperationException($"Unknown WindowFrameBoundKind: {Kind}"),
    };
}
