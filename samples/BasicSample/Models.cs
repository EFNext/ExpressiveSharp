using System.ComponentModel;
using System.Linq.Expressions;
using ExpressiveSharp;

// ── Enums ────────────────────────────────────────────────────────────────────

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
    public static string GetDescription(this OrderStatus value)
    {
        return value switch
        {
            OrderStatus.Pending => "Awaiting processing",
            OrderStatus.Approved => "Order approved",
            OrderStatus.Rejected => "Order rejected",
            _ => value.ToString(),
        };
    }
}

// ── Domain models ────────────────────────────────────────────────────────────

public class Customer
{
    public string? Name { get; set; }
    public string? Email { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public string? Tag { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public OrderStatus Status { get; set; }
    public Customer? Customer { get; set; }

    // ── [Expressive] properties ──────────────────────────────────────────

    /// Basic computed property — generates a companion expression tree.
    [Expressive]
    public double Total => Price * Quantity;

    /// Null-conditional: Customer?.Name → (Customer != null ? Customer.Name : null)
    [Expressive]
    public string? CustomerName => Customer?.Name;

    /// Same expression, but without the null-check pattern.
    /// When consumed by a LINQ provider that registers RemoveNullConditionalPatterns globally
    /// (e.g. EF Core via UseExpressives()), the null check is stripped: Customer?.Name → Customer.Name
    [Expressive]
    public string? CustomerNameUnsafe => Customer?.Name;

    /// Enum method expansion: expands GetDescription() into a ternary chain per enum value.
    [Expressive]
    public string StatusDescription => Status.GetDescription();

    // ── [Expressive] methods ─────────────────────────────────────────────

    /// Switch expression with relational patterns.
    [Expressive]
    public string GetGrade() => Price switch
    {
        >= 100 => "Premium",
        >= 50 => "Standard",
        _ => "Budget",
    };

    /// Block body with if/else and local variables.
    [Expressive(AllowBlockBody = true)]
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

// ── Constructor projection ───────────────────────────────────────────────────

public class OrderDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public double Total { get; set; }

    public OrderDto() { }

    /// Projectable constructor — generates a MemberInit expression tree.
    [Expressive]
    public OrderDto(int id, string description, double total)
    {
        Id = id;
        Description = description;
        Total = total;
    }
}

// ── Shape hierarchy for type-pattern switch demo ─────────────────────────────

public abstract class Shape
{
    public string Name { get; set; } = "";
}

public class Circle : Shape
{
    public double Radius { get; set; }
}

public class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }
}

public static class ShapeExtensions
{
    /// Extension method with type-pattern switch expression.
    [Expressive]
    public static double GetArea(this Shape shape) => shape switch
    {
        Circle c => Math.PI * c.Radius * c.Radius,
        Rectangle r => r.Width * r.Height,
        _ => 0,
    };
}
