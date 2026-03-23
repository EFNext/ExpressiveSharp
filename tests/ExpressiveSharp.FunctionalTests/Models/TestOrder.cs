namespace ExpressiveSharp.FunctionalTests.Models;

public class TestOrder
{
    public int Id { get; set; }
    public string? Tag { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public TestStatus Status { get; set; }

    public int? CustomerId { get; set; }
    public TestCustomer? Customer { get; set; }

    [Expressive]
    public double Total => Price * Quantity;

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
