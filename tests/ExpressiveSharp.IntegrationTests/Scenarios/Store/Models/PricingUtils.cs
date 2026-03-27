using ExpressiveSharp.Mapping;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

/// <summary>
/// A utility class representing an external/third-party library whose methods
/// cannot normally be used in EF Core LINQ queries because EF Core has no
/// built-in SQL translation for them.
///
/// With [ExpressiveFor], we provide expression-tree equivalents that EF Core CAN translate.
/// </summary>
public static class PricingUtils
{
    /// <summary>Clamps a value between min and max.</summary>
    public static double Clamp(double value, double min, double max)
        => Math.Max(min, Math.Min(max, value));

    /// <summary>Applies a percentage discount to a price.</summary>
    public static double ApplyDiscount(double price, double discountPercent)
        => price * (1 - discountPercent / 100.0);
}

/// <summary>
/// Provides expression-tree bodies for <see cref="PricingUtils"/> methods,
/// enabling them to be used in EF Core queries via <c>ExpandExpressives()</c>.
/// </summary>
static class PricingUtilsMappings
{
    [ExpressiveFor(typeof(PricingUtils), nameof(PricingUtils.Clamp))]
    static double Clamp(double value, double min, double max)
        => value < min ? min : (value > max ? max : value);

    [ExpressiveFor(typeof(PricingUtils), nameof(PricingUtils.ApplyDiscount))]
    static double ApplyDiscount(double price, double discountPercent)
        => price * (1.0 - discountPercent / 100.0);
}
