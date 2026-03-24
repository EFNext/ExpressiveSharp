using System.Linq.Expressions;
using ExpressiveSharp.Services;

namespace ExpressiveSharp.Tests.Services;

[TestClass]
public class ExpressiveDefaultsTests
{
    [TestCleanup]
    public void Cleanup()
    {
        ExpressiveDefaults.ClearTransformers();
    }

    [TestMethod]
    public void AddTransformers_RegistersTransformers()
    {
        var transformer = new NoOpTransformer();
        ExpressiveDefaults.AddTransformers(transformer);

        var result = ExpressiveDefaults.GetTransformers();

        Assert.AreEqual(1, result.Count);
        Assert.AreSame(transformer, result[0]);
    }

    [TestMethod]
    public void ClearTransformers_RemovesAll()
    {
        ExpressiveDefaults.AddTransformers(new NoOpTransformer(), new NoOpTransformer());
        ExpressiveDefaults.ClearTransformers();

        var result = ExpressiveDefaults.GetTransformers();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetTransformers_ReturnsSnapshot_NotLiveReference()
    {
        ExpressiveDefaults.AddTransformers(new NoOpTransformer());
        var snapshot = ExpressiveDefaults.GetTransformers();

        ExpressiveDefaults.AddTransformers(new NoOpTransformer());
        var updated = ExpressiveDefaults.GetTransformers();

        Assert.AreEqual(1, snapshot.Count);
        Assert.AreEqual(2, updated.Count);
    }

    [TestMethod]
    public void AddAndClear_ConcurrentAccess_NoExceptions()
    {
        var tasks = new Task[20];
        for (var i = 0; i < tasks.Length; i++)
        {
            tasks[i] = i % 2 == 0
                ? Task.Run(() => ExpressiveDefaults.AddTransformers(new NoOpTransformer()))
                : Task.Run(() => ExpressiveDefaults.ClearTransformers());
        }

        Task.WaitAll(tasks);

        // No exceptions thrown — thread safety verified
        ExpressiveDefaults.ClearTransformers();
        Assert.AreEqual(0, ExpressiveDefaults.GetTransformers().Count);
    }

    private class NoOpTransformer : IExpressionTreeTransformer
    {
        public Expression Transform(Expression expression) => expression;
    }
}
