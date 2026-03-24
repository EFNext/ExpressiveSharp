using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class CollectionExpressionTests : GeneratorTestBase
{
    [TestMethod]
    public Task CollectionInitializer_ListOfInt()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public List<int> Numbers => new List<int> { 1, 2, 3 };
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task CollectionInitializer_DictionaryAdd()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public Dictionary<string, int> Map => new Dictionary<string, int> { { "a", 1 }, { "b", 2 } };
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task CollectionExpression_Array()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public int[] Numbers => [1, 2, 3];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task CollectionExpression_ArrayWithSpread()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int[] Items { get; set; }

                    [Expressive]
                    public int[] Combined => [1, ..Items, 2];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task CollectionExpression_ListWithSpread()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public List<int> Items { get; set; }

                    [Expressive]
                    public List<int> Combined => [1, ..Items, 2];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task CollectionExpression_SpreadOnly()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int[] Items { get; set; }

                    [Expressive]
                    public int[] Copy => [..Items];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task CollectionExpression_MultipleSpreads()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int[] Items { get; set; }
                    public int[] Others { get; set; }

                    [Expressive]
                    public int[] All => [..Items, ..Others];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
}
