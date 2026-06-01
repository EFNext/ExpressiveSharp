namespace ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

public class LineItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string ProductName { get; set; } = "";
    public double UnitPrice { get; set; }
    public int Quantity { get; set; }

    // Virtual [Expressive] member — regression coverage that static-type expansion still reaches
    // the query provider as translatable SQL. The reverted "bad commit" gate skipped expansion for
    // virtual members, so this would hit EF Core untranslated and throw. EXP0038 is expected here
    // by design and suppressed.
#pragma warning disable EXP0038
    [Expressive]
    public virtual bool IsExpensive => UnitPrice > 40;
#pragma warning restore EXP0038
}
