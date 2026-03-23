using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class ExtensionMethodTests : GeneratorTestBase
{
    [TestMethod]
    public Task ExpressiveExtensionMethod()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class D { }

                static class C {
                    [Expressive]
                    public static int Foo(this D d) => 1;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpressiveExtensionMethod_OnValueType()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                static class C {
                    [Expressive]
                    public static int Foo(this int i) => i;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpressiveExtensionMethod_CallingAnotherExpressive()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                static class C {
                    [Expressive]
                    public static int Foo1(this int i) => i;

                    [Expressive]
                    public static int Foo2(this int i) => i.Foo1();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(2, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpressiveExtensionMethod_SelfRecursive()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                static class C {
                    [Expressive]
                    public static object Foo1(this object i) => i.Foo1();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

}
