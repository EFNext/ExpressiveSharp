using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.IntegrationTests.Infrastructure;

public abstract class IntegrationTestBase
{
    protected IIntegrationTestRunner Runner { get; private set; } = null!;

    protected abstract IIntegrationTestRunner CreateRunner();

    [TestInitialize]
    public void InitRunner() => Runner = CreateRunner();

    [TestCleanup]
    public async Task CleanupRunner() => await Runner.DisposeAsync();
}
