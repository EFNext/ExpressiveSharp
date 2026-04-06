namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;

/// <summary>
/// Represents a boundary of a SQL window frame (e.g. <c>UNBOUNDED PRECEDING</c>,
/// <c>3 PRECEDING</c>, <c>CURRENT ROW</c>, <c>5 FOLLOWING</c>, <c>UNBOUNDED FOLLOWING</c>).
/// These factory members are translated to SQL by ExpressiveSharp's translators —
/// they throw at runtime if accessed directly.
/// </summary>
public sealed class WindowFrameBound
{
    private WindowFrameBound() =>
        throw new InvalidOperationException("WindowFrameBound is a marker type for expression trees and cannot be instantiated.");

    /// <summary>Translates to <c>UNBOUNDED PRECEDING</c>.</summary>
    public static WindowFrameBound UnboundedPreceding =>
        throw new InvalidOperationException("This property is translated to SQL and cannot be accessed directly.");

    /// <summary>Translates to <c><paramref name="offset"/> PRECEDING</c>.</summary>
    public static WindowFrameBound Preceding(int offset) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Translates to <c>CURRENT ROW</c>.</summary>
    public static WindowFrameBound CurrentRow =>
        throw new InvalidOperationException("This property is translated to SQL and cannot be accessed directly.");

    /// <summary>Translates to <c><paramref name="offset"/> FOLLOWING</c>.</summary>
    public static WindowFrameBound Following(int offset) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Translates to <c>UNBOUNDED FOLLOWING</c>.</summary>
    public static WindowFrameBound UnboundedFollowing =>
        throw new InvalidOperationException("This property is translated to SQL and cannot be accessed directly.");
}
