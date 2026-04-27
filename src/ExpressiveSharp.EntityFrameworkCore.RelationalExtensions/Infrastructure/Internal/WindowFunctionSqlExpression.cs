using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;

/// <summary>
/// SQL expression for <c>FUNC(args) OVER(PARTITION BY ... ORDER BY ... [frame])</c>. Used for
/// RANK, DENSE_RANK, NTILE, aggregates, and navigation functions (ROW_NUMBER uses the built-in
/// <see cref="RowNumberExpression"/>).
/// <para>
/// Self-rendering: <see cref="VisitChildren"/> interleaves <see cref="SqlFragmentExpression"/>
/// nodes with column/ordering expressions, producing correct SQL through any provider's
/// <see cref="QuerySqlGenerator"/> — no custom generator replacement needed.
/// </para>
/// <para>
/// Function names and clause syntax are hardcoded as literal SQL fragments, relying on SQL:2003
/// window function syntax (SQL Server 2012+, PostgreSQL 8.4+, SQLite 3.25+, MySQL 8.0+,
/// Oracle 8i+, MariaDB 10.2+). A provider that deviates would need a custom implementation.
/// </para>
/// </summary>
internal sealed class WindowFunctionSqlExpression : SqlExpression
{
    public string FunctionName { get; }
    public IReadOnlyList<SqlExpression> Arguments { get; }
    public IReadOnlyList<SqlExpression> Partitions { get; }
    public IReadOnlyList<OrderingExpression> Orderings { get; }
    public WindowFrameType? FrameType { get; }
    public WindowFrameBoundInfo? FrameStart { get; }
    public WindowFrameBoundInfo? FrameEnd { get; }

    public WindowFunctionSqlExpression(
        string functionName,
        IReadOnlyList<SqlExpression> arguments,
        IReadOnlyList<SqlExpression> partitions,
        IReadOnlyList<OrderingExpression> orderings,
        Type type,
        RelationalTypeMapping? typeMapping,
        WindowFrameType? frameType = null,
        WindowFrameBoundInfo? frameStart = null,
        WindowFrameBoundInfo? frameEnd = null)
        : base(type, typeMapping)
    {
        FunctionName = functionName;
        Arguments = arguments;
        Partitions = partitions;
        Orderings = orderings;
        FrameType = frameType;
        FrameStart = frameStart;
        FrameEnd = frameEnd;
    }

    protected override Expression VisitChildren(ExpressionVisitor visitor)
    {
        EmitWindowFunction(
            text => visitor.Visit(new SqlFragmentExpression(text)),
            expr => visitor.Visit(expr));
        return this;
    }

    protected override void Print(ExpressionPrinter expressionPrinter) =>
        EmitWindowFunction(
            text => expressionPrinter.Append(text),
            expr => expressionPrinter.Visit(expr));

    private void EmitWindowFunction(Action<string> appendText, Action<Expression> visitExpression)
    {
        appendText($"{FunctionName}(");
        for (var i = 0; i < Arguments.Count; i++)
        {
            if (i > 0) appendText(", ");
            visitExpression(Arguments[i]);
        }
        appendText(") OVER(");

        var anyClauseEmitted = false;

        if (Partitions.Count > 0)
        {
            appendText("PARTITION BY ");
            for (var i = 0; i < Partitions.Count; i++)
            {
                if (i > 0) appendText(", ");
                visitExpression(Partitions[i]);
            }
            anyClauseEmitted = true;
        }

        if (Orderings.Count > 0)
        {
            if (anyClauseEmitted) appendText(" ");
            appendText("ORDER BY ");
            for (var i = 0; i < Orderings.Count; i++)
            {
                if (i > 0) appendText(", ");
                visitExpression(Orderings[i].Expression);
                appendText(Orderings[i].IsAscending ? " ASC" : " DESC");
            }
            anyClauseEmitted = true;
        }

        if (FrameType is { } frameType)
        {
            if (anyClauseEmitted) appendText(" ");
            appendText(frameType == WindowFrameType.Rows ? "ROWS BETWEEN " : "RANGE BETWEEN ");
            appendText(FrameStart!.Value.ToSqlFragment());
            appendText(" AND ");
            appendText(FrameEnd!.Value.ToSqlFragment());
        }

        appendText(")");
    }

#if NET9_0_OR_GREATER
    public override Expression Quote() =>
        throw new InvalidOperationException("WindowFunctionSqlExpression quoting is not supported.");
#endif

    public override bool Equals(object? obj) =>
        obj is WindowFunctionSqlExpression other
        && FunctionName == other.FunctionName
        && Arguments.SequenceEqual(other.Arguments)
        && Partitions.SequenceEqual(other.Partitions)
        && Orderings.SequenceEqual(other.Orderings)
        && FrameType == other.FrameType
        && FrameStart == other.FrameStart
        && FrameEnd == other.FrameEnd;

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(FunctionName);
        foreach (var a in Arguments) hash.Add(a);
        foreach (var p in Partitions) hash.Add(p);
        foreach (var o in Orderings) hash.Add(o);
        hash.Add(FrameType);
        hash.Add(FrameStart);
        hash.Add(FrameEnd);
        return hash.ToHashCode();
    }
}
