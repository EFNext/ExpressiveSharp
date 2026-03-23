using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class GenericTests : GeneratorTestBase
{
    [TestMethod]
    public Task GenericMethods_AreRewritten()
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
                    }

                    [Expressive]
                    public static string EnforceString<T>(T value) where T : unmanaged
                        => value.ToString();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task GenericClassesWithConstraints_AreRewritten()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public class TypedObject<TEnum> where TEnum : struct, System.Enum
                {
                    public TEnum SomeProp { get; set; }
                }

                public abstract class Entity<T, TEnum> where T : TypedObject<TEnum> where TEnum : struct, System.Enum
                {
                    public int Id { get; set; }
                    public string FirstName { get; set; }
                    public string LastName { get; set; }
                    public T SomeSubobject { get; set; }

                    [Expressive]
                    public string FullName => $"{FirstName} {LastName} {SomeSubobject.SomeProp}";
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task GenericClassesWithTypeConstraints_AreRewritten()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public abstract class Entity<T> where T : notnull
                {
                    public int Id { get; set; }
                    public string FirstName { get; set; }
                    public string LastName { get; set; }
                    public T SomeSubobject { get; set; }

                    [Expressive]
                    public string FullName => $"{FirstName} {LastName} {SomeSubobject}";
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task GenericTypes()
    {
        var compilation = CreateCompilation(
            """
            class EntityBase<TId> {
                [Expressive]
                public static TId GetId() => default;
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task GenericTypesWithConstraints()
    {
        var compilation = CreateCompilation(
            """
            class EntityBase<TId> where TId : ICloneable, new() {
                [Expressive]
                public static TId GetId() => default;
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
}
