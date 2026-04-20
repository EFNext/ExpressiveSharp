using System.Text;
using BenchmarkDotNet.Attributes;
using ExpressiveSharp.Generator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace ExpressiveSharp.Benchmarks;

/// <summary>
/// Baseline: all call sites in a single source file.
/// Varies <see cref="CallSiteCount"/> to show how cost scales with site count.
///
/// <b>NOTE – two benchmark variants:</b>
/// <list type="bullet">
///   <item><c>*</c> — uses <c>RunGenerators</c> (generator pipeline only, no compilation update).
///     The O(N) overhead here comes from the driver maintaining N cached output entries in its
///     internal state.</item>
///   <item><c>*_E2E</c> — uses <c>RunGeneratorsAndUpdateCompilation</c> (full pipeline including
///     the Roslyn compilation rebuild).
///     This variant measures end-to-end cost, including applying generated sources to the
///     compilation, so results reflect both generator work and compilation-update overhead.
///     Use this variant when comparing overall IDE-like responsiveness rather than isolated
///     generator-pipeline cost.</item>
/// </list>
/// </summary>
[MemoryDiagnoser]
public class PolyfillSingleFileBenchmarks
{
    [Params(1, 10, 100)]
    public int CallSiteCount { get; set; }

    private Compilation _compilation = null!;
    private GeneratorDriver _warmedDriver = null!;
    // Entity file (Polyfill0.cs) has NO call sites — editing it should trigger zero re-transforms.
    private Compilation _entityFileModified = null!;
    // Query file (Polyfill1.cs) contains the call sites — editing it forces all N re-transforms.
    private Compilation _queryFileModified = null!;

    [GlobalSetup]
    public void Setup()
    {
        var sources = BuildSources(CallSiteCount);
        _compilation = CreateCompilation(sources);

        _warmedDriver = CSharpGeneratorDriver
            .Create(new PolyfillInterceptorGenerator())
            .RunGenerators(_compilation);

        // index 0 = entity (no call sites)
        var entityTree = _compilation.SyntaxTrees.ElementAt(0);
        _entityFileModified = _compilation.ReplaceSyntaxTree(
            entityTree,
            entityTree.WithChangedText(SourceText.From(entityTree.GetText() + "\n// bench-edit")));

        // index 1 = queries (CallSiteCount call sites)
        var queryTree = _compilation.SyntaxTrees.ElementAt(1);
        _queryFileModified = _compilation.ReplaceSyntaxTree(
            queryTree,
            queryTree.WithChangedText(SourceText.From(queryTree.GetText() + "\n// bench-edit")));
    }

    // ── Generator-only (no compilation rebuild) ──────────────────────────────

    /// <summary>
    /// Fresh driver, runs all <see cref="CallSiteCount"/> per-node transforms from scratch.
    /// </summary>
    [Benchmark(Baseline = true)]
    public GeneratorDriver Cold()
        => CSharpGeneratorDriver
            .Create(new PolyfillInterceptorGenerator())
            .RunGenerators(_compilation);

    /// <summary>
    /// Incremental edit on a file with ZERO call sites.
    /// Zero transforms should re-run; cost above baseline = pipeline evaluation overhead only.
    /// </summary>
    [Benchmark]
    public GeneratorDriver Incremental_EditEntityFile()
        => _warmedDriver.RunGenerators(_entityFileModified);

    /// <summary>
    /// Incremental edit on the file containing all <see cref="CallSiteCount"/> call sites.
    /// All transforms must re-run; shows the per-site transform cost.
    /// Should be ≈ <c>Cold</c>.
    /// </summary>
    [Benchmark]
    public GeneratorDriver Incremental_EditQueryFile()
        => _warmedDriver.RunGenerators(_queryFileModified);

    // ── End-to-end (generator + Roslyn compilation rebuild) ──────────────────
    // Compilation-rebuild cost dominates at large N; see the generator-only variants above
    // for a clean measurement of caching savings.

    [Benchmark]
    public GeneratorDriver Cold_E2E()
        => CSharpGeneratorDriver
            .Create(new PolyfillInterceptorGenerator())
            .RunGeneratorsAndUpdateCompilation(_compilation, out _, out _);

    [Benchmark]
    public GeneratorDriver Incremental_EditEntityFile_E2E()
        => _warmedDriver.RunGeneratorsAndUpdateCompilation(_entityFileModified, out _, out _);

    [Benchmark]
    public GeneratorDriver Incremental_EditQueryFile_E2E()
        => _warmedDriver.RunGeneratorsAndUpdateCompilation(_queryFileModified, out _, out _);

    private static Compilation CreateCompilation(IReadOnlyList<string> sources)
    {
        var refs = Basic.Reference.Assemblies.Net100.References.All.ToList();
        refs.Add(MetadataReference.CreateFromFile(typeof(ExpressiveAttribute).Assembly.Location));
        return CSharpCompilation.Create(
            "PolyfillSingleFileBench",
            sources.Select((s, i) => CSharpSyntaxTree.ParseText(s, path: $"Polyfill{i}.cs")),
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private static IReadOnlyList<string> BuildSources(int callSiteCount)
    {
        var sources = new List<string>
        {
            // index 0: entity — NO call sites
            @"
using System;
using System.Linq;
using ExpressiveSharp;
namespace PolyfillBench;
public class BenchEntity { public int Id { get; set; } public string Name { get; set; } = """"; }
"
        };

        // index 1: queries — all callSiteCount call sites
        var sb = new StringBuilder();
        sb.AppendLine("using System.Linq; using ExpressiveSharp; namespace PolyfillBench;");
        sb.AppendLine("public static class Queries {");
        for (var i = 0; i < callSiteCount; i++)
            sb.AppendLine($"  public static IQueryable<int> Q{i}(IExpressiveQueryable<BenchEntity> q) => q.Select(x => x.Id + {i});");
        sb.AppendLine("}");
        sources.Add(sb.ToString());

        return sources;
    }
}

/// <summary>
/// Multi-file scenario: call sites spread across <see cref="FileCount"/> source files.
/// Each file contains a fixed <see cref="CallSitesPerFile"/> call sites.
/// Demonstrates the key incremental-caching benefit from the no-Collect() pipeline design:
/// each call site registers its output independently, so only changed files are re-processed.
///
/// Key comparisons (generator-only variants):
/// <list type="bullet">
///   <item><c>Cold</c>                        — all FileCount × CallSitesPerFile transforms from scratch. Should scale linearly.</item>
///   <item><c>Incremental_EditCallSiteFile</c> — only 1 file re-processed (CallSitesPerFile transforms). Cost ≈ constant regardless of FileCount.</item>
///   <item><c>Incremental_EditNoiseFile</c>    — zero transforms re-run; pipeline overhead only. Cost ≈ constant regardless of FileCount.</item>
/// </list>
/// Expected: <c>Cold</c> grows linearly; <c>Incremental_EditCallSiteFile</c> and
/// <c>Incremental_EditNoiseFile</c> stay near-flat as <see cref="FileCount"/> grows.
/// The difference between them = cost of <see cref="CallSitesPerFile"/> transforms.
///
/// <b>Why this works:</b> Instead of <c>snippets.Collect()</c> (which forces O(total_sites)
/// array re-assembly on every run), each snippet is registered directly via
/// <c>RegisterSourceOutput</c>. Roslyn's per-output cache means unchanged call sites'
/// output files are served from cache with zero work.
///
/// <b>E2E variants</b> include the Roslyn compilation-rebuild cost which also scales
/// with file count and can partially mask the generator savings.
/// </summary>
[MemoryDiagnoser]
public class PolyfillMultiFileBenchmarks
{
    [Params(1, 5, 10)]
    public int FileCount { get; set; }

    private const int CallSitesPerFile = 5;

    private Compilation _compilation = null!;
    private GeneratorDriver _warmedDriver = null!;
    private Compilation _callSiteFileModified = null!;
    private Compilation _noiseFileModified = null!;

    [GlobalSetup]
    public void Setup()
    {
        var sources = BuildSources(FileCount, CallSitesPerFile);
        _compilation = CreateCompilation(sources);

        _warmedDriver = CSharpGeneratorDriver
            .Create(new PolyfillInterceptorGenerator())
            .RunGenerators(_compilation);

        // Index FileCount = last query file (has CallSitesPerFile call sites)
        var callSiteTree = _compilation.SyntaxTrees.ElementAt(FileCount);
        _callSiteFileModified = _compilation.ReplaceSyntaxTree(
            callSiteTree,
            callSiteTree.WithChangedText(SourceText.From(callSiteTree.GetText() + "\n// bench-edit")));

        // Index FileCount + 1 = noise file (zero call sites)
        var noiseTree = _compilation.SyntaxTrees.ElementAt(FileCount + 1);
        _noiseFileModified = _compilation.ReplaceSyntaxTree(
            noiseTree,
            noiseTree.WithChangedText(SourceText.From(noiseTree.GetText() + "\n// bench-edit")));
    }

    // ── Generator-only (no compilation rebuild) ──────────────────────────────

    /// <summary>All FileCount × CallSitesPerFile transforms run from scratch.</summary>
    [Benchmark(Baseline = true)]
    public GeneratorDriver Cold()
        => CSharpGeneratorDriver
            .Create(new PolyfillInterceptorGenerator())
            .RunGenerators(_compilation);

    /// <summary>
    /// Edits a file that contains <see cref="CallSitesPerFile"/> call sites.
    /// That file's transforms re-run; all other files' transforms come from cache.
    /// Cost ≈ <see cref="CallSitesPerFile"/> transforms regardless of <see cref="FileCount"/>.
    /// </summary>
    [Benchmark]
    public GeneratorDriver Incremental_EditCallSiteFile()
        => _warmedDriver.RunGenerators(_callSiteFileModified);

    /// <summary>
    /// Edits a file with NO call sites.
    /// Zero transforms should re-run. Expected cost ≈ pipeline overhead only (near-flat vs FileCount).
    /// The gap vs <c>Incremental_EditCallSiteFile</c> isolates the per-site transform cost.
    /// </summary>
    [Benchmark]
    public GeneratorDriver Incremental_EditNoiseFile()
        => _warmedDriver.RunGenerators(_noiseFileModified);

    // ── End-to-end (generator + Roslyn compilation rebuild) ──────────────────

    [Benchmark]
    public GeneratorDriver Cold_E2E()
        => CSharpGeneratorDriver
            .Create(new PolyfillInterceptorGenerator())
            .RunGeneratorsAndUpdateCompilation(_compilation, out _, out _);

    [Benchmark]
    public GeneratorDriver Incremental_EditCallSiteFile_E2E()
        => _warmedDriver.RunGeneratorsAndUpdateCompilation(_callSiteFileModified, out _, out _);

    [Benchmark]
    public GeneratorDriver Incremental_EditNoiseFile_E2E()
        => _warmedDriver.RunGeneratorsAndUpdateCompilation(_noiseFileModified, out _, out _);

    private static Compilation CreateCompilation(IReadOnlyList<string> sources)
    {
        var refs = Basic.Reference.Assemblies.Net100.References.All.ToList();
        refs.Add(MetadataReference.CreateFromFile(typeof(ExpressiveAttribute).Assembly.Location));
        return CSharpCompilation.Create(
            "PolyfillMultiFileBench",
            sources.Select((s, i) => CSharpSyntaxTree.ParseText(s, path: $"Polyfill{i}.cs")),
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private static IReadOnlyList<string> BuildSources(int fileCount, int sitesPerFile)
    {
        // index 0: entity
        var sources = new List<string>
        {
            @"using System.Linq; using ExpressiveSharp; namespace PolyfillBench;
public class BenchEntity { public int Id { get; set; } public string Name { get; set; } = """"; }"
        };

        // indices 1..fileCount: query files
        var sb = new StringBuilder();
        for (var f = 0; f < fileCount; f++)
        {
            sb.Clear();
            sb.AppendLine("using System.Linq; using ExpressiveSharp; namespace PolyfillBench;");
            sb.AppendLine($"public static class Queries{f} {{");
            for (var i = 0; i < sitesPerFile; i++)
                sb.AppendLine($"  public static IQueryable<int> Q{f}_{i}(IExpressiveQueryable<BenchEntity> q) => q.Select(x => x.Id + {f * 100 + i});");
            sb.AppendLine("}");
            sources.Add(sb.ToString());
        }

        // index fileCount + 1: noise — no call sites at all
        sources.Add(@"namespace PolyfillBench;
/// <summary>No IExpressiveQueryable here — editing this file should cost zero generator work.</summary>
public static class BenchHelper { public static string Describe(int id) => $""#{id}""; }");

        return sources;
    }
}
