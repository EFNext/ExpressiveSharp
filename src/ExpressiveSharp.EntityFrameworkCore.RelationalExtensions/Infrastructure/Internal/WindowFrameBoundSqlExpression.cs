using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;

/// <summary>
/// Intermediate frame-boundary node produced by <see cref="WindowSpecMethodCallTranslator"/>
/// from <c>WindowFrameBound.*</c> members and consumed by the same translator at
/// <c>RowsBetween</c> / <c>RangeBetween</c>. Never reaches final SQL rendering.
/// </summary>
internal sealed class WindowFrameBoundSqlExpression : SqlExpression
{
    public WindowFrameBoundInfo BoundInfo { get; }

    public WindowFrameBoundSqlExpression(WindowFrameBoundInfo boundInfo)
        : base(typeof(object), typeMapping: null)
    {
        BoundInfo = boundInfo;
    }

    protected override Expression VisitChildren(ExpressionVisitor visitor) => this;

    protected override void Print(ExpressionPrinter expressionPrinter) =>
        expressionPrinter.Append($"WindowFrameBound({BoundInfo.ToSqlFragment()})");

#if NET9_0_OR_GREATER
    public override Expression Quote() =>
        throw new InvalidOperationException("WindowFrameBoundSqlExpression is an intermediate node and should not be quoted.");
#endif

    public override bool Equals(object? obj) =>
        obj is WindowFrameBoundSqlExpression other && base.Equals(other) && BoundInfo == other.BoundInfo;

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), BoundInfo);
}
