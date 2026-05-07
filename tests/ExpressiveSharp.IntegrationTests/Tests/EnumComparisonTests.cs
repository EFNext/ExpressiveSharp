using System.Linq.Expressions;
using ExpressiveSharp.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Tests;

[TestClass]
public class EnumComparisonTests
{
    [TestMethod]
    public void EnumLessThanOrEqual_ExpandExpressives_MaterializesAndEvaluates()
    {
        var source = new List<EnumComparisonEntity>
        {
            new() { Value = Bucket.Low },
            new() { Value = Bucket.Mid },
            new() { Value = Bucket.High },
        }.AsQueryable();

        Expression<Func<EnumComparisonEntity, string>> expr = e => e.Tier;
        var expanded = (Expression<Func<EnumComparisonEntity, string>>)expr.ExpandExpressives();

        var tiers = source.Select(expanded.Compile()).ToList();

        CollectionAssert.AreEqual(new[] { "low", "mid", "high" }, tiers);
    }

    [TestMethod]
    public void EnumLessThan_ExpandExpressives_MaterializesAndEvaluates()
    {
        var source = new List<EnumComparisonEntity>
        {
            new() { Value = Bucket.Low },
            new() { Value = Bucket.Mid },
            new() { Value = Bucket.High },
        }.AsQueryable();

        Expression<Func<EnumComparisonEntity, bool>> expr = e => e.IsBelowMid;
        var expanded = (Expression<Func<EnumComparisonEntity, bool>>)expr.ExpandExpressives();

        var results = source.Select(expanded.Compile()).ToList();

        CollectionAssert.AreEqual(new[] { true, false, false }, results);
    }

    [TestMethod]
    public void NullableEnumComparison_ExpandExpressives_MaterializesAndEvaluates()
    {
        var source = new List<EnumComparisonEntity>
        {
            new() { NullableValue = Bucket.Low },
            new() { NullableValue = Bucket.Mid },
            new() { NullableValue = Bucket.High },
            new() { NullableValue = null },
        }.AsQueryable();

        Expression<Func<EnumComparisonEntity, bool>> expr = e => e.IsLowOrMid;
        var expanded = (Expression<Func<EnumComparisonEntity, bool>>)expr.ExpandExpressives();

        var results = source.Select(expanded.Compile()).ToList();

        CollectionAssert.AreEqual(new[] { true, true, false, false }, results);
    }

    [TestMethod]
    public void ExpressivePropertyOnEnumComparison_RegistryResolves()
    {
        var registered = ExpressiveSharp.Generated.ExpressionRegistry.TryGet(
            typeof(EnumComparisonProperty).GetProperty(nameof(EnumComparisonProperty.Tier))!);

        Assert.IsNotNull(registered);
    }

    [TestMethod]
    public void ToStringOnNullableEnum_ExpandExpressives_MaterializesAndEvaluates()
    {
        var source = new List<EnumComparisonEntity>
        {
            new() { NullableValue = Bucket.Low },
            new() { NullableValue = Bucket.Mid },
            new() { NullableValue = Bucket.High },
            new() { NullableValue = null },
        }.AsQueryable();

        Expression<Func<EnumComparisonEntity, string>> expr = e => e.NullableLabel;
        var expanded = (Expression<Func<EnumComparisonEntity, string>>)expr.ExpandExpressives();

        var labels = source.Select(expanded.Compile()).ToList();

        CollectionAssert.AreEqual(new[] { "Low", "Mid", "High", "" }, labels);
    }
}

public class EnumComparisonEntity
{
    public Bucket Value { get; init; }
    public Bucket? NullableValue { get; init; }

    [Expressive]
    public string Tier =>
        Value <= Bucket.Low ? "low" :
        Value <= Bucket.Mid ? "mid" :
        "high";

    [Expressive]
    public bool IsBelowMid => Value < Bucket.Mid;

    [Expressive]
    public bool IsLowOrMid => NullableValue <= Bucket.Mid;

    [Expressive]
    public string NullableLabel => NullableValue.ToString() ?? string.Empty;
}

public enum Bucket { Low, Mid, High }

public partial class EnumComparisonProperty
{
    public Bucket Value { get; init; }

    [ExpressiveProperty("Tier")]
    private string TierExpr =>
        Value <= Bucket.Low ? "low" :
        Value <= Bucket.Mid ? "mid" :
        "high";
}
