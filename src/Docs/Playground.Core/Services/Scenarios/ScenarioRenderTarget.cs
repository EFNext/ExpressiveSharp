namespace ExpressiveSharp.Docs.Playground.Core.Services.Scenarios;

public sealed record ScenarioRenderTarget(
    string Id,
    string Label,
    string OutputLanguage,
    Func<System.Linq.IQueryable, IScenarioInstance, string> Render)
{
    public Func<IScenarioInstance, object>? GetQueryArgument { get; init; }
    public IReadOnlyList<string>? LazyLoadAssemblies { get; init; }
}
