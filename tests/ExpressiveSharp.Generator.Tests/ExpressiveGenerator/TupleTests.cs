using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class TupleTests : GeneratorTestBase
{
    [TestMethod]
    public Task TupleLiteralInMethod()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Id { get; set; }
                    public string Name { get; set; }

                    [Expressive]
                    public (int, string) GetTuple() => (Id, Name);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task TupleLiteralWithNamedElements()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Id { get; set; }
                    public string Name { get; set; }

                    [Expressive]
                    public (int Id, string Name) GetTuple() => (Id: Id, Name: Name);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task TupleLiteralWithParameters()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public (int, string) MakeTuple(int id, string name) => (id, name);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task NestedTupleLiteral()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public (int, (string, bool)) GetNested(int a, string b, bool c) => (a, (b, c));
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task TupleLiteralWith8Elements()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public (int, int, int, int, int, int, int, int) GetLarge(int a, int b, int c, int d, int e, int f, int g, int h)
                        => (a, b, c, d, e, f, g, h);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
}
