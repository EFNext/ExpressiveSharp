using BenchmarkDotNet.Attributes;
using ExpressiveSharp.Benchmarks.Helpers;
using ExpressiveSharp.Generator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace ExpressiveSharp.Benchmarks;

[MemoryDiagnoser]
public class GeneratorBenchmarks
{
    [Params(1, 100, 1000)]
    public int ExpressiveCount { get; set; }

    private Compilation _compilation = null!;
    private GeneratorDriver _warmedDriver = null!;
    private Compilation _noiseModifiedCompilation = null!;
    private Compilation _expressiveModifiedCompilation = null!;
    private int _firstNoiseTreeIndex;

    [GlobalSetup]
    public void Setup()
    {
        var expressiveSources = BenchmarkCompilationHelper.BuildExpressiveSources(ExpressiveCount);
        _firstNoiseTreeIndex = expressiveSources.Count;

        _compilation = BenchmarkCompilationHelper.CreateCompilation(expressiveSources);

        _warmedDriver = CSharpGeneratorDriver
            .Create(new ExpressiveGenerator())
            .RunGeneratorsAndUpdateCompilation(_compilation, out _, out _);

        // Noise-modified: append comment to first noise tree
        var noiseTree = _compilation.SyntaxTrees.ElementAt(_firstNoiseTreeIndex);
        _noiseModifiedCompilation = _compilation.ReplaceSyntaxTree(
            noiseTree,
            noiseTree.WithChangedText(SourceText.From(noiseTree.GetText() + "\n// bench-edit")));

        // Expressive-modified: append comment to first expressive tree
        var expressiveTree = _compilation.SyntaxTrees.First();
        _expressiveModifiedCompilation = _compilation.ReplaceSyntaxTree(
            expressiveTree,
            expressiveTree.WithChangedText(SourceText.From(expressiveTree.GetText() + "\n// bench-edit")));
    }

    // Cold benchmarks — brand-new driver per iteration

    [Benchmark(Baseline = true)]
    public GeneratorDriver RunGenerator()
        => CSharpGeneratorDriver
            .Create(new ExpressiveGenerator())
            .RunGeneratorsAndUpdateCompilation(_compilation, out _, out _);

    [Benchmark]
    public GeneratorDriver RunGenerator_NoiseChange()
        => CSharpGeneratorDriver
            .Create(new ExpressiveGenerator())
            .RunGeneratorsAndUpdateCompilation(_noiseModifiedCompilation, out _, out _);

    [Benchmark]
    public GeneratorDriver RunGenerator_ExpressiveChange()
        => CSharpGeneratorDriver
            .Create(new ExpressiveGenerator())
            .RunGeneratorsAndUpdateCompilation(_expressiveModifiedCompilation, out _, out _);

    // Incremental benchmarks — pre-warmed driver processes a single edit

    [Benchmark]
    public GeneratorDriver RunGenerator_Incremental_NoiseChange()
        => _warmedDriver
            .RunGeneratorsAndUpdateCompilation(_noiseModifiedCompilation, out _, out _);

    [Benchmark]
    public GeneratorDriver RunGenerator_Incremental_ExpressiveChange()
        => _warmedDriver
            .RunGeneratorsAndUpdateCompilation(_expressiveModifiedCompilation, out _, out _);
}
