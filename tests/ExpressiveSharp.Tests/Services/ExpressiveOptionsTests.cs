using System.Linq.Expressions;
using ExpressiveSharp.Services;

namespace ExpressiveSharp.Tests.Services;

[TestClass]
public class ExpressiveOptionsTests
{
    [TestMethod]
    public void AddTransformers_RegistersTransformers()
    {
        var options = new ExpressiveOptions();
        var transformer = new NoOpTransformer();
        options.AddTransformers(transformer);

        var result = options.GetTransformers();

        Assert.AreEqual(1, result.Count);
        Assert.AreSame(transformer, result[0]);
    }

    [TestMethod]
    public void ClearTransformers_RemovesAll()
    {
        var options = new ExpressiveOptions();
        options.AddTransformers(new NoOpTransformer(), new NoOpTransformer());
        options.ClearTransformers();

        var result = options.GetTransformers();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetTransformers_ReturnsSnapshot_NotLiveReference()
    {
        var options = new ExpressiveOptions();
        options.AddTransformers(new NoOpTransformer());
        var snapshot = options.GetTransformers();

        options.AddTransformers(new NoOpTransformer());
        var updated = options.GetTransformers();

        Assert.AreEqual(1, snapshot.Count);
        Assert.AreEqual(2, updated.Count);
    }

    [TestMethod]
    public void AddAndClear_ConcurrentAccess_NoExceptions()
    {
        var options = new ExpressiveOptions();
        var tasks = new Task[20];
        for (var i = 0; i < tasks.Length; i++)
        {
            tasks[i] = i % 2 == 0
                ? Task.Run(() => options.AddTransformers(new NoOpTransformer()))
                : Task.Run(() => options.ClearTransformers());
        }

        Task.WaitAll(tasks);

        // No exceptions thrown — thread safety verified
        options.ClearTransformers();
        Assert.AreEqual(0, options.GetTransformers().Count);
    }

    private class NoOpTransformer : IExpressionTreeTransformer
    {
        public Expression Transform(Expression expression) => expression;
    }
}
