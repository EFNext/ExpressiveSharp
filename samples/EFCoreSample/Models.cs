using System.ComponentModel;
using ExpressiveSharp;

public enum OrderStatus
{
    [Description("Awaiting processing")]
    Pending,

    [Description("Order approved")]
    Approved,

    [Description("Order rejected")]
    Rejected,
}

public static class OrderStatusExtensions
{
    [Expressive]
    public static string GetDescription(this OrderStatus value) => value switch
    {
        OrderStatus.Pending => "Awaiting processing",
        OrderStatus.Approved => "Order approved",
        OrderStatus.Rejected => "Order rejected",
        _ => value.ToString(),
    };
}

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
    public OrderStatus Status { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    [Expressive]
    public double Total => Price * Quantity;

    [Expressive]
    public string StatusDescription => Status.GetDescription();

    [Expressive]
    public string Grade => Price switch
    {
        >= 100 => "Premium",
        >= 50 => "Standard",
        _ => "Budget",
    };
}

public record OrderSummaryDto(int OrderId, string CustomerName, double Total, string Grade, string Status);
