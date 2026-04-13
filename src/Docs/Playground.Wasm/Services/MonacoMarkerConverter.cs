using Microsoft.CodeAnalysis;

namespace ExpressiveSharp.Docs.Playground.Wasm.Services;

internal static class MonacoMarkerConverter
{
    public static List<MonacoMarkerData> ToMonaco(IReadOnlyList<SnippetMarker> markers)
    {
        var result = new List<MonacoMarkerData>(markers.Count);
        foreach (var m in markers)
        {
            result.Add(new MonacoMarkerData
            {
                Severity = ToMonacoSeverity(m.Severity),
                Message = $"{m.Code}: {m.Message}",
                StartLineNumber = m.StartLine,
                StartColumn = m.StartColumn,
                EndLineNumber = m.EndLine,
                EndColumn = m.EndColumn,
            });
        }
        return result;
    }

    private static int ToMonacoSeverity(DiagnosticSeverity severity) => severity switch
    {
        DiagnosticSeverity.Error => MonacoMarkerSeverity.Error,
        DiagnosticSeverity.Warning => MonacoMarkerSeverity.Warning,
        DiagnosticSeverity.Info => MonacoMarkerSeverity.Info,
        _ => MonacoMarkerSeverity.Hint,
    };
}
