namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;

internal enum WindowFrameBoundKind
{
    UnboundedPreceding,
    Preceding,
    CurrentRow,
    Following,
    UnboundedFollowing,
}

/// <summary>
/// Single frame boundary carried through the translation pipeline. <see cref="Offset"/> is only
/// populated for Preceding/Following and is stored as a literal int — SQL requires literal
/// constants for frame-bound offsets (parameters are not allowed in the frame clause).
/// </summary>
internal readonly record struct WindowFrameBoundInfo(WindowFrameBoundKind Kind, int? Offset)
{
    public string ToSqlFragment() => Kind switch
    {
        WindowFrameBoundKind.UnboundedPreceding => "UNBOUNDED PRECEDING",
        WindowFrameBoundKind.Preceding => $"{ValidateOffset()} PRECEDING",
        WindowFrameBoundKind.CurrentRow => "CURRENT ROW",
        WindowFrameBoundKind.Following => $"{ValidateOffset()} FOLLOWING",
        WindowFrameBoundKind.UnboundedFollowing => "UNBOUNDED FOLLOWING",
        _ => throw new InvalidOperationException($"Unknown WindowFrameBoundKind: {Kind}"),
    };

    private int ValidateOffset()
    {
        if (!Offset.HasValue)
            throw new InvalidOperationException($"Window frame bound '{Kind}' requires a non-null offset.");
        if (Offset.Value < 0)
            throw new InvalidOperationException($"Window frame bound '{Kind}' requires a non-negative offset, but got {Offset.Value}.");
        return Offset.Value;
    }
}
