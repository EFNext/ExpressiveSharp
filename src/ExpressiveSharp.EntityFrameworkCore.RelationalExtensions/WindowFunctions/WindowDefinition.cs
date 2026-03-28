namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;

/// <summary>
/// Represents a window specification (PARTITION BY + ORDER BY clauses) for SQL window functions.
/// This type is a marker — instances are never created at runtime. The expression tree
/// containing calls to its methods is translated by the EF Core method call translator.
/// </summary>
public sealed class WindowDefinition
{
    private WindowDefinition() =>
        throw new InvalidOperationException("WindowDefinition is a marker type for expression trees and cannot be instantiated.");

    /// <summary>Adds an ORDER BY ASC clause to the window specification.</summary>
    public WindowDefinition OrderBy<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Adds an ORDER BY DESC clause to the window specification.</summary>
    public WindowDefinition OrderByDescending<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Adds a subsequent ORDER BY ASC clause to the window specification.</summary>
    public WindowDefinition ThenBy<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Adds a subsequent ORDER BY DESC clause to the window specification.</summary>
    public WindowDefinition ThenByDescending<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");

    /// <summary>Adds a PARTITION BY clause to the window specification.</summary>
    public WindowDefinition PartitionBy<TKey>(TKey key) =>
        throw new InvalidOperationException("This method is translated to SQL and cannot be called directly.");
}
