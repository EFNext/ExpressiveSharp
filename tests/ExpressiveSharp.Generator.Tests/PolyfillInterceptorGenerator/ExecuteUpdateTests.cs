using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

[TestClass]
public class ExecuteUpdateTests : GeneratorTestBase
{
    /// <summary>
    /// Shared mock types that simulate EF Core's SetPropertyCalls and RelationalQueryableExtensions.
    /// Used by all tests since the generator tests run against a minimal Roslyn compilation.
    /// </summary>
    private const string MockTypes =
        """
        using System;
        using System.Linq;
        using System.Linq.Expressions;
        using System.Threading;
        using System.Threading.Tasks;
        using ExpressiveSharp;
        using ExpressiveSharp.Extensions;

        namespace TestNs
        {
            // Mock EF Core's SetPropertyCalls<TSource>
            class SetPropertyCalls<TSource>
            {
                public SetPropertyCalls<TSource> SetProperty<TProperty>(
                    Func<TSource, TProperty> propertyExpression,
                    TProperty value) => this;

                public SetPropertyCalls<TSource> SetProperty<TProperty>(
                    Func<TSource, TProperty> propertyExpression,
                    Func<TSource, TProperty> valueExpression) => this;
            }

            // Mock EF Core's RelationalQueryableExtensions
            static class MockRelationalExtensions
            {
                public static int ExecuteUpdate<TSource>(
                    IQueryable<TSource> source,
                    Expression<Func<SetPropertyCalls<TSource>, SetPropertyCalls<TSource>>> setPropertyCalls)
                    => 0;

                public static Task<int> ExecuteUpdateAsync<TSource>(
                    IQueryable<TSource> source,
                    Expression<Func<SetPropertyCalls<TSource>, SetPropertyCalls<TSource>>> setPropertyCalls,
                    CancellationToken cancellationToken = default)
                    => Task.FromResult(0);
            }

            // IRewritableQueryable stubs (matching the real pattern)
            static class Stubs
            {
                [PolyfillTarget(typeof(MockRelationalExtensions))]
                public static int ExecuteUpdate<TSource>(
                    this IRewritableQueryable<TSource> source,
                    Func<SetPropertyCalls<TSource>, SetPropertyCalls<TSource>> setPropertyCalls)
                    => throw new System.Diagnostics.UnreachableException();

                [PolyfillTarget(typeof(MockRelationalExtensions))]
                public static Task<int> ExecuteUpdateAsync<TSource>(
                    this IRewritableQueryable<TSource> source,
                    Func<SetPropertyCalls<TSource>, SetPropertyCalls<TSource>> setPropertyCalls,
                    CancellationToken cancellationToken = default)
                    => throw new System.Diagnostics.UnreachableException();
            }
        """;

    [TestMethod]
    public Task ExecuteUpdate_SetProperty_ConstantValue()
    {
        var source = MockTypes +
            """
                class Order { public string Tag { get; set; } }
                class TestClass
                {
                    public void Run(IQueryable<Order> query)
                    {
                        query.AsExpressive()
                             .ExecuteUpdate(s => s.SetProperty(o => o.Tag, "updated"));
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    /// <summary>
    /// Proves new capability: null-conditional operator inside SetProperty value expression.
    /// This is impossible in normal C# expression trees.
    /// </summary>
    [TestMethod]
    public Task ExecuteUpdate_SetProperty_WithNullConditional()
    {
        var source = MockTypes +
            """
                class Customer { public string? Name { get; set; } }
                class Order
                {
                    public string Tag { get; set; }
                    public Customer? Customer { get; set; }
                }
                class TestClass
                {
                    public void Run(IQueryable<Order> query)
                    {
                        query.AsExpressive()
                             .ExecuteUpdate(s => s.SetProperty(
                                 o => o.Tag,
                                 o => o.Customer?.Name ?? "none"));
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    /// <summary>
    /// Proves new capability: switch expression inside SetProperty value expression.
    /// This is impossible in normal C# expression trees.
    /// </summary>
    [TestMethod]
    public Task ExecuteUpdate_SetProperty_WithSwitchExpression()
    {
        var source = MockTypes +
            """
                class Order
                {
                    public string Tag { get; set; }
                    public int Amount { get; set; }
                }
                class TestClass
                {
                    public void Run(IQueryable<Order> query)
                    {
                        query.AsExpressive()
                             .ExecuteUpdate(s => s.SetProperty(
                                 o => o.Tag,
                                 o => o.Amount switch
                                 {
                                     > 100 => "high",
                                     > 50 => "medium",
                                     _ => "low"
                                 }));
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task ExecuteUpdateAsync_GeneratesInterceptor()
    {
        var source = MockTypes +
            """
                class Order { public string Tag { get; set; } }
                class TestClass
                {
                    public async Task Run(IQueryable<Order> query)
                    {
                        await query.AsExpressive()
                                   .ExecuteUpdateAsync(s => s.SetProperty(o => o.Tag, "updated"));
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }
}
