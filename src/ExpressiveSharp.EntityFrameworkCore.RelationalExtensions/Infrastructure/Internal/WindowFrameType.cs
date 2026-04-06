namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;

/// <summary>
/// SQL window frame type — determines whether the frame is row-based (<c>ROWS</c>) or
/// value-based (<c>RANGE</c>).
/// </summary>
internal enum WindowFrameType
{
    Rows,
    Range,
}
