using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;

/// <summary>
/// Registers window function method call translators with EF Core's query pipeline.
/// </summary>
internal sealed class WindowFunctionTranslatorPlugin : IMethodCallTranslatorPlugin
{
    public IEnumerable<IMethodCallTranslator> Translators { get; }

    public WindowFunctionTranslatorPlugin(
        ISqlExpressionFactory sqlExpressionFactory,
        IRelationalTypeMappingSource typeMappingSource)
    {
        Translators =
        [
            new WindowSpecMethodCallTranslator(),
            new WindowFunctionMethodCallTranslator(sqlExpressionFactory, typeMappingSource)
        ];
    }
}
