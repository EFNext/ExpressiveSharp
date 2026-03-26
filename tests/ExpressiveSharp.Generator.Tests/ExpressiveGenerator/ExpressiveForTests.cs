using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class ExpressiveForTests : GeneratorTestBase
{
    [TestMethod]
    public Task StaticMethod()
    {
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                static class Mappings {
                    [ExpressiveFor(typeof(System.Math), "Abs")]
                    static int Abs(int value) => value < 0 ? -value : value;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task InstanceMethod()
    {
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class MyType {
                    public string FirstName { get; set; }
                    public string LastName { get; set; }
                    public string GetFullName() => FirstName + " " + LastName;
                }

                static class Mappings {
                    [ExpressiveFor(typeof(MyType), "GetFullName")]
                    static string GetFullName(MyType obj) => obj.FirstName + " " + obj.LastName;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task InstanceProperty()
    {
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class MyType {
                    public string FirstName { get; set; }
                    public string LastName { get; set; }
                    public string FullName => FirstName + " " + LastName;
                }

                static class Mappings {
                    [ExpressiveFor(typeof(MyType), "FullName")]
                    static string FullName(MyType obj) => obj.FirstName + " " + obj.LastName;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task StaticProperty()
    {
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class MyType {
                    public static int DefaultValue => 42;
                }

                static class Mappings {
                    [ExpressiveFor(typeof(MyType), "DefaultValue")]
                    static int DefaultValue() => 42;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task OverloadDisambiguation()
    {
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                static class Mappings {
                    [ExpressiveFor(typeof(System.Math), "Max")]
                    static int MaxInt(int a, int b) => a > b ? a : b;

                    [ExpressiveFor(typeof(System.Math), "Max")]
                    static double MaxDouble(double a, double b) => a > b ? a : b;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(2, result.GeneratedTrees.Length);

        return Verifier.Verify(string.Join("\n\n// ===\n\n",
            result.GeneratedTrees.Select(t => t.ToString())));
    }

    [TestMethod]
    public void MixedRegistry()
    {
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class MyType {
                    public int Value { get; set; }

                    [Expressive]
                    public int Doubled => Value * 2;
                }

                static class Mappings {
                    [ExpressiveFor(typeof(System.Math), "Abs")]
                    static int Abs(int value) => value < 0 ? -value : value;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(2, result.GeneratedTrees.Length);

        // Verify the registry contains both entries
        Assert.IsNotNull(result.RegistryTree, "Registry should be generated");
        var registryText = result.RegistryTree!.GetText().ToString();
        Assert.IsTrue(registryText.Contains("Math"), "Registry should contain Math.Abs entry");
        Assert.IsTrue(registryText.Contains("MyType"), "Registry should contain MyType.Doubled entry");
    }

    // ── Diagnostic Tests ────────────────────────────────────────────────────

    [TestMethod]
    public void MemberNotFound_EXP0015()
    {
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                static class Mappings {
                    [ExpressiveFor(typeof(System.Math), "NonExistentMethod")]
                    static int Nope(int value) => value;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Length);
        Assert.AreEqual("EXP0015", result.Diagnostics[0].Id);
    }

    [TestMethod]
    public void StubNotStatic_EXP0016()
    {
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class Mappings {
                    [ExpressiveFor(typeof(System.Math), "Abs")]
                    int Abs(int value) => value < 0 ? -value : value;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Length);
        Assert.AreEqual("EXP0016", result.Diagnostics[0].Id);
    }

    [TestMethod]
    public void ReturnTypeMismatch_EXP0017()
    {
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                static class Mappings {
                    [ExpressiveFor(typeof(System.Math), "Abs")]
                    static string Abs(int value) => value.ToString();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Length);
        Assert.AreEqual("EXP0017", result.Diagnostics[0].Id);
    }

    [TestMethod]
    public void ConflictWithExpressive_EXP0019()
    {
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class MyType {
                    public int Value { get; set; }

                    [Expressive]
                    public int Doubled => Value * 2;
                }

                static class Mappings {
                    [ExpressiveFor(typeof(MyType), "Doubled")]
                    static int Doubled(MyType obj) => obj.Value * 2;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        // Should have EXP0019 diagnostic
        var exp0019 = result.Diagnostics.Where(d => d.Id == "EXP0019").ToArray();
        Assert.AreEqual(1, exp0019.Length);
    }
}
