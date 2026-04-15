using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

/// <summary>
/// Tests for [Expressive(Projectable = true)] — a variant of [Expressive] that operates on a
/// writable auto-property using the C# 14 <c>field</c> keyword (or a manually declared private
/// nullable backing field). The formula is the right operand of the <c>??</c> coalesce in the
/// get accessor.
/// </summary>
[TestClass]
public class ProjectableTests : GeneratorTestBase
{
    // ── Happy paths ─────────────────────────────────────────────────────────

    [TestMethod]
    public Task SimpleProjectableProperty_FieldKeyword()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class User {
                    public string FirstName { get; set; } = "";
                    public string LastName  { get; set; } = "";

                    [Expressive(Projectable = true)]
                    public string FullName
                    {
                        get => field ?? (LastName + ", " + FirstName);
                        init => field = value;
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
    public Task SimpleProjectableProperty_ManualBackingField()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class User {
                    public string FirstName { get; set; } = "";
                    public string LastName  { get; set; } = "";

                    private string? _fullName;

                    [Expressive(Projectable = true)]
                    public string FullName
                    {
                        get => _fullName ?? (LastName + ", " + FirstName);
                        init => _fullName = value;
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
    public Task ProjectableWithSetAccessor()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class User {
                    public string FirstName { get; set; } = "";
                    public string LastName  { get; set; } = "";

                    [Expressive(Projectable = true)]
                    public string FullName
                    {
                        get => field ?? (LastName + ", " + FirstName);
                        set => field = value;
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
    public Task NullableValueTypeProperty_Ternary_FieldKeyword()
    {
        // Issue #35, attempt 1: `decimal?` property with the ternary + has-value-flag pattern.
        // The flag distinguishes "not materialized" from "materialized to null", so nullable
        // property types are permitted here (unlike the coalesce shape).
        var compilation = CreateCompilation(
            """
            #nullable enable
            namespace Foo {
                partial class Account {
                    public decimal? TotalAmount { get; init; }
                    public decimal? Discount    { get; init; }

                    private bool _amountHasValue;

                    [Expressive(Projectable = true)]
                    public decimal? Amount
                    {
                        get => _amountHasValue ? field : (
                            TotalAmount != null && Discount != null
                                ? System.Math.Round(TotalAmount.Value - Discount.Value, 2)
                                : (decimal?)null);
                        init { _amountHasValue = true; field = value; }
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
    public Task NonNullableValueTypeProperty_Ternary_FieldKeyword()
    {
        // Issue #35, attempt 2: `decimal` property with the ternary + has-value-flag pattern.
        // Coalesce `field ?? ...` doesn't compile when the backing field is non-nullable; the
        // ternary pattern is the supported path here.
        var compilation = CreateCompilation(
            """
            namespace Foo {
                partial class Account {
                    public decimal? TotalAmount { get; init; }
                    public decimal? Discount    { get; init; }

                    private bool _amountHasValue;

                    [Expressive(Projectable = true)]
                    public decimal Amount
                    {
                        get => _amountHasValue ? field : (
                            TotalAmount != null && Discount != null
                                ? System.Math.Round(TotalAmount.Value - Discount.Value, 2)
                                : 0m);
                        init { _amountHasValue = true; field = value; }
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
    public Task NonNullableValueTypeProperty_Coalesce_ManualNullableBackingField()
    {
        // Issue #35, attempt 3: `decimal` property with a manual `decimal? _amount` backing field
        // used via coalesce. The `_amount = value` assignment in the init accessor wraps `value`
        // in an implicit conversion (decimal → decimal?); the setter validator must peek through
        // it to see the plain `value` parameter reference.
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Account {
                    public decimal? TotalAmount { get; init; }
                    public decimal? Discount    { get; init; }

                    private decimal? _amount;

                    [Expressive(Projectable = true)]
                    public decimal Amount
                    {
                        get => _amount ?? (
                            TotalAmount != null && Discount != null
                                ? System.Math.Round(TotalAmount.Value - Discount.Value, 2)
                                : 0m);
                        init => _amount = value;
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
    public Task NonNullableValueTypeProperty_Ternary_ManualBackingField()
    {
        // `decimal` property with a manual non-nullable `decimal _amount` backing field plus a
        // separate has-value flag. Exercises the ternary + manual-backing-field path.
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Account {
                    public decimal? TotalAmount { get; init; }
                    public decimal? Discount    { get; init; }

                    private decimal _amount;
                    private bool    _amountHasValue;

                    [Expressive(Projectable = true)]
                    public decimal Amount
                    {
                        get => _amountHasValue ? _amount : (
                            TotalAmount != null && Discount != null
                                ? System.Math.Round(TotalAmount.Value - Discount.Value, 2)
                                : 0m);
                        init { _amountHasValue = true; _amount = value; }
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
    public void ProjectableRegistryKeyIsPropertyGetter()
    {
        // Load-bearing correctness check: the ExpressionRegistry must key the lambda against the
        // property's getter handle (typeof(User).GetProperty("FullName")?.GetMethod), NOT the
        // backing field's name. If the registry were keyed off the backing field, the runtime
        // ExpressiveReplacer.VisitMember lookup would silently never fire.
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class User {
                    public string FirstName { get; set; } = "";
                    public string LastName  { get; set; } = "";

                    [Expressive(Projectable = true)]
                    public string FullName
                    {
                        get => field ?? (LastName + ", " + FirstName);
                        init => field = value;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.IsNotNull(result.RegistryTree, "Registry should be generated");

        var registryText = result.RegistryTree!.GetText().ToString();
        StringAssert.Contains(registryText, "GetProperty(\"FullName\"",
            "Registry must key the lambda on the property's name (FullName), not the backing field's name");
        Assert.IsFalse(registryText.Contains("k__BackingField"),
            "Registry must NOT reference any compiler-generated backing field name");
        Assert.IsFalse(registryText.Contains("_fullName") || registryText.Contains("_FullName"),
            "Registry must NOT reference any manually-declared backing field name");
    }

    // ── Diagnostic Tests ────────────────────────────────────────────────────

    [TestMethod]
    public void MissingWritableAccessor_EXP0021()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class User {
                    public string FirstName { get; set; } = "";
                    public string LastName  { get; set; } = "";

                    [Expressive(Projectable = true)]
                    public string FullName => field ?? (LastName + ", " + FirstName);
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Id == "EXP0021"));
    }

    [TestMethod]
    public void NonCoalesceGetBody_EXP0022()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class User {
                    public string FirstName { get; set; } = "";
                    public string LastName  { get; set; } = "";

                    [Expressive(Projectable = true)]
                    public string FullName
                    {
                        get => LastName + ", " + FirstName;
                        init => field = value;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Id == "EXP0022"));
    }

    [TestMethod]
    public void SetterDoesNotStoreValue_EXP0023()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class User {
                    public string FirstName { get; set; } = "";
                    public string LastName  { get; set; } = "";

                    [Expressive(Projectable = true)]
                    public string FullName
                    {
                        get => field ?? (LastName + ", " + FirstName);
                        init => field = value?.Trim() ?? "";
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Id == "EXP0023"));
    }

    [TestMethod]
    public void NullablePropertyType_EXP0024()
    {
        var compilation = CreateCompilation(
            """
            #nullable enable
            namespace Foo {
                class User {
                    public string? FirstName { get; set; }
                    public string? LastName  { get; set; }

                    [Expressive(Projectable = true)]
                    public string? FullName
                    {
                        get => field ?? (LastName + ", " + FirstName);
                        init => field = value;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Id == "EXP0024"));
    }

    [TestMethod]
    public void ManualBackingFieldWrongType_EXP0025()
    {
        // Backing field is `int?` but property is `string` — type mismatch.
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class User {
                    public string FirstName { get; set; } = "";
                    public string LastName  { get; set; } = "";

                    private int? _wrong;

                    [Expressive(Projectable = true)]
                    public string FullName
                    {
                        #pragma warning disable CS8603
                        get => (_wrong.HasValue ? _wrong.ToString() : null) ?? (LastName + ", " + FirstName);
                        #pragma warning restore CS8603
                        init => field = value;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        // The top-level ?? has a method-call left side, not a field reference. Should be EXP0022.
        Assert.IsTrue(
            result.Diagnostics.Any(d => d.Id == "EXP0022" || d.Id == "EXP0025"),
            "Expected either EXP0022 (pattern mismatch) or EXP0025 (backing field type mismatch)");
    }

    [TestMethod]
    public void StaticBackingField_EXP0022()
    {
        // A static backing field would share materialized state across all instances.
        // It must be rejected so per-entity semantics are preserved.
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class User {
                    public string FirstName { get; set; } = "";
                    public string LastName  { get; set; } = "";

                    private static string? _shared;

                    [Expressive(Projectable = true)]
                    public string FullName
                    {
                        get => _shared ?? (LastName + ", " + FirstName);
                        init => _shared = value;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.IsTrue(
            result.Diagnostics.Any(d => d.Id == "EXP0022"),
            "Static backing fields must be rejected with EXP0022 (pattern mismatch)");
    }

    [TestMethod]
    public void RequiredModifier_EXP0026()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class User {
                    public string FirstName { get; set; } = "";
                    public string LastName  { get; set; } = "";

                    [Expressive(Projectable = true)]
                    public required string FullName
                    {
                        get => field ?? (LastName + ", " + FirstName);
                        init => field = value;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Id == "EXP0026"));
    }

    [TestMethod]
    public void InterfaceProperty_EXP0028()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                interface IUser {
                    string FirstName { get; }
                    string LastName  { get; }

                    [Expressive(Projectable = true)]
                    string FullName
                    {
                        get => field ?? (LastName + ", " + FirstName);
                        init => field = value;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Id == "EXP0028"));
    }

    [TestMethod]
    public void InvertedTernaryCondition_EXP0022()
    {
        // Only the bare `flag ? field : formula` form is supported in v1. Inverted conditions
        // (`!flag ? formula : field`) are rejected with a pointed EXP0022 reason.
        var compilation = CreateCompilation(
            """
            namespace Foo {
                partial class Account {
                    public decimal? TotalAmount { get; init; }
                    private bool _amountHasValue;

                    [Expressive(Projectable = true)]
                    public decimal Amount
                    {
                        get => !_amountHasValue ? (TotalAmount ?? 0m) : field;
                        init { _amountHasValue = true; field = value; }
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Id == "EXP0022"));
    }

    [TestMethod]
    public void TernaryFlagIsNullableBool_EXP0022()
    {
        // The has-value flag must be exactly `bool`, not `bool?`.
        var compilation = CreateCompilation(
            """
            namespace Foo {
                partial class Account {
                    public decimal? TotalAmount { get; init; }
                    private bool? _amountHasValue;

                    [Expressive(Projectable = true)]
                    public decimal Amount
                    {
                        get => _amountHasValue == true ? field : (TotalAmount ?? 0m);
                        init { _amountHasValue = true; field = value; }
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Id == "EXP0022"));
    }

    [TestMethod]
    public void TernaryFlagIsReadonly_EXP0023()
    {
        // The has-value flag must not be readonly — the setter needs to write to it.
        var compilation = CreateCompilation(
            """
            namespace Foo {
                partial class Account {
                    public decimal? TotalAmount { get; init; }
                    private readonly bool _amountHasValue;

                    [Expressive(Projectable = true)]
                    public decimal Amount
                    {
                        get => _amountHasValue ? field : (TotalAmount ?? 0m);
                        init { /* cannot assign to readonly */ field = value; }
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Id == "EXP0023"));
    }

    [TestMethod]
    public void TernarySetterMissingFlagWrite_EXP0023()
    {
        // The ternary form requires exactly two assignments: the flag AND the backing field.
        // A setter that only assigns the backing field is rejected (the flag is never set, so
        // the getter always falls through to the formula — the cache never activates).
        var compilation = CreateCompilation(
            """
            namespace Foo {
                partial class Account {
                    public decimal? TotalAmount { get; init; }
                    private bool _amountHasValue;

                    [Expressive(Projectable = true)]
                    public decimal Amount
                    {
                        get => _amountHasValue ? field : (TotalAmount ?? 0m);
                        init => field = value;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Id == "EXP0023"));
    }

    [TestMethod]
    public void TernarySetterWritesDifferentFlag_EXP0030()
    {
        // The setter writes to a different flag than the getter reads.
        var compilation = CreateCompilation(
            """
            namespace Foo {
                partial class Account {
                    public decimal? TotalAmount { get; init; }
                    private bool _amountHasValue;
                    private bool _otherFlag;

                    [Expressive(Projectable = true)]
                    public decimal Amount
                    {
                        get => _amountHasValue ? field : (TotalAmount ?? 0m);
                        init { _otherFlag = true; field = value; }
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Id == "EXP0030"));
    }

    [TestMethod]
    public void TernarySetterWritesDifferentBackingField_EXP0030()
    {
        // The setter writes to a different backing field than the getter reads.
        var compilation = CreateCompilation(
            """
            namespace Foo {
                partial class Account {
                    public decimal? TotalAmount { get; init; }
                    private bool    _amountHasValue;
                    private decimal _otherField;

                    [Expressive(Projectable = true)]
                    public decimal Amount
                    {
                        get => _amountHasValue ? field : (TotalAmount ?? 0m);
                        init { _amountHasValue = true; _otherField = value; }
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Id == "EXP0030"));
    }

    [TestMethod]
    public void NullablePropertyWithCoalescePattern_EXP0024()
    {
        // Nullable property with the coalesce shape is still rejected: the cache sentinel `null`
        // collides with a legitimately stored null value.
        var compilation = CreateCompilation(
            """
            #nullable enable
            namespace Foo {
                partial class Account {
                    public decimal? TotalAmount { get; init; }

                    [Expressive(Projectable = true)]
                    public decimal? Amount
                    {
                        get => field ?? (TotalAmount ?? 0m);
                        init => field = value;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Id == "EXP0024"));
    }

    [TestMethod]
    public void OverrideProperty_EXP0029()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class UserBase {
                    public virtual string FullName { get; init; } = "";
                }
                class User : UserBase {
                    public string FirstName { get; set; } = "";
                    public string LastName  { get; set; } = "";

                    [Expressive(Projectable = true)]
                    public override string FullName
                    {
                        get => field ?? (LastName + ", " + FirstName);
                        init => field = value;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Id == "EXP0029"));
    }
}
