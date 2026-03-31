using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;

/// <summary>
/// Intermediate SQL expression that carries the PARTITION BY and ORDER BY clauses
/// of a window specification. This node is consumed by the window function translator
/// and should never reach final SQL rendering.
/// </summary>
internal sealed class WindowSpecSqlExpression : SqlExpression
{
    public IReadOnlyList<SqlExpression> Partitions { get; }
    public IReadOnlyList<OrderingExpression> Orderings { get; }

    public WindowSpecSqlExpression(
        IReadOnlyList<SqlExpression> partitions,
        IReadOnlyList<OrderingExpression> orderings,
        RelationalTypeMapping? typeMapping)
        : base(typeof(object), typeMapping)
    {
        Partitions = partitions;
        Orderings = orderings;
    }

    public WindowSpecSqlExpression WithPartition(SqlExpression partition)
    {
        var newPartitions = new List<SqlExpression>(Partitions) { partition };
        return new WindowSpecSqlExpression(newPartitions, Orderings, TypeMapping);
    }

    public WindowSpecSqlExpression WithOrdering(SqlExpression expression, bool ascending)
    {
        var newOrderings = new List<OrderingExpression>(Orderings)
        {
            new(expression, ascending)
        };
        return new WindowSpecSqlExpression(Partitions, newOrderings, TypeMapping);
    }

    protected override Expression VisitChildren(ExpressionVisitor visitor)
    {
        var changed = false;

        var newPartitions = new SqlExpression[Partitions.Count];
        for (var i = 0; i < Partitions.Count; i++)
        {
            newPartitions[i] = (SqlExpression)visitor.Visit(Partitions[i]);
            changed |= newPartitions[i] != Partitions[i];
        }

        var newOrderings = new OrderingExpression[Orderings.Count];
        for (var i = 0; i < Orderings.Count; i++)
        {
            newOrderings[i] = (OrderingExpression)visitor.Visit(Orderings[i]);
            changed |= newOrderings[i] != Orderings[i];
        }

        return changed
            ? new WindowSpecSqlExpression(newPartitions, newOrderings, TypeMapping)
            : this;
    }

    protected override void Print(ExpressionPrinter expressionPrinter)
    {
        expressionPrinter.Append("WindowSpec(");
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

#if NET9_0_OR_GREATER
    public override Expression Quote() =>
        throw new InvalidOperationException("WindowSpecSqlExpression is an intermediate node and should not be quoted.");
#endif

    public override bool Equals(object? obj) =>
        obj is WindowSpecSqlExpression other
        && Partitions.SequenceEqual(other.Partitions)
        && Orderings.SequenceEqual(other.Orderings);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var p in Partitions) hash.Add(p);
        foreach (var o in Orderings) hash.Add(o);
        return hash.ToHashCode();
    }
}
