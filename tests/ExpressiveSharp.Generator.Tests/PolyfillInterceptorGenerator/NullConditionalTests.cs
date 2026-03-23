using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

[TestClass]
public class NullConditionalTests : GeneratorTestBase
{
    [TestMethod]
    public Task NullConditionalRewrite_ExpandsInWhereBody()
    {
        var source = 
            """
            using ExpressiveSharp.Extensions;

            namespace TestNs
            {
                class Customer { public string Name { get; set; } }
                class Order { public Customer Customer { get; set; } }
                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        query.WithExpressionRewrite(new ExpressionRewriteOptions())
                             .Where(o => o.Customer?.Name == "Alice")
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
    public void CustomReceiverType_NotIntercepted()
    {
        var source = 
            """
            namespace TestNs
            {
                class Order { public string Tag { get; set; } }

                class MyQueryable<T> : System.Linq.IQueryable<T>
                {
                    private readonly System.Linq.IQueryable<T> _inner;
                    public MyQueryable(System.Linq.IQueryable<T> inner) => _inner = inner;
                    public Type ElementType => _inner.ElementType;
                    public System.Linq.Expressions.Expression Expression => _inner.Expression;
                    public System.Linq.IQueryProvider Provider => _inner.Provider;
                    public System.Collections.Generic.IEnumerator<T> GetEnumerator() => _inner.GetEnumerator();
                    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _inner.GetEnumerator();
                }

                static class MyExtensions
                {
                    public static MyQueryable<T> Where<T>(this MyQueryable<T> source, Func<T, bool> predicate) => source;
                }

                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Order> query)
                    {
                        var myQ = new MyQueryable<Order>(query);
                        myQ.Where(o => o.Tag == "urgent");
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(0, result.GeneratedTrees.Length);
    }
}
