using ExpressiveSharp;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Email { get; set; }
    public List<Order> Orders { get; set; } = [];
}

public class Order
{
    public int Id { get; set; }
    public string? Tag { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    [Expressive]
    public double Total => Price * Quantity;

    [Expressive]
    public string? CustomerEmail => Customer?.Email;

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
            return "Bulk";
        else
            return "Regular";
    }
}

public class OrderSummaryDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public double Total { get; set; }

    public OrderSummaryDto() { }

    [Expressive]
    public OrderSummaryDto(int id, string description, double total)
    {
        Id = id;
        Description = description;
        Total = total;
    }
}
