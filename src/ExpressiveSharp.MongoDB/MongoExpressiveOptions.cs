using ExpressiveSharp.Services;
using ExpressiveSharp.Transformers;

namespace ExpressiveSharp.MongoDB;

/// <summary>
/// Factory for an <see cref="ExpressiveOptions"/> pre-configured with transformers
/// suitable for MongoDB's LINQ provider.
/// </summary>
public static class MongoExpressiveOptions
{
    public static ExpressiveOptions CreateDefault()
    {
        var options = new ExpressiveOptions();
        options.AddTransformers(
            new ReplaceThrowWithDefault(),
            new ConvertLoopsToLinq(),
            new RemoveNullConditionalPatterns(),
            new FlattenTupleComparisons(),
            new FlattenConcatArrayCalls(),
            new FlattenBlockExpressions());
        return options;
    }
}
