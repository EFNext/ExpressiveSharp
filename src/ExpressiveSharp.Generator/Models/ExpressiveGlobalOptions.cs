using Microsoft.CodeAnalysis.Diagnostics;

namespace ExpressiveSharp.Generator.Models;

/// <summary>
/// Plain-data snapshot of the MSBuild global defaults for [Expressive] options.
/// Read from <c>build_property.*</c> entries in <c>AnalyzerConfigOptions.GlobalOptions</c>.
/// </summary>
readonly internal record struct ExpressiveGlobalOptions
{
    public ExpressiveGlobalOptions(AnalyzerConfigOptions globalOptions)
    {
        // Future: MSBuild properties for global transformer defaults can be read here.
    }
}
