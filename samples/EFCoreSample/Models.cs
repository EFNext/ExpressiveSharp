using System.ComponentModel;
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
    public static string GetDescription(this OrderStatus value) => value switch
    {
        OrderStatus.Pending => "Awaiting processing",
        OrderStatus.Approved => "Order approved",
        OrderStatus.Rejected => "Order rejected",
        _ => value.ToString(),
    };
}

// ── Domain models ────────────────────────────────────────────────────────────

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

    /// Price * Quantity — becomes a SQL expression, not a client-side property.
    [Expressive]
    public double Total => Price * Quantity;

    /// Null-conditional navigation — UseExpressives() strips the null check
    /// automatically so EF Core can translate it to a JOIN.
    [Expressive]
    public string? CustomerEmail => Customer?.Email;

    /// Enum method expansion — each enum value is evaluated at compile time,
    /// producing a CASE WHEN chain that translates directly to SQL.
    [Expressive]
    public string StatusDescription => Status.GetDescription();

    /// Switch expression with relational patterns — becomes SQL CASE WHEN.
    [Expressive]
    public string GetGrade() => Price switch
    {
        >= 100 => "Premium",
        >= 50 => "Standard",
        _ => "Budget",
    };

    /// Block body with local variable + if/else — requires AllowBlockBody opt-in.
    /// The FlattenBlockExpressions transformer inlines locals for SQL translation.
    [Expressive(AllowBlockBody = true)]
    public string GetCategory()
    {
        var threshold = Quantity * 10;
        if (threshold > 100)
            return "Bulk";
        else
            return "Regular";
    }
}

// ── DTO for constructor projection ───────────────────────────────────────────

public class OrderSummaryDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public double Total { get; set; }

    public OrderSummaryDto() { }

    /// Expands to MemberInit so EF Core translates this projection to a clean
    /// SQL SELECT instead of loading entire Order entities.
    [Expressive]
    public OrderSummaryDto(int id, string description, double total)
    {
        Id = id;
        Description = description;
        Total = total;
    }
}
