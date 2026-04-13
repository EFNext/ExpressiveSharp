using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace ExpressiveSharp.Docs.Playground.Core.Services;

public interface IPlaygroundReferences
{
    ImmutableArray<MetadataReference> References { get; }
    bool IsLoaded { get; }
}
