namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;

/// <summary>
/// Represents a window specification after PARTITION BY — can add more partitions or start ordering.
/// Returned by <see cref="Window.PartitionBy{TKey}"/>.
/// </summary>
public sealed class PartitionedWindowDefinition
{
    private PartitionedWindowDefinition() =>
        throw new InvalidOperationException("PartitionedWindowDefinition is a marker type for expression trees and cannot be instantiated.");

    /// <summary>Adds another PARTITION BY column.</summary>
    public PartitionedWindowDefinition PartitionBy<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Starts the ORDER BY clause (ascending).</summary>
    public OrderedWindowDefinition OrderBy<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Starts the ORDER BY clause (descending).</summary>
    public OrderedWindowDefinition OrderByDescending<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");
}

/// <summary>
/// Represents a window specification after ORDER BY — can add more orderings via ThenBy.
/// This is the type accepted by <see cref="WindowFunction"/> methods, ensuring at least one
/// ORDER BY clause is present at compile time.
/// </summary>
public sealed class OrderedWindowDefinition
{
    private OrderedWindowDefinition() =>
        throw new InvalidOperationException("OrderedWindowDefinition is a marker type for expression trees and cannot be instantiated.");

    /// <summary>Adds a subsequent ORDER BY column (ascending).</summary>
    public OrderedWindowDefinition ThenBy<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Adds a subsequent ORDER BY column (descending).</summary>
    public OrderedWindowDefinition ThenByDescending<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");
}
