using ExpressiveSharp.Services;
using ExpressiveSharp.Transformers;

namespace ExpressiveSharp.MongoDB;

/// <summary>
/// Factory for creating an <see cref="ExpressiveOptions"/> pre-configured with
/// transformers suitable for MongoDB's LINQ provider.
/// </summary>
public static class MongoExpressiveOptions
{
    /// <summary>
    /// Creates an <see cref="ExpressiveOptions"/> with the default transformer pipeline
    /// for MongoDB queries. Matches the transformers used by the EF Core integration.
    /// </summary>
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
