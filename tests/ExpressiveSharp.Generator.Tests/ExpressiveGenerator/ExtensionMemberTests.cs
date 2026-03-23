using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class ExtensionMemberTests : GeneratorTestBase
{
#if NET10_0_OR_GREATER
    [TestMethod]
    public Task ExtensionMemberProperty()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Entity {
                    public int Id { get; set; }
                }

                static class EntityExtensions {
                    extension(Entity e) {
                        [Expressive]
                        public int DoubleId => e.Id * 2;
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
    public Task ExtensionMemberMethod()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Entity {
                    public int Id { get; set; }
                }

                static class EntityExtensions {
                    extension(Entity e) {
                        [Expressive]
                        public int TripleId() => e.Id * 3;
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
    public Task ExtensionMemberMethodWithParameters()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Entity {
                    public int Id { get; set; }
                }

                static class EntityExtensions {
                    extension(Entity e) {
                        [Expressive]
                        public int Multiply(int factor) => e.Id * factor;
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
    public Task ExtensionMemberOnPrimitive()
    {
        var compilation = CreateCompilation(
            """
            static class IntExtensions {
                extension(int i) {
                    [Expressive]
                    public int Squared => i * i;
                }
            }
            """);

        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExtensionMemberWithMemberAccess()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Entity {
                    public int Id { get; set; }
                    public string Name { get; set; }
                }

                static class EntityExtensions {
                    extension(Entity e) {
                        [Expressive]
                        public string IdAndName => e.Id + ": " + e.Name;
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
    public Task ExtensionMemberWithBlockBody()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Entity {
                    public int Value { get; set; }
                    public bool IsActive { get; set; }
                }

                static class EntityExtensions {
                    extension(Entity e) {
                        [Expressive]
                        public string GetStatus()
                        {
                            if (e.IsActive && e.Value > 0)
                            {
                                return "Active";
                            }
                            return "Inactive";
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
    public Task ExtensionMemberWithSwitchExpression()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Entity {
                    public int Score { get; set; }
                }

                static class EntityExtensions {
                    extension(Entity e) {
                        [Expressive]
                        public string GetGrade() => e.Score switch
                        {
                            >= 90 => "A",
                            >= 80 => "B",
                            >= 70 => "C",
                            _ => "F",
                        };
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
    public Task ExtensionMemberOnInterface()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                interface IEntity {
                    int Id { get; }
                    string Name { get; }
                }

                static class IEntityExtensions {
                    extension(IEntity e) {
                        [Expressive]
                        public string Label => e.Id + ": " + e.Name;
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
    public Task ExtensionMemberWithIsPatternExpression()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Entity {
                    public int Value { get; set; }
                }

                static class EntityExtensions {
                    extension(Entity e) {
                        [Expressive]
                        public bool IsHighValue => e.Value is > 100;
                    }
                }
            }
            """);

        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
#endif
}
