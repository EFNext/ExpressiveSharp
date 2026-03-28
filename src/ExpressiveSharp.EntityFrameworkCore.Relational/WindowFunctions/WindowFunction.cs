namespace ExpressiveSharp.EntityFrameworkCore.Relational.WindowFunctions;

/// <summary>
/// Provides SQL window function stubs (ROW_NUMBER, RANK, DENSE_RANK, NTILE)
/// for use in EF Core LINQ queries. These methods are translated to SQL by
/// ExpressiveSharp's method call translator — they throw at runtime if called directly.
/// </summary>
public static class WindowFunction
{
    /// <summary>
    /// Translates to <c>ROW_NUMBER() OVER(...)</c>.
    /// Returns a sequential number for each row within the window partition.
    /// </summary>
    public static long RowNumber(WindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>
    /// Translates to <c>ROW_NUMBER() OVER()</c> with no ordering or partitioning.
    /// Row numbering is non-deterministic. Used internally by the indexed Select transformer.
    /// </summary>
    public static long RowNumber() =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>
    /// Translates to <c>RANK() OVER(...)</c>.
    /// Returns the rank of each row within the window partition, with gaps for ties.
    /// </summary>
    public static long Rank(WindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>
    /// Translates to <c>DENSE_RANK() OVER(...)</c>.
    /// Returns the rank of each row within the window partition, without gaps for ties.
    /// </summary>
    public static long DenseRank(WindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>
    /// Translates to <c>NTILE(<paramref name="buckets"/>) OVER(...)</c>.
    /// Distributes rows into the specified number of roughly equal groups.
    /// </summary>
    public static long Ntile(int buckets, WindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");
}
