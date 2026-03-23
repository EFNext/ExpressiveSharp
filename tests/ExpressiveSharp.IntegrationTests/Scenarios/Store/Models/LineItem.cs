namespace ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

public class LineItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string ProductName { get; set; } = "";
    public double UnitPrice { get; set; }
    public int Quantity { get; set; }
}
