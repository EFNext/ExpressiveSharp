using System.Security.Cryptography;
using System.Text;

namespace ExpressiveSharp.Docs.Prerenderer;

internal sealed record DocSample(
    string FilePath,
    int Index,
    string Snippet,
    string? Setup,
    string ScenarioId,
    string StableKey);

internal static class SampleExtractor
{
    private const string ContainerOpen = "::: expressive-sample";
    private const string ContainerClose = ":::";
    private const string SetupSeparator = "---setup---";

    public static List<DocSample> Extract(string filePath, string content)
    {
        var samples = new List<DocSample>();
        var lines = content.Split('\n');
        var i = 0;

        while (i < lines.Length)
        {
            var trimmed = lines[i].TrimStart();
            if (!trimmed.StartsWith(ContainerOpen, StringComparison.Ordinal))
            {
                i++;
                continue;
            }

            // Parse optional scenario from the opening line: "::: expressive-sample webshop"
            var scenarioId = "webshop";
            var afterMarker = trimmed[ContainerOpen.Length..].Trim();
            if (afterMarker.Length > 0)
                scenarioId = afterMarker;

            i++;
            var bodyLines = new List<string>();
            while (i < lines.Length)
            {
                var closeTrimmed = lines[i].TrimStart();
                if (closeTrimmed == ContainerClose || closeTrimmed.StartsWith(ContainerClose + " ", StringComparison.Ordinal))
                    break;
                bodyLines.Add(lines[i]);
                i++;
            }
            i++; // skip the closing :::

            var body = string.Join('\n', bodyLines).Trim();
            var separatorIdx = body.IndexOf(SetupSeparator, StringComparison.Ordinal);

            string snippet;
            string? setup = null;

            if (separatorIdx >= 0)
            {
                snippet = body[..separatorIdx].Trim();
                setup = body[(separatorIdx + SetupSeparator.Length)..].Trim();
            }
            else
            {
                snippet = body;
            }

            var key = ComputeStableKey(snippet, setup);
            samples.Add(new DocSample(filePath, samples.Count, snippet, setup, scenarioId, key));
        }

        return samples;
    }

    public static string ComputeStableKey(string snippet, string? setup)
    {
        var input = snippet + "\0" + (setup ?? "");
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hash)[..12].ToLowerInvariant();
    }
}
