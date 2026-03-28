namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Tests.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class Order
{
    public int Id { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
}
