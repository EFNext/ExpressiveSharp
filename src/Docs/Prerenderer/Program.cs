using System.Text.Json;
using System.Text.Json.Serialization;
using ExpressiveSharp.Docs.Playground.Core.Services;
using ExpressiveSharp.Docs.Prerenderer;

// Treat `new DateTime(...)` literals (Kind=Unspecified) as UTC so Npgsql accepts
// them for `timestamp with time zone` columns — samples stay free of SpecifyKind.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var docsRoot = "../../docs";
for (var i = 0; i < args.Length - 1; i++)
{
    if (args[i] == "--docs-root")
    {
        docsRoot = args[i + 1];
        break;
    }
}

docsRoot = Path.GetFullPath(docsRoot);
if (!Directory.Exists(docsRoot))
{
    Console.Error.WriteLine($"Docs root not found: {docsRoot}");
    return 1;
}

Console.WriteLine($"Docs root: {docsRoot}");

var references = new LocalPlaygroundReferences();
references.Load();

var compiler = new SnippetCompiler(references);

// Skip VitePress build artifacts and node_modules.
var mdFiles = Directory.GetFiles(docsRoot, "*.md", SearchOption.AllDirectories)
    .Where(f =>
    {
        var rel = Path.GetRelativePath(docsRoot, f).Replace('\\', '/');
        return !rel.StartsWith(".vitepress/dist/", StringComparison.OrdinalIgnoreCase)
            && !rel.StartsWith(".vitepress/cache/", StringComparison.OrdinalIgnoreCase)
            && !rel.Contains("/node_modules/", StringComparison.OrdinalIgnoreCase);
    })
    .ToArray();
var allSamples = new List<(string relativePath, DocSample sample)>();

foreach (var mdFile in mdFiles)
{
    var content = File.ReadAllText(mdFile);
    var relativePath = Path.GetRelativePath(docsRoot, mdFile).Replace('\\', '/');
    var samples = SampleExtractor.Extract(relativePath, content);
    foreach (var sample in samples)
        allSamples.Add((relativePath, sample));
}

if (allSamples.Count == 0)
{
    Console.WriteLine("No ::: expressive-sample blocks found.");
    return 0;
}

Console.WriteLine($"Found {allSamples.Count} sample(s) across {allSamples.Select(s => s.relativePath).Distinct().Count()} file(s).");

await using var renderer = new SampleRenderer(compiler);
var failed = 0;
var byFile = new Dictionary<string, List<RenderedSample>>(StringComparer.Ordinal);

foreach (var (relativePath, sample) in allSamples)
{
    try
    {
        var rendered = renderer.Render(sample);
        if (!byFile.TryGetValue(relativePath, out var list))
        {
            list = new List<RenderedSample>();
            byFile[relativePath] = list;
        }
        list.Add(rendered);
        Console.WriteLine($"  OK: {relativePath} [{sample.Index}] ({sample.StableKey})");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"  FAIL: {relativePath} [{sample.Index}]: {ex.Message}");
        failed++;
    }
}

var outputDir = Path.Combine(docsRoot, ".vitepress", "data", "samples");
Directory.CreateDirectory(outputDir);

var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
};

foreach (var (relativePath, samples) in byFile)
{
    var jsonPath = Path.Combine(outputDir, Path.ChangeExtension(relativePath, ".json"));
    Directory.CreateDirectory(Path.GetDirectoryName(jsonPath)!);

    var json = JsonSerializer.Serialize(samples, jsonOptions);

    // Only overwrite if content changed, to keep git diffs clean.
    if (File.Exists(jsonPath) && File.ReadAllText(jsonPath) == json)
    {
        Console.WriteLine($"  Unchanged: {Path.GetRelativePath(docsRoot, jsonPath)}");
        continue;
    }

    File.WriteAllText(jsonPath, json);
    Console.WriteLine($"  Wrote: {Path.GetRelativePath(docsRoot, jsonPath)}");
}

if (failed > 0)
{
    Console.Error.WriteLine($"\n{failed} sample(s) failed to compile. Docs examples must compile successfully.");
    return 1;
}

Console.WriteLine($"\nPre-rendered {allSamples.Count} sample(s) successfully.");
return 0;
