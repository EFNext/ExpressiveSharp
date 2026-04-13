namespace ExpressiveSharp.Docs.PlaygroundModel.Webshop;

public class Product : WebshopEntity
{
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public decimal ListPrice { get; set; }
    public int StockQuantity { get; set; }
}
