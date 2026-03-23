using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class EnumTests : GeneratorTestBase
{
    [TestMethod]
    public Task ExpandEnumMethodsWithDisplayAttribute()
    {
        var compilation = CreateCompilation(
            """
            using System.ComponentModel.DataAnnotations;

            namespace Foo {
                public enum CustomEnum
                {
                    [Display(Name = "Value 1")]
                    Value1,

                    [Display(Name = "Value 2")]
                    Value2,
                }

                public static class EnumExtensions
                {
                    public static string GetDisplayName(this CustomEnum value)
                    {
                        return value.ToString();
                    }
                }

                public record Entity
                {
                    public int Id { get; set; }
                    public CustomEnum MyValue { get; set; }

                    [Expressive]
                    public string MyEnumName => MyValue.GetDisplayName();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpandEnumMethodsWithNullableEnum()
    {
        var compilation = CreateCompilation(
            """
            using System.ComponentModel.DataAnnotations;

            namespace Foo {
                public enum CustomEnum
                {
                    [Display(Name = "First Value")]
                    First,

                    [Display(Name = "Second Value")]
                    Second,
                }

                public static class EnumExtensions
                {
                    public static string GetDisplayName(this CustomEnum value)
                    {
                        return value.ToString();
                    }
                }

                public record Entity
                {
                    public int Id { get; set; }
                    public CustomEnum? MyValue { get; set; }

                    [Expressive]
                    public string MyEnumName => MyValue.HasValue ? MyValue.Value.GetDisplayName() : null;
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpandEnumMethodsWithDescriptionAttribute()
    {
        var compilation = CreateCompilation(
            """
            using System.ComponentModel;

            namespace Foo {
                public enum Status
                {
                    [Description("The item is pending")]
                    Pending,

                    [Description("The item is approved")]
                    Approved,

                    [Description("The item is rejected")]
                    Rejected,
                }

                public static class EnumExtensions
                {
                    public static string GetDescription(this Status value)
                    {
                        return value.ToString();
                    }
                }

                public record Entity
                {
                    public int Id { get; set; }
                    public Status Status { get; set; }

                    [Expressive]
                    public string StatusDescription => Status.GetDescription();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpandEnumMethodsReturningBoolean()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public enum Status
                {
                    Pending,
                    Approved,
                    Rejected,
                }

                public static class EnumExtensions
                {
                    public static bool IsApproved(this Status value)
                    {
                        return value == Status.Approved;
                    }
                }

                public record Entity
                {
                    public int Id { get; set; }
                    public Status Status { get; set; }

                    [Expressive]
                    public bool IsStatusApproved => Status.IsApproved();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpandEnumMethodsReturningInteger()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public enum Priority
                {
                    Low,
                    Medium,
                    High,
                }

                public static class EnumExtensions
                {
                    public static int GetSortOrder(this Priority value)
                    {
                        return (int)value;
                    }
                }

                public record Entity
                {
                    public int Id { get; set; }
                    public Priority Priority { get; set; }

                    [Expressive]
                    public int PrioritySortOrder => Priority.GetSortOrder();
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ExpandEnumMethodsWithParameter()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public enum Status
                {
                    Pending,
                    Approved,
                    Rejected,
                }

                public static class EnumExtensions
                {
                    public static string GetDisplayNameWithPrefix(this Status value, string prefix)
                    {
                        return prefix + value.ToString();
                    }
                }

                public record Entity
                {
                    public int Id { get; set; }
                    public Status Status { get; set; }

                    [Expressive]
                    public string StatusWithPrefix => Status.GetDisplayNameWithPrefix("Status: ");
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
}
