using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class CheckedArithmeticTests : GeneratorTestBase
{
    [TestMethod]
    public Task CheckedAddition()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int A { get; set; }
                    public int B { get; set; }

                    [Expressive]
                    public int Sum => checked(A + B);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task CheckedSubtraction()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int A { get; set; }
                    public int B { get; set; }

                    [Expressive]
                    public int Diff => checked(A - B);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task CheckedMultiplication()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int A { get; set; }
                    public int B { get; set; }

                    [Expressive]
                    public int Product => checked(A * B);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task CheckedNegation()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int A { get; set; }

                    [Expressive]
                    public int Negated => checked(-A);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task CheckedConversion()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public long Value { get; set; }

                    [Expressive]
                    public int Truncated => checked((int)Value);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
}
