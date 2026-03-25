namespace ExpressiveSharp.Benchmarks.Helpers;

public class TestEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public decimal Amount { get; set; }
    public decimal TaxRate { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsEnabled { get; set; }
    public int Priority { get; set; }

    [Expressive]
    public int IdPlus1 => Id + 1;

    [Expressive]
    public string FullName => $"{FirstName} {LastName}";

    [Expressive]
    public int IdPlus1Method() => Id + 1;

    [Expressive]
    public int IdPlusDelta(int delta) => Id + delta;

    [Expressive]
    public int? EmailLength => Email?.Length;

    [Expressive(AllowBlockBody = true)]
    public string GetCategory()
    {
        if (DeletedAt != null) return "Deleted";
        if (IsEnabled) return "Active";
        return "Inactive";
    }

    [Expressive]
    public TestEntity(TestEntity other)
    {
        Id = other.Id;
        FirstName = other.FirstName;
        LastName = other.LastName;
    }

    public TestEntity() { }
}
