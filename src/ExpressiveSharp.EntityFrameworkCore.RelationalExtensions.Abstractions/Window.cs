namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;

/// <summary>
/// Entry point for building window specifications. Markers — translated to SQL; throw at runtime if called directly.
/// </summary>
public static class Window
{
    /// <summary>Starts a window with a PARTITION BY clause.</summary>
    public static PartitionedWindowDefinition PartitionBy<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Starts a window with ORDER BY (ascending).</summary>
    public static OrderedWindowDefinition OrderBy<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Starts a window with ORDER BY (descending).</summary>
    public static OrderedWindowDefinition OrderByDescending<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");
}
