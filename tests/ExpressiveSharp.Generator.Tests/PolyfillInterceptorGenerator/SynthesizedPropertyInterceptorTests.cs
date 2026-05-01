using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.PolyfillInterceptorGenerator;

/// <summary>
/// Locks in the cross-generator visibility behavior: when a lambda references a property
/// synthesized by <c>[ExpressiveProperty]</c> (declared by <c>ExpressiveGenerator</c>),
/// <c>PolyfillInterceptorGenerator</c> must augment its in-memory compilation with the
/// synthesized partial-class declarations so the SemanticModel can bind the synthesized name.
/// Without the augmentation, the interceptor was silently skipped.
/// </summary>
[TestClass]
public class SynthesizedPropertyInterceptorTests : GeneratorTestBase
{
    [TestMethod]
    public Task Where_PropertyPattern_ReferencesSynthesizedProperty()
    {
        var source =
            """
            using ExpressiveSharp;
            using ExpressiveSharp.Mapping;

            namespace TestNs
            {
                public abstract class Shape { public string Name { get; set; } }

                public partial class Rectangle : Shape
                {
                    public double Width { get; set; }
                    public double Height { get; set; }

                    [ExpressiveProperty("IsSquareProp")]
                    public bool IsSquare => Width == Height;
                }

                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Shape> query)
                    {
                        query.AsExpressive()
                             .Where(s => s is Rectangle { IsSquareProp: true })
                             .ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        var generated = result.GeneratedTrees[0].GetText().ToString();
        StringAssert.Contains(generated, "typeof(global::TestNs.Rectangle).GetProperty(\"IsSquareProp\",");

        return Verifier.Verify(generated);
    }

    [TestMethod]
    public Task Select_ReferencesSynthesizedProperty()
    {
        var source =
            """
            using ExpressiveSharp;
            using ExpressiveSharp.Mapping;

            namespace TestNs
            {
                public partial class Account
                {
                    public string FirstName { get; set; }
                    public string LastName { get; set; }

                    [ExpressiveProperty("FullName")]
                    public string FullNameExpression => FirstName + " " + LastName;
                }

                class TestClass
                {
                    public void Run(System.Linq.IQueryable<Account> query)
                    {
                        query.AsExpressive()
                             .Select(a => a.FullName)
                             .ToList();
                    }
                }
            }
            """;
        var result = RunPolyfillInterceptorGenerator(CreateCompilation(source));

        Assert.AreEqual(1, result.GeneratedTrees.Length);

        var generated = result.GeneratedTrees[0].GetText().ToString();
        StringAssert.Contains(generated, "typeof(global::TestNs.Account).GetProperty(\"FullName\",");

        return Verifier.Verify(generated);
    }
}
