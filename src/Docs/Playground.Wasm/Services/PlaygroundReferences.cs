// PlaygroundReferences — fetches the reference assemblies the SnippetCompiler
// needs into MetadataReference instances. In a Blazor WASM app every loaded
// assembly is shipped under /_framework/<name>.dll, so we use HttpClient to
// pull the raw bytes and feed them to MetadataReference.CreateFromImage.
//
// The reference set is the union of every scenario's ReferenceAssemblies,
// loaded once at startup and cached. Adding a new scenario in Phase 2
// automatically extends the reference set with its assemblies — no edits to
// this file are required.

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

        // .NET 10 ref-only BCL — embedded as resources in the
        // Basic.Reference.Assemblies.Net100 assembly. No HTTP needed; the
        // package's own embedded resources are loaded by the normal Blazor
        // assembly loader at startup.
        builder.AddRange(Net100.References.All);

        // Take the union of every scenario's ReferenceAssemblies. With one
        // scenario today this loads ExpressiveSharp + ExpressiveSharp.EntityFrameworkCore
        // + EF Core 10 + the PlaygroundModel assembly. Future scenarios are
        // additive: registering a Mongo scenario in ScenarioRegistry would
        // automatically pull in MongoDB.Driver here.
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
        // Try the standard _framework/ path first. If Blazor's BaseAddress
        // doesn't point at the playground subdirectory (e.g., the web component
        // is hosted on a VitePress page), fall back to the playground/ prefix.
        var url = $"_framework/{assemblyName}.dll";
        try
        {
            var bytes = await _http.GetByteArrayAsync(url);
            return MetadataReference.CreateFromImage(bytes, filePath: assemblyName + ".dll");
        }
        catch (HttpRequestException)
        {
            // The runtime sometimes splits an assembly into multiple package
            // ones — if a logical name doesn't resolve to a file in /_framework,
            // skip it. Roslyn will surface a "missing reference" diagnostic
            // later if the snippet actually needs the type.
            return null;
        }
    }
}
