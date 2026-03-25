using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class PropertyTests : GeneratorTestBase
{
    [TestMethod]
    public void EmptyCode_Noop()
    {
        var compilation = CreateCompilation("""class C { }""");
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(0, result.GeneratedTrees.Length);
    }

    [TestMethod]
    public Task SimpleExpressiveProperty()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public int Foo => 1;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task MinimalExpressiveComputedProperty()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Bar { get; set; }

                    [Expressive]
                    public int Foo => Bar;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task SimpleExpressiveComputedProperty()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Bar { get; set; }

                    [Expressive]
                    public int Foo => Bar + 1;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task SimpleExpressiveComputedInNestedClassProperty()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public class C {
                    public class D {
                        public int Bar { get; set; }

                        [Expressive]
                        public int Foo => Bar + 1;
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
    public Task ExpressiveComputedPropertyUsingThis()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Bar { get; set; }

                    [Expressive]
                    public int Foo => this.Bar;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpressiveComputedPropertyMethod()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Bar() => 1;

                    [Expressive]
                    public int Foo => Bar();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpressivePropertyWithExplicitExpressionGetter()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public int Foo { get => 1; }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpressivePropertyWithExplicitBlockGetter()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public int Foo { get { return 1; } }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpressiveComputedPropertyWithExplicitExpressionGetter()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Bar { get; set; }

                    [Expressive]
                    public int Foo { get => Bar + 1; }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpressiveComputedPropertyWithExplicitBlockGetter()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Bar { get; set; }

                    [Expressive(AllowBlockBody = true)]
                    public int Foo { get { return Bar + 1; } }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpressivePropertyWithExplicitBlockGetterUsingThis()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Bar { get; set; }

                    [Expressive(AllowBlockBody = true)]
                    public int Foo { get { return this.Bar; } }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpressivePropertyWithExplicitBlockGetterAndMethodCall()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Bar() => 1;

                    [Expressive(AllowBlockBody = true)]
                    public int Foo { get { return Bar(); } }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public void ExpressivePropertyWithExplicitBlockGetter_AlwaysAllowed()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive(AllowBlockBody = true)]
                    public int Foo { get { return 1; } }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);
    }

    [TestMethod]
    public Task MoreComplexExpressiveComputedProperty()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Bar { get; set; }

                    [Expressive]
                    public int Foo => Bar + this.Bar + Bar;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpressivePropertyToNavigationalProperty()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class D { }

                class C {
                    public System.Collections.Generic.List<D> Dees { get; set; }

                    [Expressive]
                    public D Foo => Dees.First();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task NavigationProperties()
    {
        var compilation = CreateCompilation(
            """
            namespace Projectables.Repro;

            public class SomeEntity
            {
                public int Id { get; set; }

                public SomeEntity Parent { get; set; }

                public ICollection<SomeEntity> Children { get; set; }

                [Expressive]
                public ICollection<SomeEntity> RootChildren =>
                    Parent != null ? Parent.RootChildren : Children;
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task FooOrBar()
    {
        var compilation = CreateCompilation(
            """
            namespace Projectables.Repro;

            public class SomeEntity
            {
                public int Id { get; set; }

                public string Foo { get; set; }

                public string Bar { get; set; }

                [Expressive]
                public string FooOrBar =>
                    Foo != null ? Foo : Bar;
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task RelationalProperty()
    {
        var compilation = CreateCompilation(
            """
            namespace Foos {
                public class Foo {
                    public int Id { get; set; }
                }

                public class Bar {
                    public Foo Foo { get; set; }

                    [Expressive]
                    public int FooId => Foo.Id;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task SimpleExpressiveComputedPropertyWithSetter()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Bar { get; set; }

                    [Expressive]
                    public int Foo
                    {
                        get => Bar;
                        set => Bar = value;
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
    public Task PropertyWithCustomTransformers()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public string? Bar { get; set; }

                    [Expressive(Transformers = [typeof(ExpressiveSharp.Transformers.RemoveNullConditionalPatterns)])]
                    public int? Foo => Bar?.Length;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

}
