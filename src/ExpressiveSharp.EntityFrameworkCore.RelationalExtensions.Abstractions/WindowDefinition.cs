namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;

/// <summary>Window specification after PARTITION BY. Returned by <see cref="Window.PartitionBy{TKey}"/>.</summary>
public sealed class PartitionedWindowDefinition
{
    private PartitionedWindowDefinition() =>
        throw new InvalidOperationException("PartitionedWindowDefinition is a marker type for expression trees and cannot be instantiated.");

    public PartitionedWindowDefinition PartitionBy<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    public OrderedWindowDefinition OrderBy<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    public OrderedWindowDefinition OrderByDescending<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");
}

/// <summary>
/// Window specification after ORDER BY. The type accepted by <see cref="WindowFunction"/> methods,
/// ensuring at least one ORDER BY clause is present at compile time.
/// </summary>
public sealed class OrderedWindowDefinition
{
    private OrderedWindowDefinition() =>
        throw new InvalidOperationException("OrderedWindowDefinition is a marker type for expression trees and cannot be instantiated.");

    public OrderedWindowDefinition ThenBy<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    public OrderedWindowDefinition ThenByDescending<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary><c>ROWS BETWEEN <paramref name="start"/> AND <paramref name="end"/></c>.</summary>
    public FramedWindowDefinition RowsBetween(WindowFrameBound start, WindowFrameBound end) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary><c>RANGE BETWEEN <paramref name="start"/> AND <paramref name="end"/></c>.</summary>
    public FramedWindowDefinition RangeBetween(WindowFrameBound start, WindowFrameBound end) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");
}

/// <summary>Terminal window specification after a frame clause (ROWS/RANGE BETWEEN) has been applied.</summary>
public sealed class FramedWindowDefinition
{
    private FramedWindowDefinition() =>
        throw new InvalidOperationException("FramedWindowDefinition is a marker type for expression trees and cannot be instantiated.");
}
