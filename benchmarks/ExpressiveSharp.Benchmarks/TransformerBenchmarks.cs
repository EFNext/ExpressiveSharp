using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;
using ExpressiveSharp.Benchmarks.Helpers;
using ExpressiveSharp.Extensions;
using ExpressiveSharp.Services;
using ExpressiveSharp.Transformers;

namespace ExpressiveSharp.Benchmarks;

[MemoryDiagnoser]
public class TransformerBenchmarks
{
    private Expression _nullConditionalTree = null!;
    private Expression _blockExpressionTree = null!;
    private Expression _fullPipelineTree = null!;
    private RemoveNullConditionalPatterns _removeNullConditional = null!;
    private FlattenBlockExpressions _flattenBlocks = null!;
    private ConvertLoopsToLinq _convertLoops = null!;
    private FlattenTupleComparisons _flattenTuples = null!;

    [GlobalSetup]
    public void Setup()
    {
        _removeNullConditional = new RemoveNullConditionalPatterns();
        _flattenBlocks = new FlattenBlockExpressions();
        _convertLoops = new ConvertLoopsToLinq();
        _flattenTuples = new FlattenTupleComparisons();

        // Build expression trees by first replacing [Expressive] members,
        // then feeding the raw expanded tree (pre-transform) to each transformer.
        var replacer = new ExpressiveReplacer(new ExpressiveResolver());

        Expression<Func<TestEntity, int?>> nullCondExpr = e => e.EmailLength;
        _nullConditionalTree = replacer.Replace(nullCondExpr);

        replacer = new ExpressiveReplacer(new ExpressiveResolver());
        Expression<Func<TestEntity, string>> blockExpr = e => e.GetCategory();
        _blockExpressionTree = replacer.Replace(blockExpr);

        // Full pipeline test: expression with multiple [Expressive] usages
        Expression<Func<TestEntity, string>> fullExpr = e =>
            $"{e.FullName} ({e.GetCategory()}) #{e.IdPlus1Method()}";
        _fullPipelineTree = fullExpr;
    }

    [Benchmark]
    public Expression Transform_RemoveNullConditionalPatterns()
        => _removeNullConditional.Transform(_nullConditionalTree);

    [Benchmark]
    public Expression Transform_FlattenBlockExpressions()
        => _flattenBlocks.Transform(_blockExpressionTree);

    [Benchmark]
    public Expression Transform_ConvertLoopsToLinq()
        => _convertLoops.Transform(_blockExpressionTree);

    [Benchmark]
    public Expression Transform_FlattenTupleComparisons()
        => _flattenTuples.Transform(_nullConditionalTree);

    [Benchmark(Baseline = true)]
    public Expression ExpandExpressives_FullPipeline()
        => _fullPipelineTree.ExpandExpressives();
}
