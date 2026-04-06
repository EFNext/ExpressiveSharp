using System.Reflection;
using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;

/// <summary>
/// Translates the no-argument <see cref="WindowFrameBound"/> static property getters
/// (<c>UnboundedPreceding</c>, <c>CurrentRow</c>, <c>UnboundedFollowing</c>) into
/// <see cref="WindowFrameBoundSqlExpression"/> intermediate nodes, which are then
/// consumed by <see cref="WindowSpecMethodCallTranslator"/> when it sees
/// <c>RowsBetween</c> / <c>RangeBetween</c>.
/// <para>
/// The offset-bearing variants (<c>Preceding(int)</c>, <c>Following(int)</c>) are
/// methods and are handled by <see cref="WindowSpecMethodCallTranslator"/>.
/// </para>
/// </summary>
internal sealed class WindowFrameBoundMemberTranslator : IMemberTranslator
{
    public SqlExpression? Translate(
        SqlExpression? instance,
        MemberInfo member,
        Type returnType,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        if (member.DeclaringType != typeof(WindowFrameBound))
            return null;

        return member.Name switch
        {
            nameof(WindowFrameBound.UnboundedPreceding) =>
                new WindowFrameBoundSqlExpression(new WindowFrameBoundInfo(WindowFrameBoundKind.UnboundedPreceding, null)),
            nameof(WindowFrameBound.CurrentRow) =>
                new WindowFrameBoundSqlExpression(new WindowFrameBoundInfo(WindowFrameBoundKind.CurrentRow, null)),
            nameof(WindowFrameBound.UnboundedFollowing) =>
                new WindowFrameBoundSqlExpression(new WindowFrameBoundInfo(WindowFrameBoundKind.UnboundedFollowing, null)),
            _ => null
        };
    }
}
