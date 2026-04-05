namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Models;

/// <summary>
/// Entity used for ExecuteUpdate integration tests. Distinct from the shared
/// Order/Customer graph because bulk update tests need their own data set that
/// can be mutated freely per test without affecting other scenarios.
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Category { get; set; }
    public string Tag { get; set; } = "";
    public double Price { get; set; }
    public int Quantity { get; set; }
}
