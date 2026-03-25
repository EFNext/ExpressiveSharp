using System.Reflection;
using BenchmarkDotNet.Attributes;
using ExpressiveSharp.Benchmarks.Helpers;
using ExpressiveSharp.Services;

namespace ExpressiveSharp.Benchmarks;

[MemoryDiagnoser]
public class ExpressionResolverBenchmarks
{
    private ExpressiveResolver _resolver = null!;
    private MemberInfo _propertyMember = null!;
    private MemberInfo _methodMember = null!;
    private MemberInfo _methodWithParamsMember = null!;
    private MemberInfo _constructorMember = null!;

    [GlobalSetup]
    public void Setup()
    {
        _resolver = new ExpressiveResolver();

        var type = typeof(TestEntity);
        _propertyMember = type.GetProperty(nameof(TestEntity.IdPlus1))!;
        _methodMember = type.GetMethod(nameof(TestEntity.IdPlus1Method))!;
        _methodWithParamsMember = type.GetMethod(nameof(TestEntity.IdPlusDelta))!;
        _constructorMember = type.GetConstructor(new[] { typeof(TestEntity) })!;

        // Warm up the caches so we measure steady-state
        _resolver.FindGeneratedExpression(_propertyMember);
        _resolver.FindGeneratedExpression(_methodMember);
        _resolver.FindGeneratedExpression(_methodWithParamsMember);
        _resolver.FindGeneratedExpression(_constructorMember);
    }

    [Benchmark(Baseline = true)]
    public object Resolve_Property()
        => _resolver.FindGeneratedExpression(_propertyMember);

    [Benchmark]
    public object Resolve_Method()
        => _resolver.FindGeneratedExpression(_methodMember);

    [Benchmark]
    public object Resolve_MethodWithParams()
        => _resolver.FindGeneratedExpression(_methodWithParamsMember);

    [Benchmark]
    public object Resolve_Constructor()
        => _resolver.FindGeneratedExpression(_constructorMember);

    [Benchmark]
    public object? ResolveViaReflection_Property()
        => ExpressiveResolver.FindGeneratedExpressionViaReflection(_propertyMember);

    [Benchmark]
    public object? ResolveViaReflection_Method()
        => ExpressiveResolver.FindGeneratedExpressionViaReflection(_methodMember);

    [Benchmark]
    public object? ResolveViaReflection_MethodWithParams()
        => ExpressiveResolver.FindGeneratedExpressionViaReflection(_methodWithParamsMember);

    [Benchmark]
    public object? ResolveViaReflection_Constructor()
        => ExpressiveResolver.FindGeneratedExpressionViaReflection(_constructorMember);
}
