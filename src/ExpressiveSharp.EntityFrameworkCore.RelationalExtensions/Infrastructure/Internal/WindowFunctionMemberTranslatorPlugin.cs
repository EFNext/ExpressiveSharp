using Microsoft.EntityFrameworkCore.Query;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;

internal sealed class WindowFunctionMemberTranslatorPlugin : IMemberTranslatorPlugin
{
    public IEnumerable<IMemberTranslator> Translators { get; } =
    [
        new WindowFrameBoundMemberTranslator()
    ];
}
