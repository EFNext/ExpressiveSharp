namespace ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

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
