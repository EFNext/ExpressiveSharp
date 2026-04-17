using ExpressiveSharp.Mapping;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

/// <summary>
/// Represents an external (e.g. third-party) class with <b>instance</b>
/// methods and properties. Demonstrates two <c>[ExpressiveFor]</c> styles in one model:
/// <list type="bullet">
///   <item>The <c>Wrap</c> mapping uses the ergonomic co-located form — an <b>instance stub</b>
///         with the <b>single-argument</b> attribute, where the stub's <c>this</c> is the receiver.</item>
///   <item>The <c>Label</c> mapping uses the original external form — a static stub in
///         <see cref="DisplayFormatterMappings"/> with the explicit receiver parameter.</item>
/// </list>
/// Integration tests assert both paths produce identical runtime behavior, so regressions
/// in either the new or the legacy form are caught end-to-end.
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

    // Single-arg + instance-stub form: the target is Wrap on this type, and the stub's `this`
    // is the receiver. Equivalent to [ExpressiveFor(typeof(DisplayFormatter), nameof(Wrap))]
    // with a static stub taking `(DisplayFormatter, string)`.
    [ExpressiveFor(nameof(Wrap))]
    string WrapExpr(string value) => Prefix + value + Suffix;
}

/// <summary>
/// <c>[ExpressiveFor]</c> mapping for <see cref="DisplayFormatter.Label"/> using the original
/// external-class form (static stub with explicit receiver parameter).
/// </summary>
static class DisplayFormatterMappings
{
    [ExpressiveFor(typeof(DisplayFormatter), nameof(DisplayFormatter.Label))]
    static string Label(DisplayFormatter formatter)
        => "[" + formatter.Prefix + "/" + formatter.Suffix + "]";
}
