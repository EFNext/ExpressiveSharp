using System.Reflection;

namespace ExpressiveSharp.Docs.Playground.Core.Services.Scenarios;

public interface IPlaygroundScenario
{
    string Id { get; }
    string Title { get; }
    string DefaultSnippet { get; }
    string? DefaultSetup { get; }
    string WrapperTemplate { get; }
    IReadOnlyList<Assembly> ReferenceAssemblies { get; }
    IReadOnlyList<ScenarioRenderTarget> RenderTargets { get; }
    IScenarioInstance CreateInstance();
}
