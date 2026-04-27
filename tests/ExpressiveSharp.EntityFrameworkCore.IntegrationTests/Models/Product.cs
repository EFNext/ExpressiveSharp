namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Models;

// Used for ExecuteUpdate tests; isolated from the shared Order/Customer graph
// so bulk-update tests can mutate freely.
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Category { get; set; }
    public string Tag { get; set; } = "";
    public double Price { get; set; }
    public int Quantity { get; set; }
}
