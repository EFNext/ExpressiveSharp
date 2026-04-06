namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;

/// <summary>
/// Provides SQL window function stubs for use in EF Core LINQ queries.
/// These methods are translated to SQL by ExpressiveSharp's method call
/// translator — they throw at runtime if called directly.
/// <para>
/// <b>Ranking functions</b> (ROW_NUMBER, RANK, DENSE_RANK, NTILE) accept
/// <see cref="OrderedWindowDefinition"/> only — the SQL standard forbids
/// frame clauses on ranking functions.
/// </para>
/// <para>
/// <b>Aggregate functions</b> (SUM, AVG, COUNT, MIN, MAX) accept both
/// <see cref="OrderedWindowDefinition"/> (uses SQL default frame) and
/// <see cref="FramedWindowDefinition"/> (explicit ROWS/RANGE BETWEEN).
/// </para>
/// </summary>
public static class WindowFunction
{
    // ── Ranking functions ────────────────────────────────────────────────

    /// <summary>
    /// Translates to <c>ROW_NUMBER() OVER(...)</c>.
    /// Returns a sequential number for each row within the window partition.
    /// </summary>
    public static long RowNumber(OrderedWindowDefinition window) =>
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
    public static long Rank(OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>
    /// Translates to <c>DENSE_RANK() OVER(...)</c>.
    /// Returns the rank of each row within the window partition, without gaps for ties.
    /// </summary>
    public static long DenseRank(OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>
    /// Translates to <c>NTILE(<paramref name="buckets"/>) OVER(...)</c>.
    /// Distributes rows into the specified number of roughly equal groups.
    /// </summary>
    public static long Ntile(int buckets, OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>
    /// Translates to <c>PERCENT_RANK() OVER(...)</c>.
    /// Returns the relative rank of each row as a value between 0.0 and 1.0.
    /// </summary>
    public static double PercentRank(OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>
    /// Translates to <c>CUME_DIST() OVER(...)</c>.
    /// Returns the cumulative distribution of each row as a value between 0.0 and 1.0.
    /// </summary>
    public static double CumeDist(OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    // ── Aggregate functions ──────────────────────────────────────────────

    /// <summary>Translates to <c>SUM(expression) OVER(...)</c>.</summary>
    public static T Sum<T>(T expression, OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <inheritdoc cref="Sum{T}(T, OrderedWindowDefinition)"/>
    public static T Sum<T>(T expression, FramedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Translates to <c>AVG(expression) OVER(...)</c>.</summary>
    public static T? Average<T>(T expression, OrderedWindowDefinition window) where T : struct =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <inheritdoc cref="Average{T}(T, OrderedWindowDefinition)"/>
    public static T? Average<T>(T expression, FramedWindowDefinition window) where T : struct =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    // int/long → double (matching Queryable.Average semantics)

    /// <summary>Translates to <c>AVG(expression) OVER(...)</c>. Returns <c>double</c> for integer input.</summary>
    public static double Average(int expression, OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <inheritdoc cref="Average(int, OrderedWindowDefinition)"/>
    public static double Average(int expression, FramedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <inheritdoc cref="Average(int, OrderedWindowDefinition)"/>
    public static double? Average(int? expression, OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <inheritdoc cref="Average(int, OrderedWindowDefinition)"/>
    public static double? Average(int? expression, FramedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Translates to <c>AVG(expression) OVER(...)</c>. Returns <c>double</c> for long input.</summary>
    public static double Average(long expression, OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <inheritdoc cref="Average(long, OrderedWindowDefinition)"/>
    public static double Average(long expression, FramedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <inheritdoc cref="Average(long, OrderedWindowDefinition)"/>
    public static double? Average(long? expression, OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <inheritdoc cref="Average(long, OrderedWindowDefinition)"/>
    public static double? Average(long? expression, FramedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Translates to <c>COUNT(*) OVER(...)</c>. Counts all rows in the window.</summary>
    public static int Count(OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <inheritdoc cref="Count(OrderedWindowDefinition)"/>
    public static int Count(FramedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Translates to <c>COUNT(expression) OVER(...)</c>. Counts non-null values.</summary>
    public static int Count<T>(T expression, OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <inheritdoc cref="Count{T}(T, OrderedWindowDefinition)"/>
    public static int Count<T>(T expression, FramedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Translates to <c>MIN(expression) OVER(...)</c>.</summary>
    public static T Min<T>(T expression, OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <inheritdoc cref="Min{T}(T, OrderedWindowDefinition)"/>
    public static T Min<T>(T expression, FramedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Translates to <c>MAX(expression) OVER(...)</c>.</summary>
    public static T Max<T>(T expression, OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <inheritdoc cref="Max{T}(T, OrderedWindowDefinition)"/>
    public static T Max<T>(T expression, FramedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    // ── Navigation functions ─────────────────────────────────────────────
    // LAG/LEAD access a row at a specific offset from the current row.
    // The SQL standard forbids frame clauses on these — OrderedWindowDefinition only.
    //
    // Without a default value, the SQL result is NULL when no row exists at the
    // requested offset. For value types, project into a nullable column explicitly:
    //   (double?)WindowFunction.Lag(o.Price, Window.OrderBy(o.Price))

    /// <summary>
    /// Translates to <c>LAG(expression) OVER(...)</c>. Returns the previous row's value (offset 1).
    /// The result is NULL when no previous row exists; cast to a nullable type if needed.
    /// </summary>
    public static T Lag<T>(T expression, OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Translates to <c>LAG(expression, <paramref name="offset"/>) OVER(...)</c>.</summary>
    public static T Lag<T>(T expression, int offset, OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Translates to <c>LAG(expression, <paramref name="offset"/>, <paramref name="defaultValue"/>) OVER(...)</c>.</summary>
    public static T Lag<T>(T expression, int offset, T defaultValue, OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>
    /// Translates to <c>LEAD(expression) OVER(...)</c>. Returns the next row's value (offset 1).
    /// The result is NULL when no next row exists; cast to a nullable type if needed.
    /// </summary>
    public static T Lead<T>(T expression, OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Translates to <c>LEAD(expression, <paramref name="offset"/>) OVER(...)</c>.</summary>
    public static T Lead<T>(T expression, int offset, OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Translates to <c>LEAD(expression, <paramref name="offset"/>, <paramref name="defaultValue"/>) OVER(...)</c>.</summary>
    public static T Lead<T>(T expression, int offset, T defaultValue, OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>
    /// Translates to <c>FIRST_VALUE(expression) OVER(...)</c>.
    /// Returns the first value in the window frame. The result depends on the frame;
    /// with the default frame this is the first row of the partition.
    /// </summary>
    public static T FirstValue<T>(T expression, OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <inheritdoc cref="FirstValue{T}(T, OrderedWindowDefinition)"/>
    public static T FirstValue<T>(T expression, FramedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>
    /// Translates to <c>LAST_VALUE(expression) OVER(...)</c>.
    /// Returns the last value in the window frame. With the default frame this returns
    /// the <em>current row's</em> value — use an explicit frame like
    /// <c>.RowsBetween(UnboundedPreceding, UnboundedFollowing)</c> to get the partition's last value.
    /// </summary>
    public static T LastValue<T>(T expression, OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <inheritdoc cref="LastValue{T}(T, OrderedWindowDefinition)"/>
    public static T LastValue<T>(T expression, FramedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>
    /// Translates to <c>NTH_VALUE(expression, <paramref name="n"/>) OVER(...)</c>.
    /// Returns the value at the Nth row in the window frame (1-based).
    /// </summary>
    public static T NthValue<T>(T expression, int n, OrderedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <inheritdoc cref="NthValue{T}(T, int, OrderedWindowDefinition)"/>
    public static T NthValue<T>(T expression, int n, FramedWindowDefinition window) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");
}
