namespace ExpressiveSharp.IntegrationTests.Scenarios.Store.Models;

public enum OrderStatus
{
    [System.ComponentModel.Description("Awaiting processing")]
    Pending,

    [System.ComponentModel.Description("Order approved")]
    Approved,

    [System.ComponentModel.Description("Order rejected")]
    Rejected,
}

public static class OrderStatusExtensions
{
    public static string GetDescription(this OrderStatus value)
    {
        return value switch
        {
            OrderStatus.Pending => "Awaiting processing",
            OrderStatus.Approved => "Order approved",
            OrderStatus.Rejected => "Order rejected",
            _ => value.ToString(),
        };
    }
}
