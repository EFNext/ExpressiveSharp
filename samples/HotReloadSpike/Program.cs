using System.Linq.Expressions;
using ExpressiveSharp;

Console.WriteLine("HotReload spike — edit Models.cs while running and watch output.");
Console.WriteLine("Press Ctrl+C to exit.\n");

var i = 0;
while (true)
{
    var orders = new List<Order>
    {
        new() { Id = 1, Tag = "urgent", Price = 120.0, Quantity = 2 },
        new() { Id = 2, Tag = "bulk",   Price =   8.0, Quantity = 50 },
        new() { Id = 3, Tag = null,     Price = 2000.0, Quantity = 1 },
    };
    Console.WriteLine($"--- tick {i++} @ {DateTime.Now:HH:mm:ss} ---");

    Expression<Func<Order, double>> totalExpr = o => o.Total;
    Console.WriteLine($"  Total     expanded: {totalExpr.ExpandExpressives()}");

    Expression<Func<Order, string>> gradeExpr = o => o.GetGrade();
    Console.WriteLine($"  GetGrade  expanded: {gradeExpr.ExpandExpressives()}");

    foreach (var o in orders)
        Console.WriteLine($"    #{o.Id}  Total={o.Total,6:F2}  Grade={o.GetGrade()}");

    Console.WriteLine();
    await Task.Delay(2000);
}
