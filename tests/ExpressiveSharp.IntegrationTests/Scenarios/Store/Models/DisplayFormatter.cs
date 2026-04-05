using ExpressiveSharp.Mapping;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

/// <summary>
/// Represents an external (e.g. third-party) class with <b>instance</b>
/// methods and properties that test code cannot mark with
/// <c>[Expressive]</c> directly. <see cref="DisplayFormatterMappings"/>
/// provides <c>[ExpressiveFor]</c> bodies so its members can be used inside
/// LINQ expression trees.
/// </summary>
public class DisplayFormatter
{
    public string Prefix { get; }
    public string Suffix { get; }

    public DisplayFormatter(string prefix, string suffix)
    {
        Prefix = prefix;
        Suffix = suffix;
    }

    /// <summary>Wraps a string with the configured prefix/suffix — instance method.</summary>
    public string Wrap(string value) => Prefix + value + Suffix;

    /// <summary>Label that combines the prefix/suffix — instance property.</summary>
    public string Label => "[" + Prefix + "/" + Suffix + "]";
}

/// <summary>
/// <c>[ExpressiveFor]</c> mappings for <see cref="DisplayFormatter"/>.
/// Each mapping takes the target instance as its first parameter.
/// </summary>
static class DisplayFormatterMappings
{
    [ExpressiveFor(typeof(DisplayFormatter), nameof(DisplayFormatter.Wrap))]
    static string Wrap(DisplayFormatter formatter, string value)
        => formatter.Prefix + value + formatter.Suffix;

    [ExpressiveFor(typeof(DisplayFormatter), nameof(DisplayFormatter.Label))]
    static string Label(DisplayFormatter formatter)
        => "[" + formatter.Prefix + "/" + formatter.Suffix + "]";
}
