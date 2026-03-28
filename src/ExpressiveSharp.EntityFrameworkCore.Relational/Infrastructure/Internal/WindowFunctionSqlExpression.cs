using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExpressiveSharp.EntityFrameworkCore.Relational.Infrastructure.Internal;

/// <summary>
/// SQL expression representing a window function call: FUNC_NAME(args) OVER(PARTITION BY ... ORDER BY ...).
/// Used for RANK, DENSE_RANK, NTILE (ROW_NUMBER uses the built-in <see cref="RowNumberExpression"/>).
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

    protected override Expression VisitChildren(ExpressionVisitor visitor)
    {
        var changed = false;

        var newArguments = VisitList(visitor, Arguments, ref changed);
        var newPartitions = VisitList(visitor, Partitions, ref changed);

        var newOrderings = new OrderingExpression[Orderings.Count];
        for (var i = 0; i < Orderings.Count; i++)
        {
            newOrderings[i] = (OrderingExpression)visitor.Visit(Orderings[i]);
            changed |= newOrderings[i] != Orderings[i];
        }

        return changed
            ? new WindowFunctionSqlExpression(FunctionName, newArguments, newPartitions, newOrderings, Type, TypeMapping)
            : this;
    }

    private static SqlExpression[] VisitList(ExpressionVisitor visitor, IReadOnlyList<SqlExpression> list, ref bool changed)
    {
        var result = new SqlExpression[list.Count];
        for (var i = 0; i < list.Count; i++)
        {
            result[i] = (SqlExpression)visitor.Visit(list[i]);
            changed |= result[i] != list[i];
        }
        return result;
    }

    protected override void Print(ExpressionPrinter expressionPrinter)
    {
        expressionPrinter.Append(FunctionName).Append("(");
        for (var i = 0; i < Arguments.Count; i++)
        {
            if (i > 0) expressionPrinter.Append(", ");
            expressionPrinter.Visit(Arguments[i]);
        }
        expressionPrinter.Append(") OVER(");

        if (Partitions.Count > 0)
        {
            expressionPrinter.Append("PARTITION BY ");
            for (var i = 0; i < Partitions.Count; i++)
            {
                if (i > 0) expressionPrinter.Append(", ");
                expressionPrinter.Visit(Partitions[i]);
            }
        }

        if (Orderings.Count > 0)
        {
            if (Partitions.Count > 0) expressionPrinter.Append(" ");
            expressionPrinter.Append("ORDER BY ");
            for (var i = 0; i < Orderings.Count; i++)
            {
                if (i > 0) expressionPrinter.Append(", ");
                expressionPrinter.Visit(Orderings[i].Expression);
                expressionPrinter.Append(Orderings[i].IsAscending ? " ASC" : " DESC");
            }
        }

        expressionPrinter.Append(")");
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
