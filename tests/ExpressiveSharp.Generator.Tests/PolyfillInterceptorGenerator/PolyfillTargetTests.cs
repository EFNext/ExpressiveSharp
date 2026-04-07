using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

[TestClass]
public class PolyfillTargetTests : GeneratorTestBase
{
    [TestMethod]
    public Task PolyfillTarget_RoutesToSpecifiedType()
    {
        var source =
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
                class Order { public bool Active { get; set; } }

                // Mock target type simulating EntityFrameworkQueryableExtensions
                static class MockEfExtensions
                {
                    public static Task<bool> AnyAsync<T>(IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken ct = default)
                        => Task.FromResult(false);
                }

                static class Stubs
                {
                    [PolyfillTarget(typeof(MockEfExtensions))]
                    public static Task<bool> AnyAsync<T>(
                        this IExpressiveQueryable<T> source,
                        Func<T, bool> predicate,
                        CancellationToken cancellationToken = default)
                        => throw new System.Diagnostics.UnreachableException();
                }

                class TestClass
                {
                    public void Run(IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().AnyAsync(o => o.Active);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task PolyfillTarget_WithoutAttribute_DefaultsToQueryable()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public bool Active { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var result = query.AsExpressive().Any(o => o.Active);
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }
}
