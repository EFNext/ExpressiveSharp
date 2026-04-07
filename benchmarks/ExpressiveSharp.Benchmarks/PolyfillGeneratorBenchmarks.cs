using System.Text;
using BenchmarkDotNet.Attributes;
using ExpressiveSharp.Generator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace ExpressiveSharp.Benchmarks;

[MemoryDiagnoser]
public class PolyfillGeneratorBenchmarks
{
    [Params(1, 100)]
    public int CallSiteCount { get; set; }

    private Compilation _compilation = null!;
    private GeneratorDriver _warmedDriver = null!;
    private Compilation _modifiedCompilation = null!;

    [GlobalSetup]
    public void Setup()
    {
        var sources = BuildPolyfillSources(CallSiteCount);
        _compilation = CreatePolyfillCompilation(sources);

        _warmedDriver = CSharpGeneratorDriver
            .Create(new PolyfillInterceptorGenerator())
            .RunGeneratorsAndUpdateCompilation(_compilation, out _, out _);

        var firstTree = _compilation.SyntaxTrees.First();
        _modifiedCompilation = _compilation.ReplaceSyntaxTree(
            firstTree,
            firstTree.WithChangedText(SourceText.From(firstTree.GetText() + "\n// bench-edit")));
    }

    [Benchmark(Baseline = true)]
    public GeneratorDriver RunGenerator()
        => CSharpGeneratorDriver
            .Create(new PolyfillInterceptorGenerator())
            .RunGeneratorsAndUpdateCompilation(_compilation, out _, out _);

    [Benchmark]
    public GeneratorDriver RunGenerator_Incremental()
        => _warmedDriver
            .RunGeneratorsAndUpdateCompilation(_modifiedCompilation, out _, out _);

    private static Compilation CreatePolyfillCompilation(IReadOnlyList<string> sources)
    {
        var references = Basic.Reference.Assemblies.Net100.References.All.ToList();
        references.Add(MetadataReference.CreateFromFile(typeof(ExpressiveAttribute).Assembly.Location));

        return CSharpCompilation.Create(
            "PolyfillBenchmarkInput",
            sources.Select((s, idx) => CSharpSyntaxTree.ParseText(s, path: $"Polyfill{idx}.cs")),
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private static IReadOnlyList<string> BuildPolyfillSources(int callSiteCount)
    {
        var sources = new List<string>();

        // Entity class
        sources.Add(@"
using System;
using System.Linq;
using ExpressiveSharp;

namespace PolyfillBenchmarkInput;

public class BenchEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
");

        // Call sites — each in its own method to generate distinct interceptors
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Linq;");
        sb.AppendLine("using ExpressiveSharp;");
        sb.AppendLine();
        sb.AppendLine("namespace PolyfillBenchmarkInput;");
        sb.AppendLine();
        sb.AppendLine("public static class Queries");
        sb.AppendLine("{");

        for (var i = 0; i < callSiteCount; i++)
        {
            sb.AppendLine($"    public static IQueryable<int> Query{i}(IExpressiveQueryable<BenchEntity> q)");
            sb.AppendLine($"        => q.Select(x => x.Id + {i});");
        }

        sb.AppendLine("}");
        sources.Add(sb.ToString());

        return sources;
    }
}
