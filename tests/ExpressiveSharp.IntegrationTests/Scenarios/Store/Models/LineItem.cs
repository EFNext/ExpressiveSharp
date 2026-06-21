namespace ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

public class LineItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string ProductName { get; set; } = "";
    public double UnitPrice { get; set; }
    public int Quantity { get; set; }

    // Virtual [Expressive] member with no derived overrides — regression coverage that expansion
    // still reaches the query provider as translatable SQL. With no overrides anywhere the
    // polymorphic plan is trivial, so this expands exactly like a non-virtual member (no type-test).
    [Expressive]
    public virtual bool IsExpensive => UnitPrice > 40;
}
