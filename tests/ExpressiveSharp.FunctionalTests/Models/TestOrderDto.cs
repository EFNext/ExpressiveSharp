namespace ExpressiveSharp.FunctionalTests.Models;

public class TestOrderDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public double Total { get; set; }

    public TestOrderDto() { }

    [Expressive]
    public TestOrderDto(int id, string description, double total)
    {
        Id = id;
        Description = description;
        Total = total;
    }
}
