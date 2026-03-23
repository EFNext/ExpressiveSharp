using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class MethodTests : GeneratorTestBase
{
    [TestMethod]
    public Task SimpleExpressiveMethod()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public int Foo() => 1;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpressiveComputedMethodWithSingleArgument()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public int Foo(int i) => i;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpressiveComputedMethodWithMultipleArguments()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public int Foo(int a, string b, object d) => a;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task StaticMethodWithNoParameters()
    {
        var compilation = CreateCompilation(
            """
            public static class Foo {
                [Expressive]
                public static int Zero() => 0;
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task StaticMethodWithParameters()
    {
        var compilation = CreateCompilation(
            """
            public static class Foo {
                [Expressive]
                public static int Zero(int x) => 0;
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task StaticMembers()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public class Foo {
                    public static int Bar { get; set; }

                    public int Id { get; set; }

                    [Expressive]
                    public int IdWithBar() => Id + Bar;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task StaticMembers2()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public static class Constants {
                    public static readonly int Bar  = 1;
                }

                public class Foo {
                    public int Id { get; set; }

                    [Expressive]
                    public int IdWithBar() => Id + Constants.Bar;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ConstMember()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public class Foo {
                    public const int Bar = 1;

                    public int Id { get; set; }

                    [Expressive]
                    public int IdWithBar() => Id + Bar;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task DefaultValuesGetRemoved()
    {
        var compilation = CreateCompilation(
            """
            class Foo {
                [Expressive]
                public int Calculate(int i = 0) => i;
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ParamsModifierGetsRemoved()
    {
        var compilation = CreateCompilation(
            """
            class Foo {
                [Expressive]
                public int First(params int[] all) => all[0];
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task MethodOverloads_WithDifferentParameterTypes()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public int Method(int x) => x;

                    [Expressive]
                    public int Method(string s) => s.Length;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(2, result.GeneratedTrees.Length);

        var generatedFiles = result.GeneratedTrees.Select(t => t.FilePath).ToList();
        Assert.IsTrue(generatedFiles.Any(f => f.Contains("Method_P0_int.g.cs")));
        Assert.IsTrue(generatedFiles.Any(f => f.Contains("Method_P0_string.g.cs")));

        return Verifier.Verify(result.GeneratedTrees.Select(t => t.ToString()));
    }

    [TestMethod]
    public Task MethodOverloads_WithDifferentParameterCounts()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public int Method(int x) => x;

                    [Expressive]
                    public int Method(int x, int y) => x + y;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(2, result.GeneratedTrees.Length);

        var generatedFiles = result.GeneratedTrees.Select(t => t.FilePath).ToList();
        Assert.IsTrue(generatedFiles.Any(f => f.Contains("Method_P0_int.g.cs")));
        Assert.IsTrue(generatedFiles.Any(f => f.Contains("Method_P0_int_P1_int.g.cs")));

        return Verifier.Verify(result.GeneratedTrees.Select(t => t.ToString()));
    }

    [TestMethod]
    public Task InheritedMembers()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public class Foo {
                    public int Id { get; set; }
                }

                public class Bar : Foo {
                    [Expressive]
                    public int ProjectedId => Id;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task BaseMemberExplicitReference()
    {
        var compilation = CreateCompilation(
            """
            namespace Projectables.Repro;

            class Base
            {
                public string Foo { get; set; }
            }

            class Derived : Base
            {
                [Expressive]
                public string Bar => base.Foo;
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task BaseMemberImplicitReference()
    {
        var compilation = CreateCompilation(
            """
            namespace Projectables.Repro;

            class Base
            {
                public string Foo { get; set; }
            }

            class Derived : Base
            {
                [Expressive]
                public string Bar => Foo;
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task BaseMethodExplicitReference()
    {
        var compilation = CreateCompilation(
            """
            namespace Projectables.Repro;

            class Base
            {
                public string Foo() => "";
            }

            class Derived : Base
            {
                [Expressive]
                public string Bar => base.Foo();
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task IsOperator()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class A {
                    [Expressive]
                    public bool IsB => this is B;
                }

                class B : A {
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task Cast()
    {
        var compilation = CreateCompilation(
            """
            namespace Projectables.Repro;

            public class SuperEntity : SomeEntity
            {
                public string Superpower { get; set; }
            }

            public class SomeEntity
            {
                public int Id { get; set; }
            }

            public static class SomeExtensions
            {
                [Expressive]
                public static string AsSomeResult(this SomeEntity e) => ((SuperEntity)e).Superpower;
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task StringInterpolationWithStaticCall_IsBeingRewritten()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                static class MyExtensions {
                    public static string ToDateString(this DateTime date) => date.ToString("dd/MM/yyyy");
                }

                class C {
                    public DateTime? ValidationDate { get; set; }

                    [Expressive]
                    public string Status => ValidationDate != null ? $"Validation date : ({ValidationDate.Value.ToDateString()})" : "";
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task TypesInBodyGetsFullyQualified()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class D { }

                class C {
                    public System.Collections.Generic.List<D> Dees { get; set; }

                    [Expressive]
                    public int Foo => Dees.OfType<D>().Count();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task DeclarationTypeNamesAreGettingFullyQualified()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public static class EntityExtensions
                {
                    public record Entity
                    {
                        public int Id { get; set; }
                        public string? FullName { get; set; }

                        [Expressive]
                        public static Entity Something(Entity entity)
                            => entity;
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
    public Task RequiredNamespace()
    {
        var compilation = CreateCompilation(
            """
            namespace One {
                static class IntExtensions {
                    public static int AddOne(this int i) => i + 1;
                }
            }

            namespace One.Two {
                class Bar {
                    [Expressive]
                    public int Method() => 1.AddOne();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
}
