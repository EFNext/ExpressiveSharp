// In Blazor WASM every loaded assembly ships under /_framework/<name>.dll, so
// HttpClient pulls the raw bytes and feeds them to MetadataReference.CreateFromImage.
// The reference set is the union of every scenario's ReferenceAssemblies —
// adding a scenario to ScenarioRegistry automatically extends the set.

using System.Collections.Immutable;
using Basic.Reference.Assemblies;
using ExpressiveSharp.Docs.Playground.Core.Services;
using ExpressiveSharp.Docs.Playground.Core.Services.Scenarios;
using Microsoft.CodeAnalysis;

namespace ExpressiveSharp.Docs.Playground.Wasm.Services;

internal sealed class PlaygroundReferences : IPlaygroundReferences
{
    private readonly HttpClient _http;
    private ImmutableArray<MetadataReference> _references;

    public PlaygroundReferences(HttpClient http)
    {
        _http = http;
    }

    public ImmutableArray<MetadataReference> References => _references;

    public bool IsLoaded => !_references.IsDefault;

    public async Task LoadAsync()
    {
        if (IsLoaded) return;

        var builder = ImmutableArray.CreateBuilder<MetadataReference>();

        // .NET 10 ref-only BCL — embedded resources in Basic.Reference.Assemblies.Net100.
        builder.AddRange(Net100.References.All);

        var seenNames = new HashSet<string>(StringComparer.Ordinal);
        var fetchTasks = new List<Task<MetadataReference?>>();

        foreach (var scenario in ScenarioRegistry.All)
        {
            foreach (var assembly in scenario.ReferenceAssemblies)
            {
                var name = assembly.GetName().Name;
                if (name is null || !seenNames.Add(name)) continue;
                fetchTasks.Add(FetchAsync(name));
            }
        }

        var fetched = await Task.WhenAll(fetchTasks);
        foreach (var reference in fetched)
            if (reference is not null)
                builder.Add(reference);

        _references = builder.ToImmutable();
    }

    private async Task<MetadataReference?> FetchAsync(string assemblyName)
    {
        var url = $"_framework/{assemblyName}.dll";
        try
        {
            var bytes = await _http.GetByteArrayAsync(url);
            return MetadataReference.CreateFromImage(bytes, filePath: assemblyName + ".dll");
        }
        catch (HttpRequestException)
        {
            // Missing DLL → skip; Roslyn reports "missing reference" later if needed.
            return null;
        }
    }
}
