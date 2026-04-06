using Microsoft.EntityFrameworkCore.Query;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;

/// <summary>
/// Registers member access translators with EF Core's query pipeline.
/// Currently handles only <see cref="WindowFunctions.WindowFrameBound"/> property getters
/// (<c>UnboundedPreceding</c>, <c>CurrentRow</c>, <c>UnboundedFollowing</c>).
/// </summary>
internal sealed class WindowFunctionMemberTranslatorPlugin : IMemberTranslatorPlugin
{
    public IEnumerable<IMemberTranslator> Translators { get; } =
    [
        new WindowFrameBoundMemberTranslator()
    ];
}
