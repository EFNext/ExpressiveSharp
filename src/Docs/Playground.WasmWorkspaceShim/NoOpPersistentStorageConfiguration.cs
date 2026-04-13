// Adapted from DotNetLab/src/RoslynWorkspaceAccess/RoslynWorkspaceAccessors.cs (MIT).
// https://github.com/jjonescz/DotNetLab
//
// Why this exists: Roslyn's DefaultPersistentStorageConfiguration..cctor() calls
// Process.GetCurrentProcess(), which throws PlatformNotSupportedException on
// Blazor WebAssembly. MEF discovers it on the first CompletionService.GetService()
// call inside an AdhocWorkspace. By exporting our own IPersistentStorageConfiguration
// with ServiceLayer.Test, MEF prefers ours over the default and never instantiates
// the broken type — its cctor never runs, the PNSE is never thrown, and completions
// work in WASM.
//
// This file lives in an assembly whose AssemblyName is impersonated to
// "Microsoft.CodeAnalysis.Workspaces.UnitTests" (see csproj) so Roslyn's
// [InternalsVisibleTo] attribute lets us reference IPersistentStorageConfiguration,
// MefConstruction, ExportWorkspaceServiceAttribute, and ServiceLayer — all of
// which are `internal` in Microsoft.CodeAnalysis.Workspaces.dll.

using System.Composition;
using Microsoft.CodeAnalysis.Host;       // IPersistentStorageConfiguration
using Microsoft.CodeAnalysis.Host.Mef;   // ExportWorkspaceService, ServiceLayer, MefConstruction
using Microsoft.CodeAnalysis.Storage;    // SolutionKey

namespace ExpressiveSharp.Docs.Playground.Wasm.WorkspaceShim;

[ExportWorkspaceService(typeof(IPersistentStorageConfiguration), ServiceLayer.Test), Shared]
[method: ImportingConstructor]
[method: Obsolete(MefConstruction.ImportingConstructorMessage, error: true)]
public sealed class NoOpPersistentStorageConfiguration() : IPersistentStorageConfiguration
{
    public bool ThrowOnFailure => false;

    string? IPersistentStorageConfiguration.TryGetStorageLocation(SolutionKey solutionKey) => null;
}
