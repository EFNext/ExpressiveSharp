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

    [Expressive]
    public string Summary => $"Order #{Id}: {Tag ?? "N/A"}";

    // String concatenation via + operator (tests Expression.Call(string.Concat) emission)
    [Expressive]
    public string SummaryConcat => "Order #" + Id + ": " + (Tag ?? "N/A");

    // Format specifier: works in-memory but not translatable to SQL
    [Expressive]
    public string FormattedPrice => $"{Price:F2}";

    // 5+ interpolation parts: tests string.Concat(string[]) overload
    [Expressive]
    public string DetailedSummary => $"Order #{Id}: {Tag ?? "N/A"} (${Price})";

    // Loop-based computed members (foreach → Expression.Loop)
    [Expressive(AllowBlockBody = true)]
    public int ItemCount()
    {
        var count = 0;
        foreach (var item in Items) { count++; }
        return count;
    }

    [Expressive(AllowBlockBody = true)]
    public double ItemTotal()
    {
        var total = 0.0;
        foreach (var item in Items) { total += item.UnitPrice * item.Quantity; }
        return total;
    }

    [Expressive(AllowBlockBody = true)]
    public bool HasExpensiveItems()
    {
        var found = false;
        foreach (var item in Items) { if (item.UnitPrice > 40) found = true; }
        return found;
    }

    [Expressive(AllowBlockBody = true)]
    public bool AllItemsAffordable()
    {
        var all = true;
        foreach (var item in Items) { if (!(item.UnitPrice <= 100)) all = false; }
        return all;
    }

    [Expressive(AllowBlockBody = true)]
    public double ItemTotalForExpensive()
    {
        var total = 0.0;
        foreach (var item in Items)
        {
            if (item.UnitPrice > 40) { total += item.UnitPrice * item.Quantity; }
        }
        return total;
    }

    // Collection expression: array literal
    [Expressive]
    public int[] PriceBreakpoints => [10, 50, 100];

    // Tuple equality
    [Expressive]
    public bool IsPriceQuantityMatch => (Price, Quantity) == (50.0, 5);

    // Tuple inequality
    [Expressive]
    public bool IsPriceQuantityDifferent => (Price, Quantity) != (50.0, 5);

    // Checked arithmetic
    [Expressive]
    public double CheckedTotal => checked(Price * Quantity);
}
