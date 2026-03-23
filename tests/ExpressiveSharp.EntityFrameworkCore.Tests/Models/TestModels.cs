namespace ExpressiveSharp.EntityFrameworkCore.Tests.Models;

public enum OrderStatus
{
    [System.ComponentModel.Description("Pending")]
    Pending,

    [System.ComponentModel.Description("Approved")]
    Approved,
}

public class Customer
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public OrderStatus Status { get; set; }
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    [Expressive]
    public double Total => Price * Quantity;

    [Expressive]
    public string? CustomerName => Customer?.Name;

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
}

public class OrderDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public double Total { get; set; }

    public OrderDto() { }

    [Expressive]
    public OrderDto(int id, string description, double total)
    {
        Id = id;
        Description = description;
        Total = total;
    }
}
