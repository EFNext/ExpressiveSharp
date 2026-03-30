namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;

/// <summary>
/// Static entry point for building window specifications used in SQL window functions.
/// These methods are markers — they are translated to SQL by the EF Core query pipeline
/// and throw at runtime if called directly.
/// </summary>
public static class Window
{
    /// <summary>Creates a window specification starting with a PARTITION BY clause.</summary>
    public static PartitionedWindowDefinition PartitionBy<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Creates a window specification starting with an ORDER BY ASC clause.</summary>
    public static OrderedWindowDefinition OrderBy<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Creates a window specification starting with an ORDER BY DESC clause.</summary>
    public static OrderedWindowDefinition OrderByDescending<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");
}
