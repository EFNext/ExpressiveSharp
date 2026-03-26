using BenchmarkDotNet.Attributes;
using ExpressiveSharp.Benchmarks.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.Benchmarks;

[MemoryDiagnoser]
public class EFCoreQueryOverheadBenchmarks
{
    private TestDbContext _baselineCtx = null!;
    private TestDbContext _expressiveCtx = null!;

    [GlobalSetup]
    public void Setup()
    {
        _baselineCtx = new TestDbContext(useExpressives: false);
        _expressiveCtx = new TestDbContext(useExpressives: true);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _baselineCtx.Dispose();
        _expressiveCtx.Dispose();
    }

    [Benchmark(Baseline = true)]
    public string Baseline()
        => _baselineCtx.Entities.Select(x => x.Id).ToQueryString();

    [Benchmark]
    public string WithExpressives_Property()
        => _expressiveCtx.Entities.Select(x => x.IdPlus1).ToQueryString();

    [Benchmark]
    public string WithExpressives_Method()
        => _expressiveCtx.Entities.Select(x => x.IdPlus1Method()).ToQueryString();

    [Benchmark]
    public string WithExpressives_NullConditional()
        => _expressiveCtx.Entities.Select(x => x.EmailLength).ToQueryString();

    [Benchmark]
    public string ColdStart_WithExpressives()
    {
        using var ctx = new TestDbContext(useExpressives: true);
        return ctx.Entities.Select(x => x.IdPlus1).ToQueryString();
    }

    [Benchmark]
    public string ColdStart_Baseline()
    {
        using var ctx = new TestDbContext(useExpressives: false);
        return ctx.Entities.Select(x => x.Id).ToQueryString();
    }
}
