using System.Collections.Immutable;
using System.Reflection;
using Basic.Reference.Assemblies;
using ExpressiveSharp.Docs.Playground.Core.Services;
using ExpressiveSharp.Docs.Playground.Core.Services.Scenarios;
using Microsoft.CodeAnalysis;

namespace ExpressiveSharp.Docs.Prerenderer;

/// <summary>
/// Loads reference assemblies from disk (the build output of the referenced
/// projects) instead of fetching via HTTP like the WASM <c>PlaygroundReferences</c>.
/// </summary>
internal sealed class LocalPlaygroundReferences : IPlaygroundReferences
{
    private ImmutableArray<MetadataReference> _references;

    public ImmutableArray<MetadataReference> References => _references;
    public bool IsLoaded => !_references.IsDefault;

    public void Load()
    {
        if (IsLoaded) return;

        var builder = ImmutableArray.CreateBuilder<MetadataReference>();
        builder.AddRange(Net100.References.All);

        var seenPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var scenario in ScenarioRegistry.All)
        {
            foreach (var assembly in scenario.ReferenceAssemblies)
            {
                if (string.IsNullOrEmpty(assembly.Location)) continue;
                if (!seenPaths.Add(assembly.Location)) continue;
                builder.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
        }

        _references = builder.ToImmutable();
    }
}
