using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class StringConcatenationTests : GeneratorTestBase
{
    [TestMethod]
    public Task StringPlusString()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public string First { get; set; }
                    public string Last { get; set; }

                    [Expressive]
                    public string FullName => First + " " + Last;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task IntPlusString()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Id { get; set; }
                    public string Name { get; set; }

                    [Expressive]
                    public string Label => Id + ": " + Name;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task SingleStringConcat()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public string A { get; set; }
                    public string B { get; set; }

                    [Expressive]
                    public string Joined => A + B;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
}
