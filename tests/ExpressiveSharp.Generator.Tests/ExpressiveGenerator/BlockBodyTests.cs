using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class BlockBodyTests : GeneratorTestBase
{
    [TestMethod]
    public void BlockBodiedMethod_WithAllowFlag_NoWarning()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public int Foo()
                    {
                        return 1;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);
    }

    [TestMethod]
    public void BlockBodiedMethod_WithoutExplicitFlag_StillWorks()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Value { get; set; }

                    [Expressive]
                    public int GetDouble()
                    {
                        return Value * 2;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);
    }

    [TestMethod]
    public Task BlockBodiedMethod_SimpleReturn()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public int Foo()
                    {
                        return 42;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task BlockBodiedMethod_WithPropertyAccess()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Bar { get; set; }

                    [Expressive]
                    public int Foo()
                    {
                        return Bar + 10;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task BlockBodiedMethod_WithIfElse()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Bar { get; set; }

                    [Expressive]
                    public int Foo()
                    {
                        if (Bar > 10)
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task BlockBodiedMethod_WithNestedIfElse()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Bar { get; set; }

                    [Expressive]
                    public string Foo()
                    {
                        if (Bar > 10)
                        {
                            return "High";
                        }
                        else if (Bar > 5)
                        {
                            return "Medium";
                        }
                        else
                        {
                            return "Low";
                        }
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task BlockBodiedMethod_WithLocalVariable()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Bar { get; set; }

                    [Expressive]
                    public int Foo()
                    {
                        var temp = Bar * 2;
                        return temp + 5;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task BlockBodiedMethod_WithTransitiveLocalVariables()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Bar { get; set; }

                    [Expressive]
                    public int Foo()
                    {
                        var a = Bar * 2;
                        var b = a + 5;
                        return b + 10;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
}
