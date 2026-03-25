using ExpressiveSharp;

// ── Domain models ────────────────────────────────────────────────────────────

public enum OrderStatus { Pending, Approved, Rejected }

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

    /// Computed total — usable in LINQ-to-SQL queries, not just in-memory code.
    [Expressive]
    public double Total => Price * Quantity;

    /// Null-conditional access — normally illegal in expression trees (CS8072).
    [Expressive]
    public string? CustomerName => Customer?.Name;

    /// Switch expression with relational patterns — translates to CASE WHEN in SQL.
    [Expressive]
    public string GetGrade() => Price switch
    {
        >= 100 => "Premium",
        >= 50 => "Standard",
        _ => "Budget",
    };

    /// Block body with local variables and if/else — requires AllowBlockBody opt-in.
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

// ── DTO with projectable constructor ─────────────────────────────────────────

public class OrderDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public double Total { get; set; }

    public OrderDto() { }

    /// Projectable constructor — expands to MemberInit so LINQ providers
    /// can translate the projection to SQL instead of loading entire entities.
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
    /// Extension method with type-pattern switch — works with [Expressive] too.
    [Expressive]
    public static double GetArea(this Shape shape) => shape switch
    {
        Circle c => Math.PI * c.Radius * c.Radius,
        Rectangle r => r.Width * r.Height,
        _ => 0,
    };
}
