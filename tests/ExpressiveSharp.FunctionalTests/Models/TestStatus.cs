namespace ExpressiveSharp.FunctionalTests.Models;

public enum TestStatus
{
    [System.ComponentModel.Description("Awaiting processing")]
    Pending,

    [System.ComponentModel.Description("Order approved")]
    Approved,

    [System.ComponentModel.Description("Order rejected")]
    Rejected,
}

public static class TestStatusExtensions
{
    public static string GetDescription(this TestStatus value)
    {
        return value switch
        {
            TestStatus.Pending => "Awaiting processing",
            TestStatus.Approved => "Order approved",
            TestStatus.Rejected => "Order rejected",
            _ => value.ToString(),
        };
    }
}
