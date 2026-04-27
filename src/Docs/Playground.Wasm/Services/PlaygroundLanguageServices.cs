// Long-lived AdhocWorkspace backing Monaco's completion + hover providers; the
// incremental semantic model gives sub-ms per-keystroke completion. Separate
// from SnippetCompiler, which builds fresh CSharpCompilations per run.
// One Document per <expressive-playground> instance, routed by Monaco model
// URI. The DefaultPersistentStorageConfiguration cctor PNSE on WASM is bypassed
// by the WorkspaceShim project — see its header.
// Architecture inspired by DotNetLab/src/Compiler/LanguageServices.cs (MIT).

using ExpressiveSharp.Docs.Playground.Core.Services;
using ExpressiveSharp.Docs.Playground.Core.Services.Scenarios;
using ExpressiveSharp.Docs.Playground.Wasm.WorkspaceShim;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.QuickInfo;
using Microsoft.CodeAnalysis.Text;

namespace ExpressiveSharp.Docs.Playground.Wasm.Services;

internal sealed class PlaygroundLanguageServices : IDisposable
{
    // Distinct from SnippetCompiler.SnippetFilePath so the run path's
    // [InterceptsLocation] interceptors stay decoupled from workspace docs.
    private const string DocumentPathPrefix = "/snippet/__Snippet_";

    private readonly AdhocWorkspace _workspace;
    private readonly ProjectId _projectId;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly Dictionary<string, DocumentId> _modelToDocument = new(StringComparer.Ordinal);
    private readonly Dictionary<DocumentId, SnippetWrap> _wrapByDoc = new();

    public PlaygroundLanguageServices(PlaygroundReferences references)
    {
        if (!references.IsLoaded)
            throw new InvalidOperationException(
                "PlaygroundReferences must be loaded before PlaygroundLanguageServices is constructed.");

        // Append WorkspaceShim AFTER DefaultAssemblies. Its NoOpPersistentStorageConfiguration
        // exports with ServiceLayer.Test, gaining MEF priority over Roslyn's
        // DefaultPersistentStorageConfiguration so the latter's cctor never runs
        // (it calls Process.GetCurrentProcess() — PNSE on WASM).
        var hostServices = MefHostServices.Create(
            MefHostServices.DefaultAssemblies
                .Append(typeof(NoOpPersistentStorageConfiguration).Assembly));

        _workspace = new AdhocWorkspace(hostServices);

        var projectInfo = ProjectInfo.Create(
            id: ProjectId.CreateNewId(),
            version: VersionStamp.Create(),
            name: "PlaygroundProject",
            assemblyName: "PlaygroundProject",
            language: LanguageNames.CSharp,
            metadataReferences: references.References,
            // WASM is single-threaded; parallel Roslyn compile threads deadlock or throw.
            compilationOptions: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithNullableContextOptions(NullableContextOptions.Enable)
                .WithConcurrentBuild(false),
            parseOptions: CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp13));

        _workspace.AddProject(projectInfo);
        _projectId = projectInfo.Id;
    }

    // Forces MEF composition during page load instead of lazily on the first
    // keystroke. Resolving CompletionService triggers MEF + the cctor moment
    // WorkspaceShim guards against; subsequent real completions hit warm caches.
    public async Task PrewarmAsync()
    {
        await _lock.WaitAsync();
        try
        {
            const string warmupSource = "class __Warmup { void M() { var x = 1; } }";
            var docInfo = DocumentInfo.Create(
                id: DocumentId.CreateNewId(_projectId),
                name: "__Warmup.cs",
                filePath: "/snippet/__Warmup.cs",
                loader: TextLoader.From(TextAndVersion.Create(SourceText.From(warmupSource), VersionStamp.Create())));

            var newSolution = _workspace.CurrentSolution.AddDocument(docInfo);
            if (!_workspace.TryApplyChanges(newSolution))
                return;

            var doc = _workspace.CurrentSolution.GetDocument(docInfo.Id);
            if (doc is not null)
            {
                var completionService = CompletionService.GetService(doc);
                if (completionService is not null)
                    _ = await completionService.GetCompletionsAsync(doc, caretPosition: warmupSource.Length - 3);
            }

            _workspace.TryApplyChanges(_workspace.CurrentSolution.RemoveDocument(docInfo.Id));
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task RegisterEditorAsync(string modelUri, string snippetText, string? setupText, IPlaygroundScenario scenario)
    {
        var wrap = SnippetWrap.Build(scenario.WrapperTemplate, snippetText, setupText);
        var docInfo = DocumentInfo.Create(
            id: DocumentId.CreateNewId(_projectId),
            name: "__Snippet.cs",
            filePath: $"{DocumentPathPrefix}{Guid.NewGuid():N}.cs",
            loader: TextLoader.From(TextAndVersion.Create(SourceText.From(wrap.Source), VersionStamp.Create())));

        await _lock.WaitAsync();
        try
        {
            // Drop any prior document under the same URI (instance dispose + remount).
            if (_modelToDocument.TryGetValue(modelUri, out var existingId))
            {
                _workspace.TryApplyChanges(_workspace.CurrentSolution.RemoveDocument(existingId));
                _wrapByDoc.Remove(existingId);
            }

            if (!_workspace.TryApplyChanges(_workspace.CurrentSolution.AddDocument(docInfo)))
                return;

            _modelToDocument[modelUri] = docInfo.Id;
            _wrapByDoc[docInfo.Id] = wrap;
        }
        finally
        {
            _lock.Release();
        }
    }

    // Called per-keystroke; no debounce — Roslyn diffs the syntax tree (sub-ms).
    public async Task UpdateEditorAsync(string modelUri, string snippetText, string? setupText, IPlaygroundScenario scenario)
    {
        var wrap = SnippetWrap.Build(scenario.WrapperTemplate, snippetText, setupText);

        await _lock.WaitAsync();
        try
        {
            if (!_modelToDocument.TryGetValue(modelUri, out var docId))
                return;

            _workspace.TryApplyChanges(
                _workspace.CurrentSolution.WithDocumentText(docId, SourceText.From(wrap.Source)));
            _wrapByDoc[docId] = wrap;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task UnregisterEditorAsync(string modelUri)
    {
        await _lock.WaitAsync();
        try
        {
            if (!_modelToDocument.TryGetValue(modelUri, out var docId))
                return;

            _workspace.TryApplyChanges(_workspace.CurrentSolution.RemoveDocument(docId));
            _modelToDocument.Remove(modelUri);
            _wrapByDoc.Remove(docId);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<MonacoCompletionList?> GetCompletionsAsync(string modelUri, MonacoPosition position)
    {
        await _lock.WaitAsync();
        try
        {
            if (!_modelToDocument.TryGetValue(modelUri, out var docId)) return null;
            if (!_wrapByDoc.TryGetValue(docId, out var wrap)) return null;

            var doc = _workspace.CurrentSolution.GetDocument(docId);
            if (doc is null) return null;

            var completionService = CompletionService.GetService(doc);
            if (completionService is null) return null;

            var text = await doc.GetTextAsync().ConfigureAwait(false);
            var caretOffset = MonacoPositionToCaretOffset(position, text, wrap);
            if (caretOffset < 0) return null;

            var roslynList = await completionService.GetCompletionsAsync(doc, caretOffset).ConfigureAwait(false);
            if (roslynList is null || roslynList.ItemsList.Count == 0) return null;

            return RoslynMonacoConverters.ToMonacoCompletionList(roslynList, text, wrap);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<MonacoHover?> GetHoverAsync(string modelUri, MonacoPosition position)
    {
        await _lock.WaitAsync();
        try
        {
            if (!_modelToDocument.TryGetValue(modelUri, out var docId)) return null;
            if (!_wrapByDoc.TryGetValue(docId, out var wrap)) return null;

            var doc = _workspace.CurrentSolution.GetDocument(docId);
            if (doc is null) return null;

            var quickInfoService = QuickInfoService.GetService(doc);
            if (quickInfoService is null) return null;

            var text = await doc.GetTextAsync().ConfigureAwait(false);
            var caretOffset = MonacoPositionToCaretOffset(position, text, wrap);
            if (caretOffset < 0) return null;

            var quickInfo = await quickInfoService.GetQuickInfoAsync(doc, caretOffset).ConfigureAwait(false);
            if (quickInfo is null) return null;

            return RoslynMonacoConverters.ToMonacoHover(quickInfo, text, wrap);
        }
        finally
        {
            _lock.Release();
        }
    }

    // Returns -1 if the position falls outside the snippet region.
    // Monaco is 1-based, LinePosition is 0-based.
    private static int MonacoPositionToCaretOffset(MonacoPosition position, SourceText text, SnippetWrap wrap)
    {
        var snippetRelative = new LinePosition(
            line: Math.Max(0, position.LineNumber - 1),
            character: Math.Max(0, position.Column - 1));
        var wrapped = wrap.ToWrapped(snippetRelative);

        if (wrapped.Line < 0 || wrapped.Line >= text.Lines.Count)
            return -1;
        var lineSpan = text.Lines[wrapped.Line];
        if (wrapped.Character > lineSpan.End - lineSpan.Start)
            return -1;
        return text.Lines.GetPosition(wrapped);
    }

    public void Dispose()
    {
        _workspace.Dispose();
        _lock.Dispose();
    }
}
