using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class SwitchPatternTests : GeneratorTestBase
{
    [TestMethod]
    public Task SwitchExpressionWithConstantPattern()
    {
        var compilation = CreateCompilation(
            """
            class Foo {
                public int? FancyNumber { get; set; }

                [Expressive]
                public int SomeNumber(int input) => input switch {
                        1 => 2,
                        3 => 4,
                        4 when FancyNumber == 12 => 48,
                        _ => 1000,
                    };
                }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task SwitchExpressionWithTypePattern()
    {
        var compilation = CreateCompilation(
            """
            public abstract class Item
            {
                public int Id { get; set; }
                public string Name { get; set; }
            }

            public class GroupItem : Item
            {
                public string Description { get; set; }
            }

            public class DocumentItem : Item
            {
                public int Priority { get; set; }
            }

            public abstract record ItemData(int Id, string Name);
            public record GroupData(int Id, string Name, string Description) : ItemData(Id, Name);
            public record DocumentData(int Id, string Name, int Priority) : ItemData(Id, Name);

            public static class ItemMapper
            {
                [Expressive]
                public static ItemData ToData(this Item item) =>
                    item switch
                    {
                        GroupItem groupItem => new GroupData(groupItem.Id, groupItem.Name, groupItem.Description),
                        DocumentItem documentItem => new DocumentData(documentItem.Id, documentItem.Name, documentItem.Priority),
                        _ => null!
                    };
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task SwitchExpression_WithRelationalPattern()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Entity {
                    public int Score { get; set; }

                    [Expressive]
                    public string GetGrade() => Score switch
                    {
                        >= 90 => "A",
                        >= 80 => "B",
                        >= 70 => "C",
                        _ => "F",
                    };
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task SwitchExpression_WithRelationalPattern_OnExtensionMethod()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Order {
                    public decimal Amount { get; set; }
                }

                static class OrderExtensions {
                    [Expressive]
                    public static string GetTier(this Order order) => order.Amount switch
                    {
                        >= 1000 => "Platinum",
                        >= 500  => "Gold",
                        >= 100  => "Silver",
                        _       => "Bronze",
                    };
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpressionBodied_IsPattern_WithAndPattern()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Entity {
                    public int Value { get; set; }

                    [Expressive]
                    public bool IsInRange => Value is >= 1 and <= 100;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpressionBodied_IsPattern_WithOrPattern()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Entity {
                    public int Value { get; set; }

                    [Expressive]
                    public bool IsOutOfRange => Value is 0 or > 100;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpressionBodied_IsPattern_WithNotNullPattern()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Entity {
                    public string? Name { get; set; }

                    [Expressive]
                    public bool HasName => Name is not null;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
}
