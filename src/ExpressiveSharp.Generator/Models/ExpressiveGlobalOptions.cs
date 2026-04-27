using Microsoft.CodeAnalysis.Diagnostics;

namespace ExpressiveSharp.Generator.Models;

readonly internal record struct ExpressiveGlobalOptions
{
    // Set via MSBuild property `Expressive_AllowBlockBody`. Defaults to false.
    public bool AllowBlockBody { get; }

    public ExpressiveGlobalOptions(AnalyzerConfigOptions globalOptions)
    {
        AllowBlockBody = globalOptions.TryGetValue("build_property.Expressive_AllowBlockBody", out var value)
            && bool.TryParse(value, out var parsed) && parsed;
    }
}
