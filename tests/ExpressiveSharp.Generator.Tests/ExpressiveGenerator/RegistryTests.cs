using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class RegistryTests : GeneratorTestBase
{
    [TestMethod]
    public void NoExpressives_NoRegistry()
    {
        var compilation = CreateCompilation("""class C { }""");
        var result = RunExpressiveGenerator(compilation);

        Assert.IsNull(result.RegistryTree);
    }

    [TestMethod]
    public Task SingleProperty_RegistryContainsEntry()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Id { get; set; }
                    [Expressive]
                    public int IdPlus1 => Id + 1;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        return Verifier.Verify(result.RegistryTree!.GetText().ToString());
    }

    [TestMethod]
    public Task SingleMethod_RegistryContainsEntry()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Id { get; set; }
                    [Expressive]
                    public int AddDelta(int delta) => Id + delta;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        return Verifier.Verify(result.RegistryTree!.GetText().ToString());
    }

    [TestMethod]
    public Task MultipleExpressives_AllRegistered()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Id { get; set; }
                    [Expressive]
                    public int IdPlus1 => Id + 1;
                    [Expressive]
                    public int AddDelta(int delta) => Id + delta;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        return Verifier.Verify(result.RegistryTree!.GetText().ToString());
    }

    [TestMethod]
    public void GenericClass_NotIncludedInRegistry()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C<T> {
                    public int Id { get; set; }
                    [Expressive]
                    public int IdPlus1 => Id + 1;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.IsNull(result.RegistryTree);
    }

    [TestMethod]
    public Task GenericClass_GetsEditorBrowsableAttribute()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C<T> {
                    public int Id { get; set; }
                    [Expressive]
                    public int IdPlus1 => Id + 1;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        var attributeTree = result.AllGeneratedTrees
            .FirstOrDefault(t => t.FilePath.EndsWith(".Attributes.g.cs", StringComparison.Ordinal));

        Assert.IsNotNull(attributeTree);

        return Verifier.Verify(attributeTree!.GetText().ToString());
    }

    [TestMethod]
    public Task MethodOverloads_BothRegistered()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class C {
                    public int Id { get; set; }
                    [Expressive]
                    public int Add(int delta) => Id + delta;
                    [Expressive]
                    public long Add(long delta) => Id + delta;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        return Verifier.Verify(result.RegistryTree!.GetText().ToString());
    }
}
