using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;
using ExpressiveSharp.Benchmarks.Helpers;
using ExpressiveSharp.Services;

namespace ExpressiveSharp.Benchmarks;

[MemoryDiagnoser]
public class ExpressionReplacerBenchmarks
{
    private Expression _propertyExpression = null!;
    private Expression _methodExpression = null!;
    private Expression _nullConditionalExpression = null!;
    private Expression _blockBodyExpression = null!;
    private Expression _deepChainExpression = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Build expression trees that reference [Expressive] members on TestEntity.
        // When Replace is called, the replacer will resolve and inline them.
        Expression<Func<TestEntity, int>> propExpr = e => e.IdPlus1;
        _propertyExpression = propExpr;

        Expression<Func<TestEntity, int>> methodExpr = e => e.IdPlus1Method();
        _methodExpression = methodExpr;

        Expression<Func<TestEntity, int?>> nullCondExpr = e => e.EmailLength;
        _nullConditionalExpression = nullCondExpr;

        Expression<Func<TestEntity, string>> blockExpr = e => e.GetCategory();
        _blockBodyExpression = blockExpr;

        // Deep chain: multiple [Expressive] member accesses in one tree
        Expression<Func<TestEntity, string>> deepExpr = e =>
            $"{e.FullName} ({e.GetCategory()}) #{e.IdPlus1Method()}";
        _deepChainExpression = deepExpr;
    }

    [Benchmark(Baseline = true)]
    public Expression Replace_Property()
        => new ExpressiveReplacer(new ExpressiveResolver()).Replace(_propertyExpression);

    [Benchmark]
    public Expression Replace_Method()
        => new ExpressiveReplacer(new ExpressiveResolver()).Replace(_methodExpression);

    [Benchmark]
    public Expression Replace_NullConditional()
        => new ExpressiveReplacer(new ExpressiveResolver()).Replace(_nullConditionalExpression);

    [Benchmark]
    public Expression Replace_BlockBody()
        => new ExpressiveReplacer(new ExpressiveResolver()).Replace(_blockBodyExpression);

    [Benchmark]
    public Expression Replace_DeepChain()
        => new ExpressiveReplacer(new ExpressiveResolver()).Replace(_deepChainExpression);
}
