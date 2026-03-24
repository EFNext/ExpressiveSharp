using Microsoft.CodeAnalysis.Diagnostics;

namespace ExpressiveSharp.Generator.Models;

/// <summary>
/// Plain-data snapshot of the MSBuild global defaults for [Expressive] options.
/// Read from <c>build_property.*</c> entries in <c>AnalyzerConfigOptions.GlobalOptions</c>.
/// </summary>
readonly internal record struct ExpressiveGlobalOptions
{
    /// <summary>
    /// Global default for <see cref="ExpressiveAttribute.AllowBlockBody"/>.
    /// Set via MSBuild property <c>Expressive_AllowBlockBody</c>. Defaults to <c>false</c>.
    /// </summary>
    public bool AllowBlockBody { get; }

    public ExpressiveGlobalOptions(AnalyzerConfigOptions globalOptions)
    {
        AllowBlockBody = globalOptions.TryGetValue("build_property.Expressive_AllowBlockBody", out var value)
            && bool.TryParse(value, out var parsed) && parsed;
    }
}
