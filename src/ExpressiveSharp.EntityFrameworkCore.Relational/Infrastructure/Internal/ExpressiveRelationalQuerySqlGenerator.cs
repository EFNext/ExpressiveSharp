using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ExpressiveSharp.EntityFrameworkCore.Relational.Infrastructure.Internal;

/// <summary>
/// Extends <see cref="QuerySqlGenerator"/> to render <see cref="WindowFunctionSqlExpression"/>
/// as <c>FUNC_NAME(args) OVER(PARTITION BY ... ORDER BY ...)</c>.
/// </summary>
internal sealed class ExpressiveRelationalQuerySqlGenerator : QuerySqlGenerator
{
    public ExpressiveRelationalQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies)
        : base(dependencies)
    {
    }

    protected override Expression VisitExtension(Expression extensionExpression)
    {
        if (extensionExpression is WindowFunctionSqlExpression windowFunc)
        {
            VisitWindowFunction(windowFunc);
            return extensionExpression;
        }

        return base.VisitExtension(extensionExpression);
    }

    private void VisitWindowFunction(WindowFunctionSqlExpression expression)
    {
        var sql = Sql;

        sql.Append(expression.FunctionName).Append("(");
        for (var i = 0; i < expression.Arguments.Count; i++)
        {
            if (i > 0) sql.Append(", ");
            Visit(expression.Arguments[i]);
        }
        sql.Append(") OVER(");

        if (expression.Partitions.Count > 0)
        {
            sql.Append("PARTITION BY ");
            for (var i = 0; i < expression.Partitions.Count; i++)
            {
                if (i > 0) sql.Append(", ");
                Visit(expression.Partitions[i]);
            }
        }

        if (expression.Orderings.Count > 0)
        {
            if (expression.Partitions.Count > 0) sql.Append(" ");
            sql.Append("ORDER BY ");
            for (var i = 0; i < expression.Orderings.Count; i++)
            {
                if (i > 0) sql.Append(", ");
                Visit(expression.Orderings[i].Expression);
                sql.Append(expression.Orderings[i].IsAscending ? " ASC" : " DESC");
            }
        }

        sql.Append(")");
    }
}
