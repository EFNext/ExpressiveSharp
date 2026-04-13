// Plain data only — no [Expressive] members. Samples that demonstrate
// computed members do so with their own inline [Expressive] extension methods
// declared in the snippet's `setup` attribute, so the docs reader sees the
// definition next to the query that uses it.

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
