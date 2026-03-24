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

    [TestMethod]
    public Task CollectionInitializer_ListOfInt()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public List<int> Numbers => new List<int> { 1, 2, 3 };
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task CollectionInitializer_DictionaryAdd()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public Dictionary<string, int> Map => new Dictionary<string, int> { { "a", 1 }, { "b", 2 } };
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task IndexFromEnd_WorksAsExpression()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int[] Items { get; set; }

                    [Expressive]
                    public int Last => Items[^1];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task Range_WorksAsExpression()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Start { get; set; }
                    public int End { get; set; }

                    [Expressive]
                    public Range Slice => Start..End;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task TupleBinary_Equality()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public (int X, int Y) A { get; set; }
                    public (int X, int Y) B { get; set; }

                    [Expressive]
                    public bool Same => A == B;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task TupleBinary_Inequality()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public (int X, int Y) A { get; set; }
                    public (int X, int Y) B { get; set; }

                    [Expressive]
                    public bool Different => A != B;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task WithExpression_OnRecord()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                record Point(int X, int Y);

                class C {
                    public Point P { get; set; }

                    [Expressive]
                    public Point Shifted => P with { X = P.X + 1 };
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task CollectionExpression_Array()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    [Expressive]
                    public int[] Numbers => [1, 2, 3];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task CollectionExpression_ArrayWithSpread()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int[] Items { get; set; }

                    [Expressive]
                    public int[] Combined => [1, ..Items, 2];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task CollectionExpression_ListWithSpread()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public List<int> Items { get; set; }

                    [Expressive]
                    public List<int> Combined => [1, ..Items, 2];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task CollectionExpression_SpreadOnly()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int[] Items { get; set; }

                    [Expressive]
                    public int[] Copy => [..Items];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task CollectionExpression_MultipleSpreads()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int[] Items { get; set; }
                    public int[] Others { get; set; }

                    [Expressive]
                    public int[] All => [..Items, ..Others];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ListPattern_FixedLength()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public List<int> Items { get; set; }

                    [Expressive]
                    public bool IsOneTwoThree => Items is [1, 2, 3];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ListPattern_WithSlice()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public List<int> Items { get; set; }

                    [Expressive]
                    public bool StartsWithOne => Items is [1, ..];
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
}
