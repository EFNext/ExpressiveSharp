using System.Linq.Expressions;
using ExpressiveSharp.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Tests;

/// <summary>
/// Provider-agnostic tests for <c>[ExpressiveProperty]</c>. Verifies the
/// dual-direction runtime behavior of the generated property: in-memory reads evaluate the
/// stub (because the backing field is not yet materialized), while values assigned through
/// the synthesized <c>init</c> accessor are stored and returned verbatim.
/// </summary>
[TestClass]
public class SynthesizedExpressiveTests
{
    // ── In-memory runtime behavior ──────────────────────────────────────────

    [TestMethod]
    public void InMemoryConstruction_ReadsComputeFromFormula()
    {
        var entity = new SynthesizedEntity { Name = "Ada", Email = "ada@example.com" };

        Assert.AreEqual("Ada <ada@example.com>", entity.DisplayLabel);
    }

    [TestMethod]
    public void InMemoryMutation_FormulaReflectsChanges()
    {
        var entity = new SynthesizedEntity { Name = "Ada", Email = "ada@example.com" };
        var firstRead = entity.DisplayLabel;
        entity.Name = "Augusta";
        var secondRead = entity.DisplayLabel;

        Assert.AreEqual("Ada <ada@example.com>", firstRead);
        Assert.AreEqual("Augusta <ada@example.com>", secondRead,
            "Mutating dependencies before materialization must propagate to the formula");
    }

    [TestMethod]
    public void InitAccessor_StoredValueWins()
    {
        var entity = new SynthesizedEntity
        {
            Name = "Ada",
            Email = "ada@example.com",
            DisplayLabel = "Custom Label",
        };

        Assert.AreEqual("Custom Label", entity.DisplayLabel,
            "Once materialized via init, the stored value must win over the formula");
    }

    [TestMethod]
    public void InitAccessor_StoredValueSurvivesDependencyMutation()
    {
        var entity = new SynthesizedEntity
        {
            Name = "Ada",
            Email = "ada@example.com",
            DisplayLabel = "Frozen Label",
        };

        entity.Name = "Augusta";

        Assert.AreEqual("Frozen Label", entity.DisplayLabel,
            "After materialization the stored field wins; mutating dependencies is a no-op");
    }

    [TestMethod]
    public void NullDependencies_FormulaUsesFallbacks()
    {
        var entity = new SynthesizedEntity { Name = null, Email = null };

        Assert.AreEqual("(unnamed) <no-email>", entity.DisplayLabel);
    }

    // ── Expression-tree expansion ──────────────────────────────────────────

    [TestMethod]
    public void ExpandExpressives_Select_RewritesSynthesizedToFormula()
    {
        var source = new List<SynthesizedEntity>
        {
            new() { Name = "Ada",  Email = "ada@example.com" },
            new() { Name = "Alan", Email = "alan@example.com" },
        }.AsQueryable();

        Expression<Func<SynthesizedEntity, string>> labelExpr = c => c.DisplayLabel;
        var expanded = (Expression<Func<SynthesizedEntity, string>>)labelExpr.ExpandExpressives();

        var labels = source.Select(expanded.Compile()).ToList();

        Assert.AreEqual(2, labels.Count);
        Assert.AreEqual("Ada <ada@example.com>", labels[0]);
        Assert.AreEqual("Alan <alan@example.com>", labels[1]);
    }

    [TestMethod]
    public void Ternary_ExpandExpressives_Select_RewritesToFormula()
    {
        var source = new List<DiscountedSynthesizedEntity>
        {
            new() { TotalAmount = 100m, Discount = 20m },
            new() { TotalAmount = 50m,  Discount = 5m },
        }.AsQueryable();

        Expression<Func<DiscountedSynthesizedEntity, decimal?>> expr = c => c.DiscountedAmount;
        var expanded = (Expression<Func<DiscountedSynthesizedEntity, decimal?>>)expr.ExpandExpressives();

        var values = source.Select(expanded.Compile()).ToList();

        Assert.AreEqual(2, values.Count);
        Assert.AreEqual(80m, values[0]);
        Assert.AreEqual(45m, values[1]);
    }

    [TestMethod]
    public void CrossReferencedSynthesized_FormulaResolvesAtRuntime()
    {
        // Issue #44: one [ExpressiveProperty] body referencing another's synthesized target.
        // FullName's body references FirstName (also synthesized). The generator-side fix lets
        // SemanticModel bind the cross-reference; the runtime path should evaluate correctly.
        var person = new CrossReferencedPerson();

        Assert.AreEqual("Jane", person.FirstName,
            "FirstName should fall through to the FirstNameExpr formula");
        Assert.AreEqual("Jane Doe", person.FullName,
            "FullName should resolve FirstName via the synthesized property's getter");
    }

    [TestMethod]
    public void CrossReferencedSynthesized_ExpandExpressives_RewritesNestedReference()
    {
        // Verify the runtime expression-tree path: ExpandExpressives must recursively expand
        // FullName → "FirstName + \" Doe\"" → "\"Jane\" + \" Doe\"".
        var source = new List<CrossReferencedPerson> { new() }.AsQueryable();

        Expression<Func<CrossReferencedPerson, string>> fullNameExpr = p => p.FullName;
        var expanded = (Expression<Func<CrossReferencedPerson, string>>)fullNameExpr.ExpandExpressives();
        var result = source.Select(expanded.Compile()).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Jane Doe", result[0]);
    }

    [TestMethod]
    public void ExpandExpressives_MemberInit_RewritesRhsOfProjection()
    {
        // Projection middleware pattern: `new T { DisplayLabel = src.DisplayLabel }`.
        // The RHS references a synthesized member and must be rewritten.
        var source = new List<SynthesizedEntity>
        {
            new() { Name = "Ada",  Email = "ada@example.com" },
        }.AsQueryable();

        Expression<Func<SynthesizedEntity, SynthesizedEntity>> projectExpr = c => new SynthesizedEntity
        {
            Name = c.Name,
            Email = c.Email,
            DisplayLabel = c.DisplayLabel,
        };
        var expanded = (Expression<Func<SynthesizedEntity, SynthesizedEntity>>)projectExpr.ExpandExpressives();
        var projected = source.Select(expanded.Compile()).ToList();

        Assert.AreEqual(1, projected.Count);
        Assert.AreEqual("Ada <ada@example.com>", projected[0].DisplayLabel);
    }
}

/// <summary>
/// Non-nullable reference-type target — exercises the coalesce shape of the synthesized property.
/// </summary>
public partial class SynthesizedEntity
{
    public string? Name { get; set; }
    public string? Email { get; set; }

    [ExpressiveProperty("DisplayLabel")]
    private string DisplayLabelExpression =>
        (Name ?? "(unnamed)") + " <" + (Email ?? "no-email") + ">";
}

/// <summary>
/// Issue #44 reproduction: one [ExpressiveProperty] stub body references another's synthesized
/// target. The generator must augment its in-memory compilation with the synthesized partials so
/// SemanticModel can bind the cross-reference.
/// </summary>
public partial class CrossReferencedPerson
{
    [ExpressiveProperty("FirstName")]
    private string FirstNameExpr => "Jane";

    [ExpressiveProperty("FullName")]
    private string FullNameExpr => FirstName + " Doe";
}

/// <summary>
/// Nullable value-type target — exercises the ternary+flag shape of the synthesized property.
/// The flag distinguishes "not materialized" from "materialized to null", which the coalesce
/// shape cannot do for nullable property types.
/// </summary>
public partial class DiscountedSynthesizedEntity
{
    public decimal? TotalAmount { get; set; }
    public decimal? Discount { get; set; }

    [ExpressiveProperty("DiscountedAmount")]
    private decimal? DiscountedAmountExpression =>
        TotalAmount != null && Discount != null
            ? TotalAmount.Value - Discount.Value
            : (decimal?)null;
}
