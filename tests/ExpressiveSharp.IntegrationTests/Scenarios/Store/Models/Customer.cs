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

    // Block-body [Expressive] used by QueryFilterTestBase to exercise the
    // FlattenBlockExpressions transformer inside a global query filter.
    // A local variable assigned once + a single return expression is the
    // canonical shape the transformer inlines.
    [Expressive(AllowBlockBody = true)]
    public bool HasValidEmail()
    {
        var email = Email;
        return email != null && email.Length > 0;
    }
}
