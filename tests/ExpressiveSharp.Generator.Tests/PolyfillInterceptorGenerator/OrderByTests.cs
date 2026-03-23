using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

[TestClass]
public class OrderByTests : GeneratorTestBase
{
    [TestMethod]
    public Task OrderBy_GeneratesInterceptor()
    {
        var source = 
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int Priority { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.WithExpressionRewrite().OrderBy(o => o.Priority).ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task ThenBy_CastsToIOrderedQueryable()
    {
        var source = 
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public int Priority { get; set; } public string Name { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.WithExpressionRewrite()
                             .OrderBy(o => o.Priority)
                             .ThenBy(o => o.Name)
                             .ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }
}
