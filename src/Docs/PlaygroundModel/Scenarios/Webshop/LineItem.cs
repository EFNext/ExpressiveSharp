namespace ExpressiveSharp.Docs.PlaygroundModel.Webshop;

public class LineItem : WebshopEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
