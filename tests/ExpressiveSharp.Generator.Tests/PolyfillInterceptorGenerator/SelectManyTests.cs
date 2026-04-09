using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

[TestClass]
public class SelectManyTests : GeneratorTestBase
{
    [TestMethod]
    public Task SelectMany_CollectionSelector_GeneratesInterceptor()
    {
        var source =
            """
            using ExpressiveSharp;

            namespace TestNs
            {
                class Order { public System.Collections.Generic.List<string> Tags { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.AsExpressive()
                             .SelectMany(o => o.Tags)
                             .ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task SelectMany_WithResultSelector_GeneratesInterceptor()
    {
        var source =
            """
            using ExpressiveSharp;

            namespace TestNs
            {
                class Order { public System.Collections.Generic.List<string> Tags { get; set; } public string Name { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.AsExpressive()
                             .SelectMany(o => o.Tags, (o, t) => o.Name + ": " + t)
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
