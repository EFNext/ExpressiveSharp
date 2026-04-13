namespace ExpressiveSharp.Docs.Playground.Core.Services.Scenarios;

public interface IScenarioInstance : IAsyncDisposable
{
    object QueryArgument { get; }
}
