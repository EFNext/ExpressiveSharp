namespace ExpressiveSharp.Docs.Playground.Core.Services.Scenarios;

public static class ScenarioRegistry
{
    public static readonly IPlaygroundScenario Webshop = new WebshopScenario();

    public static readonly IReadOnlyList<IPlaygroundScenario> All = new[] { Webshop };

    public static IPlaygroundScenario Default => Webshop;

    public static IPlaygroundScenario Resolve(string? id)
    {
        if (string.IsNullOrEmpty(id)) return Default;
        foreach (var scenario in All)
        {
            if (scenario.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                return scenario;
        }
        return Default;
    }
}
