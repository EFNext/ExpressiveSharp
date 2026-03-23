namespace ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

public class Customer
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }

    public int? AddressId { get; set; }
    public Address? Address { get; set; }

    [Expressive]
    public string? City => Address?.City;
}
