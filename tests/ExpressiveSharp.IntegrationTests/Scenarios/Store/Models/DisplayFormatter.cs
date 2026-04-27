using ExpressiveSharp.Mapping;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

// Exercises both [ExpressiveFor] styles: Wrap uses co-located instance-stub form;
// Label uses static stub with explicit receiver in DisplayFormatterMappings.
public class DisplayFormatter
{
    public string Prefix { get; }
    public string Suffix { get; }

    public DisplayFormatter(string prefix, string suffix)
    {
        Prefix = prefix;
        Suffix = suffix;
    }

    public string Wrap(string value) => Prefix + value + Suffix;

    public string Label => "[" + Prefix + "/" + Suffix + "]";

    // Single-arg + instance-stub form: the target is Wrap on this type, and the stub's `this`
    // is the receiver. Equivalent to [ExpressiveFor(typeof(DisplayFormatter), nameof(Wrap))]
    // with a static stub taking `(DisplayFormatter, string)`.
    [ExpressiveFor(nameof(Wrap))]
    string WrapExpr(string value) => Prefix + value + Suffix;
}

static class DisplayFormatterMappings
{
    [ExpressiveFor(typeof(DisplayFormatter), nameof(DisplayFormatter.Label))]
    static string Label(DisplayFormatter formatter)
        => "[" + formatter.Prefix + "/" + formatter.Suffix + "]";
}
