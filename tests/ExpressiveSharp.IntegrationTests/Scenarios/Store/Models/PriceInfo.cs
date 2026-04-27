namespace ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

public record PriceInfo(double BasePrice, double Multiplier)
{
    public double Final => BasePrice * Multiplier;
}
