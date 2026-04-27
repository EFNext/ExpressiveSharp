using ExpressiveSharp.Mapping;

namespace ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

// External/third-party utility class with no built-in EF Core SQL translation;
// [ExpressiveFor] mappings below supply translatable expression bodies.
public static class PricingUtils
{
    public static double Clamp(double value, double min, double max)
        => Math.Max(min, Math.Min(max, value));

    public static double ApplyDiscount(double price, double discountPercent)
        => price * (1 - discountPercent / 100.0);
}

static class PricingUtilsMappings
{
    [ExpressiveFor(typeof(PricingUtils), nameof(PricingUtils.Clamp))]
    static double Clamp(double value, double min, double max)
        => value < min ? min : (value > max ? max : value);

    [ExpressiveFor(typeof(PricingUtils), nameof(PricingUtils.ApplyDiscount))]
    static double ApplyDiscount(double price, double discountPercent)
        => price * (1.0 - discountPercent / 100.0);
}
