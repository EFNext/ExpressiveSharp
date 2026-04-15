using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Tests;

/// <summary>
/// Provider-agnostic tests for <c>[Expressive(Projectable = true)]</c>. Verifies the dual-direction
/// runtime behavior: in-memory reads evaluate the formula (because the backing field is null),
/// while values assigned through the <c>init</c> accessor are stored and returned verbatim.
/// </summary>
[TestClass]
public class ProjectableExpressiveTests
{
    // ── In-memory runtime behavior ──────────────────────────────────────────

    [TestMethod]
    public void InMemoryConstruction_ReadsComputeFromFormula()
    {
        // Cognitive-trap regression guard: if we ever regressed back to the partial-property
        // design, this would return the default (empty string) instead of the formula.
        var entity = new ProjectableEntity { Name = "Ada", Email = "ada@example.com" };

        Assert.AreEqual("Ada <ada@example.com>", entity.DisplayLabel);
    }

    [TestMethod]
    public void InMemoryMutation_FormulaReflectsChanges()
    {
        var entity = new ProjectableEntity { Name = "Ada", Email = "ada@example.com" };
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
        // When the property is assigned via init (as EF or HC does after materialization from SQL),
        // the stored value should take precedence over the formula on subsequent reads.
        var entity = new ProjectableEntity
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
        var entity = new ProjectableEntity
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
        var entity = new ProjectableEntity { Name = null, Email = null };

        Assert.AreEqual("(unnamed) <no-email>", entity.DisplayLabel);
    }

    // ── Expression-tree expansion ──────────────────────────────────────────

    [TestMethod]
    public void ExpandExpressives_Select_RewritesProjectableToFormula()
    {
        var source = new List<ProjectableEntity>
        {
            new() { Name = "Ada",  Email = "ada@example.com" },
            new() { Name = "Alan", Email = "alan@example.com" },
        }.AsQueryable();

        Expression<Func<ProjectableEntity, string>> labelExpr = c => c.DisplayLabel;
        var expanded = (Expression<Func<ProjectableEntity, string>>)labelExpr.ExpandExpressives();

        // After expansion, the body is the formula — no reference to c.DisplayLabel remains.
        var labels = source.Select(expanded.Compile()).ToList();

        Assert.AreEqual(2, labels.Count);
        Assert.AreEqual("Ada <ada@example.com>", labels[0]);
        Assert.AreEqual("Alan <alan@example.com>", labels[1]);
    }

    [TestMethod]
    public void Ternary_ExpandExpressives_Select_RewritesToFormula()
    {
        var source = new List<DiscountedProjectableEntity>
        {
            new() { TotalAmount = 100m, Discount = 20m },
            new() { TotalAmount = 50m,  Discount = 5m },
        }.AsQueryable();

        Expression<Func<DiscountedProjectableEntity, decimal?>> expr = c => c.DiscountedAmount;
        var expanded = (Expression<Func<DiscountedProjectableEntity, decimal?>>)expr.ExpandExpressives();

        var values = source.Select(expanded.Compile()).ToList();

        Assert.AreEqual(2, values.Count);
        Assert.AreEqual(80m, values[0]);
        Assert.AreEqual(45m, values[1]);
    }

    [TestMethod]
    public void ExpandExpressives_MemberInit_RewritesRhsOfProjection()
    {
        // Projection middleware pattern: `new T { DisplayLabel = src.DisplayLabel }`.
        // The RHS references a Projectable member and must be rewritten.
        var source = new List<ProjectableEntity>
        {
            new() { Name = "Ada",  Email = "ada@example.com" },
        }.AsQueryable();

        Expression<Func<ProjectableEntity, ProjectableEntity>> projectExpr = c => new ProjectableEntity
        {
            Name = c.Name,
            Email = c.Email,
            DisplayLabel = c.DisplayLabel,
        };
        var expanded = (Expression<Func<ProjectableEntity, ProjectableEntity>>)projectExpr.ExpandExpressives();
        var projected = source.Select(expanded.Compile()).ToList();

        Assert.AreEqual(1, projected.Count);
        // The init accessor stored the formula's result; the stored value wins on read.
        Assert.AreEqual("Ada <ada@example.com>", projected[0].DisplayLabel);
    }
}

/// <summary>
/// Test-local fixture with a Projectable property. Declared here to keep the
/// Projectable dependency out of the shared Store scenario models.
/// </summary>
public class ProjectableEntity
{
    public string? Name { get; set; }
    public string? Email { get; set; }

    [Expressive(Projectable = true)]
    public string DisplayLabel
    {
        get => field ?? ((Name ?? "(unnamed)") + " <" + (Email ?? "no-email") + ">");
        init => field = value;
    }
}

/// <summary>
/// Exercises the ternary + has-value-flag projectable pattern with a nullable value-type
/// property (<c>decimal?</c>). The flag distinguishes "not materialized" from "materialized
/// to null", which the coalesce shape cannot do for nullable property types.
/// </summary>
public class DiscountedProjectableEntity
{
    public decimal? TotalAmount { get; set; }
    public decimal? Discount { get; set; }

    private bool _discountedAmountHasValue;

    [Expressive(Projectable = true)]
    public decimal? DiscountedAmount
    {
        get => _discountedAmountHasValue
            ? field
            : (TotalAmount != null && Discount != null
                ? TotalAmount.Value - Discount.Value
                : (decimal?)null);
        init
        {
            _discountedAmountHasValue = true;
            field = value;
        }
    }
}
