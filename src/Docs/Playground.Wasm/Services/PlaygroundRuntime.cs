// Singleton DI service shared by every PlaygroundHost on the page; owns the
// reference set, SnippetCompiler, IntelliSense workspace, lazy-loaded provider
// assemblies, and per-page compile cache. Dispatch goes through
// IPlaygroundScenario / IScenarioInstance.

using ExpressiveSharp.Docs.Playground.Core.Services;
using ExpressiveSharp.Docs.Playground.Core.Services.Scenarios;
using Microsoft.AspNetCore.Components.WebAssembly.Services;

namespace ExpressiveSharp.Docs.Playground.Wasm.Services;

internal sealed class PlaygroundRuntime : IAsyncDisposable
{
    // Sentinel target id for the universal "show generator output" target.
    public const string GeneratorTargetId = "generator";

    private const int CompileCacheMaxSize = 32;

    private readonly PlaygroundReferences _references;
    private readonly LazyAssemblyLoader _lazyLoader;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private readonly SemaphoreSlim _lazyLoadLock = new(1, 1);
    private readonly Dictionary<string, IScenarioInstance> _scenarioInstances = new(StringComparer.Ordinal);
    private readonly Dictionary<string, CachedCompile> _compileCache = new(StringComparer.Ordinal);
    private readonly Queue<string> _compileCacheOrder = new();
    private readonly HashSet<string> _loadedLazyAssemblies = new(StringComparer.Ordinal);
    private SnippetCompiler? _compiler;
    private PlaygroundLanguageServices? _languageServices;
    private bool _initialized;
    private string? _sharedTargetId;

    public PlaygroundRuntime(PlaygroundReferences references, LazyAssemblyLoader lazyLoader)
    {
        _references = references;
        _lazyLoader = lazyLoader;
    }

    public bool IsInitialized => _initialized;

    public PlaygroundLanguageServices LanguageServices =>
        _languageServices ?? throw new InvalidOperationException(
            "PlaygroundRuntime.LanguageServices accessed before InitializeAsync completed.");

    // Most recent target id chosen by any PlaygroundHost on the page. Dynamic
    // instances mounting after a broadcast read this to pick up the active
    // target instead of the scenario default.
    public string? SharedTargetId => _sharedTargetId;

    public void SetSharedTargetId(string targetId) => _sharedTargetId = targetId;

    public async Task InitializeAsync()
    {
        if (_initialized) return;
        await _initLock.WaitAsync();
        try
        {
            if (_initialized) return;

            await _references.LoadAsync();
            _compiler = new SnippetCompiler(_references);
            _languageServices = new PlaygroundLanguageServices(_references);
            await _languageServices.PrewarmAsync();
            _initialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    // Memoized by (scenarioId, setup, snippet); the cache stores the snippet's
    // Run method as a callable Func so multi-provider scenarios can invoke it
    // against a different argument per render target without recompiling.
    public async Task<RenderResult> RunAsync(
        string snippet,
        string? setup,
        string targetId,
        IPlaygroundScenario scenario)
    {
        if (!_initialized || _compiler is null)
            throw new InvalidOperationException("PlaygroundRuntime is not initialized.");

        // Must run before the cache check + Task.Run: LazyAssemblyLoader yields
        // to the JS event loop, and the assembly must be resolved before any IL
        // referencing it gets JIT-compiled.
        var renderTarget = scenario.RenderTargets.FirstOrDefault(t => t.Id == targetId);
        if (renderTarget?.LazyLoadAssemblies is { Count: > 0 } lazyAssemblies)
        {
            try
            {
                await EnsureLazyAssembliesLoadedAsync(lazyAssemblies);
            }
            catch (Exception ex)
            {
                return RenderResult.Exception(Unwrap(ex));
            }
        }

        var cacheKey = MakeCompileCacheKey(scenario.Id, setup, snippet);
        if (_compileCache.TryGetValue(cacheKey, out var cached))
            return await Task.Run(() => RenderFromCache(cached, targetId, scenario));

        return await Task.Run(() =>
        {
            try
            {
                var compileResult = _compiler.Compile(snippet, setup, scenario);
                var partitioned = PartitionDiagnostics(compileResult);

                if (!compileResult.Success)
                {
                    var failureCache = new CachedCompile(
                        Success: false,
                        GeneratedSources: compileResult.GeneratedSources,
                        SnippetMarkers: partitioned.snippet,
                        SetupErrorMessages: partitioned.setup,
                        FailureDiagnostics: compileResult.Diagnostics,
                        Invoke: null);
                    StoreInCache(cacheKey, failureCache);
                    return RenderFromCache(failureCache, targetId, scenario);
                }

                if (compileResult.Assembly is null)
                    throw new InvalidOperationException("Compile reported success but produced no assembly.");

                var snippetType = compileResult.Assembly.GetType(SnippetCompiler.SnippetTypeFullName)
                    ?? throw new InvalidOperationException(
                        $"Snippet type '{SnippetCompiler.SnippetTypeFullName}' not found.");
                var runMethod = snippetType.GetMethod("Run", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                    ?? throw new InvalidOperationException("Snippet.Run method not found.");

                Func<object, IQueryable> invoke = arg =>
                    (IQueryable?)runMethod.Invoke(null, new[] { arg })
                        ?? throw new InvalidOperationException("Snippet.Run returned null.");

                var successCache = new CachedCompile(
                    Success: true,
                    GeneratedSources: compileResult.GeneratedSources,
                    SnippetMarkers: partitioned.snippet,
                    SetupErrorMessages: partitioned.setup,
                    FailureDiagnostics: null,
                    Invoke: invoke);
                StoreInCache(cacheKey, successCache);
                return RenderFromCache(successCache, targetId, scenario);
            }
            catch (Exception ex)
            {
                return RenderResult.Exception(Unwrap(ex));
            }
        });
    }

    // Null bytes prevent ambiguous concatenation collisions. targetId is
    // intentionally NOT in the key — one compile serves any target.
    private static string MakeCompileCacheKey(string scenarioId, string? setup, string snippet) =>
        scenarioId + "\0" + (setup ?? "") + "\0" + snippet;

    // The lock serializes concurrent loads from a dropdown broadcast hitting
    // multiple instances at once with the same target.
    private async Task EnsureLazyAssembliesLoadedAsync(IReadOnlyList<string> assemblyFileNames)
    {
        var needsLoad = false;
        foreach (var name in assemblyFileNames)
            if (!_loadedLazyAssemblies.Contains(name)) { needsLoad = true; break; }
        if (!needsLoad) return;

        await _lazyLoadLock.WaitAsync();
        try
        {
            var pending = new List<string>(assemblyFileNames.Count);
            foreach (var name in assemblyFileNames)
                if (!_loadedLazyAssemblies.Contains(name))
                    pending.Add(name);
            if (pending.Count == 0) return;

            await _lazyLoader.LoadAssembliesAsync(pending);
            foreach (var name in pending)
                _loadedLazyAssemblies.Add(name);
        }
        finally
        {
            _lazyLoadLock.Release();
        }
    }

    private void StoreInCache(string key, CachedCompile entry)
    {
        if (!_compileCache.ContainsKey(key))
        {
            if (_compileCache.Count >= CompileCacheMaxSize)
            {
                var oldest = _compileCacheOrder.Dequeue();
                _compileCache.Remove(oldest);
            }
            _compileCacheOrder.Enqueue(key);
        }
        _compileCache[key] = entry;
    }

    private RenderResult RenderFromCache(CachedCompile cached, string targetId, IPlaygroundScenario scenario)
    {
        try
        {
            if (!cached.Success)
            {
                return RenderResult.Failure(
                    cached.FailureDiagnostics ?? Array.Empty<SnippetDiagnostic>(),
                    cached.GeneratedSources,
                    cached.SnippetMarkers,
                    cached.SetupErrorMessages);
            }

            if (targetId == GeneratorTargetId)
            {
                return RenderResult.Ok(
                    FormatGeneratorOutput(cached.GeneratedSources),
                    cached.GeneratedSources,
                    cached.SnippetMarkers,
                    cached.SetupErrorMessages);
            }

            var renderTarget = scenario.RenderTargets.FirstOrDefault(t => t.Id == targetId)
                ?? throw new InvalidOperationException(
                    $"Scenario '{scenario.Id}' does not support render target '{targetId}'.");

            var instance = GetOrCreateInstance(scenario);
            var queryArgument = renderTarget.GetQueryArgument?.Invoke(instance) ?? instance.QueryArgument;
            var queryable = cached.Invoke!(queryArgument);

            var output = renderTarget.Render(queryable, instance);
            return RenderResult.Ok(
                output,
                cached.GeneratedSources,
                cached.SnippetMarkers,
                cached.SetupErrorMessages);
        }
        catch (Exception ex)
        {
            return RenderResult.Exception(Unwrap(ex));
        }
    }

    private sealed record CachedCompile(
        bool Success,
        IReadOnlyList<GeneratedSource> GeneratedSources,
        IReadOnlyList<SnippetMarker> SnippetMarkers,
        IReadOnlyList<string> SetupErrorMessages,
        IReadOnlyList<SnippetDiagnostic>? FailureDiagnostics,
        Func<object, IQueryable>? Invoke);

    private IScenarioInstance GetOrCreateInstance(IPlaygroundScenario scenario)
    {
        if (_scenarioInstances.TryGetValue(scenario.Id, out var existing))
            return existing;

        var fresh = scenario.CreateInstance();
        _scenarioInstances[scenario.Id] = fresh;
        return fresh;
    }

    // Snippet markers go to Monaco squiggles (translated to user coordinates);
    // setup messages go to PlaygroundHost's error block since Monaco doesn't
    // see setup. Anything anchored elsewhere in the wrapped source is dropped.
    private static (List<SnippetMarker> snippet, List<string> setup) PartitionDiagnostics(CompileResult result)
    {
        var snippetMarkers = new List<SnippetMarker>();
        var setupMessages = new List<string>();

        foreach (var diag in result.Diagnostics)
        {
            if (diag.Span is not { } span)
                continue;

            if (result.Wrap.IsInSnippet(span.Start))
            {
                var startRel = result.Wrap.ToSnippetRelative(span.Start);
                var endRel = result.Wrap.ToSnippetRelative(span.End);
                snippetMarkers.Add(new SnippetMarker(
                    Severity: diag.Severity,
                    Code: diag.Id,
                    Message: diag.Message,
                    StartLine: startRel.Line + 1,
                    StartColumn: startRel.Character + 1,
                    EndLine: endRel.Line + 1,
                    // At least one column wide so zero-width diagnostics still get a squiggle.
                    EndColumn: Math.Max(endRel.Character + 1, startRel.Character + 2)));
            }
            else if (result.Wrap.IsInSetup(span.Start))
            {
                setupMessages.Add(diag.ToString());
            }
        }

        return (snippetMarkers, setupMessages);
    }

    // Unwraps TargetInvocationException / TypeInitializationException to surface
    // the actual root cause from reflection-invoked methods and cctor failures.
    private static Exception Unwrap(Exception ex)
    {
        while (true)
        {
            if (ex is System.Reflection.TargetInvocationException tie && tie.InnerException is not null)
            {
                ex = tie.InnerException;
                continue;
            }
            if (ex is TypeInitializationException tinit && tinit.InnerException is not null)
            {
                ex = tinit.InnerException;
                continue;
            }
            return ex;
        }
    }

    private static string FormatGeneratorOutput(IReadOnlyList<GeneratedSource> sources)
    {
        if (sources.Count == 0)
            return "// (no generator output for this snippet)";

        var sb = new System.Text.StringBuilder();
        for (var i = 0; i < sources.Count; i++)
        {
            if (i > 0) sb.AppendLine().AppendLine();
            sb.Append("// === ").Append(sources[i].HintName).AppendLine(" ===");
            sb.Append(sources[i].Source);
        }
        return sb.ToString();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var instance in _scenarioInstances.Values)
        {
            try { await instance.DisposeAsync(); }
            catch { /* swallow — page unload */ }
        }
        _scenarioInstances.Clear();
        _languageServices?.Dispose();
        _initLock.Dispose();
        _lazyLoadLock.Dispose();
    }
}

// Diagnostic in the user's snippet region with positions translated to
// snippet-relative 1-based coordinates for Monaco's setModelMarkers.
internal sealed record SnippetMarker(
    Microsoft.CodeAnalysis.DiagnosticSeverity Severity,
    string Code,
    string Message,
    int StartLine,
    int StartColumn,
    int EndLine,
    int EndColumn);

internal sealed record RenderResult(
    bool Success,
    string? Output,
    string? ErrorMessage,
    IReadOnlyList<SnippetDiagnostic> Diagnostics,
    IReadOnlyList<GeneratedSource> GeneratedSources,
    IReadOnlyList<SnippetMarker> SnippetMarkers,
    IReadOnlyList<string> SetupErrorMessages)
{
    public static RenderResult Ok(
        string output,
        IReadOnlyList<GeneratedSource> generatedSources,
        IReadOnlyList<SnippetMarker> snippetMarkers,
        IReadOnlyList<string> setupErrorMessages)
        => new(true, output, null, Array.Empty<SnippetDiagnostic>(), generatedSources, snippetMarkers, setupErrorMessages);

    public static RenderResult Failure(
        IReadOnlyList<SnippetDiagnostic> diagnostics,
        IReadOnlyList<GeneratedSource> generatedSources,
        IReadOnlyList<SnippetMarker> snippetMarkers,
        IReadOnlyList<string> setupErrorMessages)
        => new(false, null, "Compilation failed.", diagnostics, generatedSources, snippetMarkers, setupErrorMessages);

    public static RenderResult Exception(Exception ex)
        => new(false, null, ex.GetType().Name + ": " + ex.Message,
            Array.Empty<SnippetDiagnostic>(), Array.Empty<GeneratedSource>(),
            Array.Empty<SnippetMarker>(), Array.Empty<string>());
}
