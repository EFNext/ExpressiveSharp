namespace ExpressiveSharp.Docs.PlaygroundModel.Webshop;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public DateTime PlacedAt { get; set; }
    public OrderStatus Status { get; set; }
    public ICollection<LineItem> Items { get; set; } = new List<LineItem>();
}
