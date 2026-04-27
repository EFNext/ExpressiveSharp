// Adapted from DotNetLab/src/RoslynWorkspaceAccess/RoslynWorkspaceAccessors.cs (MIT).
//
// Roslyn's DefaultPersistentStorageConfiguration..cctor() calls
// Process.GetCurrentProcess(), which PNSEs on Blazor WebAssembly; MEF
// discovers it on the first CompletionService.GetService() inside an
// AdhocWorkspace. Exporting our own IPersistentStorageConfiguration with
// ServiceLayer.Test gives it MEF priority over the default so the broken
// type's cctor never runs.
//
// The csproj impersonates AssemblyName "Microsoft.CodeAnalysis.Workspaces.UnitTests"
// so Roslyn's [InternalsVisibleTo] lets us reference the internal types
// IPersistentStorageConfiguration, MefConstruction, ExportWorkspaceServiceAttribute,
// and ServiceLayer.

using System.Composition;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Storage;

namespace ExpressiveSharp.Docs.Playground.Wasm.WorkspaceShim;

[ExportWorkspaceService(typeof(IPersistentStorageConfiguration), ServiceLayer.Test), Shared]
[method: ImportingConstructor]
[method: Obsolete(MefConstruction.ImportingConstructorMessage, error: true)]
public sealed class NoOpPersistentStorageConfiguration() : IPersistentStorageConfiguration
{
    public bool ThrowOnFailure => false;

    string? IPersistentStorageConfiguration.TryGetStorageLocation(SolutionKey solutionKey) => null;
}
