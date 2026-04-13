namespace ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

public sealed record OrderSummaryRecord(int Id, double Total)
{
    [Expressive]
    public OrderSummaryRecord(Order o) : this(o.Id, o.Price * o.Quantity) { }
}

public sealed class OrderSummaryDeconstructed
{
    public int Id { get; set; }
    public double Total { get; set; }

    public OrderSummaryDeconstructed() { }

    [Expressive]
    public OrderSummaryDeconstructed(Order o) =>
        (Id, Total) = (o.Id, o.Price * o.Quantity);
}
