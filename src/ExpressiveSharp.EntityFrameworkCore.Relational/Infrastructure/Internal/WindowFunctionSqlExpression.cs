using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExpressiveSharp.EntityFrameworkCore.Relational.Infrastructure.Internal;

/// <summary>
/// SQL expression representing a window function call: FUNC_NAME(args) OVER(PARTITION BY ... ORDER BY ...).
/// Used for RANK, DENSE_RANK, NTILE (ROW_NUMBER uses the built-in <see cref="RowNumberExpression"/>).
/// <para>
/// This expression is self-rendering: <see cref="VisitChildren"/> produces correct SQL through
/// any provider's <see cref="QuerySqlGenerator"/> by interleaving <see cref="SqlFragmentExpression"/>
/// nodes with the actual column/ordering expressions. This makes it fully provider-agnostic —
/// no custom QuerySqlGenerator replacement is needed.
/// </para>
/// </summary>
internal sealed class WindowFunctionSqlExpression : SqlExpression
{
    public string FunctionName { get; }
    public IReadOnlyList<SqlExpression> Arguments { get; }
    public IReadOnlyList<SqlExpression> Partitions { get; }
    public IReadOnlyList<OrderingExpression> Orderings { get; }

    public WindowFunctionSqlExpression(
        string functionName,
        IReadOnlyList<SqlExpression> arguments,
        IReadOnlyList<SqlExpression> partitions,
        IReadOnlyList<OrderingExpression> orderings,
        Type type,
        RelationalTypeMapping? typeMapping)
        : base(type, typeMapping)
    {
        FunctionName = functionName;
        Arguments = arguments;
        Partitions = partitions;
        Orderings = orderings;
    }

    /// <summary>
    /// Self-rendering: when any QuerySqlGenerator visits this expression via VisitExtension,
    /// it calls VisitChildren, which visits SqlFragmentExpression and child SqlExpression nodes
    /// in the correct order to produce <c>FUNC(args) OVER(PARTITION BY ... ORDER BY ...)</c>.
    /// </summary>
    protected override Expression VisitChildren(ExpressionVisitor visitor)
    {
        EmitWindowFunction(
            text => visitor.Visit(new SqlFragmentExpression(text)),
            expr => visitor.Visit(expr));
        return this;
    }

    /// <summary>Diagnostic output for logging and ToString(). Not used for SQL generation.</summary>
    protected override void Print(ExpressionPrinter expressionPrinter) =>
        EmitWindowFunction(
            text => expressionPrinter.Append(text),
            expr => expressionPrinter.Visit(expr));

    /// <summary>
    /// Shared rendering logic for both SQL generation (<see cref="VisitChildren"/>) and
    /// diagnostic output (<see cref="Print"/>). Produces the
    /// <c>FUNC(args) OVER(PARTITION BY ... ORDER BY ...)</c> structure.
    /// </summary>
    private void EmitWindowFunction(Action<string> appendText, Action<Expression> visitExpression)
    {
        appendText($"{FunctionName}(");
        for (var i = 0; i < Arguments.Count; i++)
        {
            if (i > 0) appendText(", ");
            visitExpression(Arguments[i]);
        }
        appendText(") OVER(");

        if (Partitions.Count > 0)
        {
            appendText("PARTITION BY ");
            for (var i = 0; i < Partitions.Count; i++)
            {
                if (i > 0) appendText(", ");
                visitExpression(Partitions[i]);
            }
        }

        if (Orderings.Count > 0)
        {
            if (Partitions.Count > 0) appendText(" ");
            appendText("ORDER BY ");
            for (var i = 0; i < Orderings.Count; i++)
            {
                if (i > 0) appendText(", ");
                visitExpression(Orderings[i].Expression);
                appendText(Orderings[i].IsAscending ? " ASC" : " DESC");
            }
        }

        appendText(")");
    }

#if NET10_0_OR_GREATER
    public override Expression Quote() =>
        throw new InvalidOperationException("WindowFunctionSqlExpression quoting is not supported.");
#endif

    public override bool Equals(object? obj) =>
        obj is WindowFunctionSqlExpression other
        && FunctionName == other.FunctionName
        && Arguments.SequenceEqual(other.Arguments)
        && Partitions.SequenceEqual(other.Partitions)
        && Orderings.SequenceEqual(other.Orderings);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(FunctionName);
        foreach (var a in Arguments) hash.Add(a);
        foreach (var p in Partitions) hash.Add(p);
        foreach (var o in Orderings) hash.Add(o);
        return hash.ToHashCode();
    }
}
