namespace ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

public class Order
{
    public int Id { get; set; }
    public string? Tag { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public OrderStatus Status { get; set; }

    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public List<LineItem> Items { get; set; } = new();

    [Expressive]
    public double Total => Price * Quantity;

    // Multi-level null-conditional chain: Order → Customer → Address → Country
    [Expressive]
    public string? CustomerCountry => Customer?.Address?.Country;

    // Tuple projection
    [Expressive]
    public (int Id, string Grade, double Total) GetOrderSummaryTuple()
        => (Id, GetGrade(), Total);

    [Expressive]
    public string? CustomerName => Customer?.Name;

    [Expressive]
    public int? TagLength => Tag?.Length;

    [Expressive]
    public string StatusDescription => Status.GetDescription();

    [Expressive]
    public string GetGrade() => Price switch
    {
        >= 100 => "Premium",
        >= 50 => "Standard",
        _ => "Budget",
    };

    [Expressive]
    public string GetCategory()
    {
        var threshold = Quantity * 10;
        if (threshold > 100)
        {
            return "Bulk";
        }
        else
        {
            return "Regular";
        }
    }

    [Expressive]
    public string Summary => $"Order #{Id}: {Tag ?? "N/A"}";
}
