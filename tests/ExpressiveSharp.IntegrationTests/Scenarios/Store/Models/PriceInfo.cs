namespace ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

/// <summary>
/// Record used by the C# language feature tests to exercise <c>with</c>
/// expressions in lambdas that get compiled to expression trees.
/// </summary>
public record PriceInfo(double BasePrice, double Multiplier)
{
    public double Final => BasePrice * Multiplier;
}
