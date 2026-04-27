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

        Assert.IsNotNull(result.RegistryTree, "Registry should be generated");
        var registryText = result.RegistryTree!.GetText().ToString();
        Assert.IsTrue(registryText.Contains("Math"), "Registry should contain Math.Abs entry");
        Assert.IsTrue(registryText.Contains("MyType"), "Registry should contain MyType.Doubled entry");
    }

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
    public void InstanceStubOnUnrelatedType_Rejected_EXP0015()
    {
        // Instance stub targeting System.Math.Abs — stub's containing type is `Mappings`,
        // which does not match `System.Math`, so no member should be found.
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
        Assert.AreEqual("EXP0015", result.Diagnostics[0].Id);
    }

    [TestMethod]
    public Task InstanceStub_OnInstanceProperty_SameType()
    {
        // Instance stub on the target type — `this` is the receiver.
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class MyType {
                    public string FirstName { get; set; }
                    public string LastName { get; set; }
                    public string FullName => FirstName + " " + LastName;

                    [ExpressiveFor(typeof(MyType), "FullName")]
                    string FullNameExpr() => FirstName + " " + LastName;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task SingleArgForm_DefaultsToContainingType()
    {
        // [ExpressiveFor(nameof(X))] without typeof — target defaults to the stub's containing type.
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class MyType {
                    public string FirstName { get; set; }
                    public string LastName { get; set; }
                    public string FullName => FirstName + " " + LastName;

                    [ExpressiveFor(nameof(FullName))]
                    string FullNameExpr() => FirstName + " " + LastName;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task PropertyStub_InstanceProperty_SameType()
    {
        // [ExpressiveFor] on an expression-bodied PROPERTY — cleanest same-type form.
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class MyType {
                    public string FirstName { get; set; }
                    public string LastName { get; set; }
                    public string FullName { get; set; }

                    [ExpressiveFor(nameof(FullName))]
                    private string FullNameExpression => FirstName + " " + LastName;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task SingleArgForm_InstanceMethodTarget()
    {
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class MyType {
                    public int Base { get; set; }
                    public int AddAndDouble(int x) => (Base + x) * 2;

                    [ExpressiveFor(nameof(AddAndDouble))]
                    int AddAndDoubleExpr(int x) => (Base + x) * 2;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
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

        var exp0019 = result.Diagnostics.Where(d => d.Id == "EXP0019").ToArray();
        Assert.AreEqual(1, exp0019.Length);
    }

    [TestMethod]
    public void DuplicateMapping_EXP0020()
    {
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                static class Mappings1 {
                    [ExpressiveFor(typeof(System.Math), "Abs")]
                    static int Abs(int value) => value < 0 ? -value : value;
                }

                static class Mappings2 {
                    [ExpressiveFor(typeof(System.Math), "Abs")]
                    static int Abs(int value) => value >= 0 ? value : -value;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        var exp0020 = result.Diagnostics.Where(d => d.Id == "EXP0020").ToArray();
        Assert.AreEqual(2, exp0020.Length);
    }

    // Each test below exercises one specific branch of
    // ExpressiveForSignatureMatcher (method/property, stub kind, static/instance).
    // Happy-path acceptance is covered by the snapshot tests above; this block
    // focuses on the rejection branches (param-count, receiver-type, containing-type,
    // param-type mismatches) so every matrix cell has a test.

    [TestMethod]
    public Task StaticStub_StaticPropertyTarget_Match()
    {
        // Method stub → static property, both parameterless. Exercises
        // MatchesPropertyFromMethodStub static/static branch.
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class MyType {
                    public static int DefaultValue => 42;
                }

                static class Mappings {
                    [ExpressiveFor(typeof(MyType), nameof(MyType.DefaultValue))]
                    static int DefaultValue() => 99;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public void InstanceStub_StaticPropertyTarget_Rejected_EXP0015()
    {
        // Static property + instance property stub → never matches (no way to supply receiver).
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class MyType {
                    public static int DefaultValue => 42;

                    [ExpressiveFor(nameof(DefaultValue))]
                    int DefaultValueExpression => 99;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Length);
        Assert.AreEqual("EXP0015", result.Diagnostics[0].Id);
    }

    [TestMethod]
    public void PropertyStub_TargetingMethod_Rejected_EXP0015()
    {
        // Property stubs can only target properties — even if a matching method exists.
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class MyType {
                    public int Value { get; set; }
                    public int Compute() => Value * 2;

                    [ExpressiveFor(nameof(Compute))]
                    int ComputeExpression => Value * 2;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Length);
        Assert.AreEqual("EXP0015", result.Diagnostics[0].Id);
    }

    [TestMethod]
    public void StaticStub_InstanceMethod_WrongReceiverType_Rejected_EXP0015()
    {
        // Static stub over instance method, but the explicit receiver param is the wrong type.
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class MyType {
                    public int Value { get; set; }
                    public int Add(int x) => Value + x;
                }

                static class Mappings {
                    [ExpressiveFor(typeof(MyType), nameof(MyType.Add))]
                    static int Add(string wrongType, int x) => x;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Length);
        Assert.AreEqual("EXP0015", result.Diagnostics[0].Id);
    }

    [TestMethod]
    public void StaticStub_MethodTarget_WrongParamType_Rejected_EXP0015()
    {
        // Param count matches but a param type differs — hits the matcher's per-param loop.
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                static class Mappings {
                    [ExpressiveFor(typeof(System.Math), nameof(System.Math.Abs))]
                    static int Abs(string s) => 0;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Length);
        Assert.AreEqual("EXP0015", result.Diagnostics[0].Id);
    }

    [TestMethod]
    public void StaticStub_InstanceMethod_ParamCountMismatch_Rejected_EXP0015()
    {
        // Instance method has 1 param; static stub provides [receiver] only (missing the arg).
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class MyType {
                    public int Value { get; set; }
                    public int Add(int x) => Value + x;
                }

                static class Mappings {
                    [ExpressiveFor(typeof(MyType), nameof(MyType.Add))]
                    static int Add(MyType receiver) => receiver.Value;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Length);
        Assert.AreEqual("EXP0015", result.Diagnostics[0].Id);
    }

    [TestMethod]
    public void PropertyStub_WithExplicitTargetType_WrongContainingType_Rejected_EXP0015()
    {
        // Property stub must be on the target type; [ExpressiveFor(typeof(Other))] on a stub
        // whose containing type is not Other cannot supply a receiver from `this`.
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class Other {
                    public string Name { get; set; }
                }

                class MyType {
                    [ExpressiveFor(typeof(Other), nameof(Other.Name))]
                    string NameExpression => "hardcoded";
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Length);
        Assert.AreEqual("EXP0015", result.Diagnostics[0].Id);
    }

    [TestMethod]
    public void InstanceStub_InstanceMethod_ParamCountMismatch_Rejected_EXP0015()
    {
        // Instance stub on target type, but arg count doesn't match the target method.
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class MyType {
                    public int Value { get; set; }
                    public int Add(int x, int y) => Value + x + y;

                    [ExpressiveFor(nameof(Add))]
                    int AddExpression(int x) => Value + x;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Length);
        Assert.AreEqual("EXP0015", result.Diagnostics[0].Id);
    }

    [TestMethod]
    public void SingleArgForm_UnknownMember_Rejected_EXP0015()
    {
        // Single-arg form with a name that doesn't exist on the stub's containing type.
        var compilation = CreateCompilation(
            """
            using ExpressiveSharp.Mapping;

            namespace Foo {
                class MyType {
                    public int Value { get; set; }

                    [ExpressiveFor("NoSuchMember")]
                    int NoSuchMemberExpression => Value;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Length);
        Assert.AreEqual("EXP0015", result.Diagnostics[0].Id);
    }
}
