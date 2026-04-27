namespace ExpressiveSharp.Docs.PlaygroundModel.Webshop;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Email { get; set; }
    public string? Country { get; set; }
    public DateTime JoinedAt { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
