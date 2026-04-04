using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

[TestClass]
public class WhereTests : GeneratorTestBase
{
    [TestMethod]
    public Task Where_SimpleCondition_GeneratesInterceptor()
    {
        var source = 
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public string Tag { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.AsExpressive().Where(o => o.Tag == "urgent").ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public void Where_PlainIQueryable_NotIntercepted()
    {
        var source = 
            """
            namespace TestNs
            {
                class Order { public string Tag { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.Where(o => o.Tag == "urgent").ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(0, result.GeneratedTrees.Length);
    }

    [TestMethod]
    public Task Where_CapturedVariable_GeneratesClosureAccess()
    {
        var source =
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public double Price { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query, double minPrice)
                    {
                        query.AsExpressive().Where(o => o.Price > minPrice).ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].GetText().ToString());
    }

    [TestMethod]
    public Task ChainedWhereAndSelect_GeneratesTwoInterceptors()
    {
        var source = 
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Order { public string Tag { get; set; } public string Name { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.AsExpressive()
                             .Where(o => o.Tag == "urgent")
                             .Select(o => o.Name)
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
