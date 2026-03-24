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
                    [Expressive(AllowBlockBody = true)]
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

                    [Expressive(AllowBlockBody = true)]
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
                    [Expressive(AllowBlockBody = true)]
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

                    [Expressive(AllowBlockBody = true)]
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

                    [Expressive(AllowBlockBody = true)]
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

                    [Expressive(AllowBlockBody = true)]
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

                    [Expressive(AllowBlockBody = true)]
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

                    [Expressive(AllowBlockBody = true)]
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

    [TestMethod]
    public void BlockBody_WithoutAllowFlag_ReportsError()
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

        Assert.IsTrue(result.Diagnostics.Any(d => d.Id == "EXP0020"),
            "Expected EXP0020 for block body without AllowBlockBody flag");
        Assert.AreEqual(0, result.GeneratedTrees.Length);
    }

    [TestMethod]
    public void BlockBody_WithTryCatch_ReportsWarning()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public int Foo()
                    {
                        try { return 1; } catch { return 0; }
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.IsTrue(result.Diagnostics.Any(d => d.Id == "EXP0003"),
            "Expected EXP0003 warning for try/catch in block body");
    }

    [TestMethod]
    public void BlockBody_WithAwait_ReportsError()
    {
        var compilation = CreateCompilation(
            """
            using System.Threading.Tasks;
            namespace Foo {
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public async Task<int> Foo()
                    {
                        return await Task.FromResult(1);
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.IsTrue(result.Diagnostics.Any(d => d.Id == "EXP0004"),
            "Expected EXP0004 error for await in block body");
    }

    [TestMethod]
    public void BlockBody_WithThrow_ReportsWarning()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public int Foo()
                    {
                        if (true) throw new System.Exception();
                        return 1;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.IsTrue(result.Diagnostics.Any(d => d.Id == "EXP0003"),
            "Expected EXP0003 warning for throw in block body");
    }
}
