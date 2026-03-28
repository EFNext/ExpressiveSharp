using System.Reflection;
using ExpressiveSharp.EntityFrameworkCore.Relational.WindowFunctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ExpressiveSharp.EntityFrameworkCore.Relational.Infrastructure.Internal;

/// <summary>
/// Translates <see cref="Window"/> static methods and <see cref="WindowDefinition"/> instance methods
/// into <see cref="WindowSpecSqlExpression"/> intermediate nodes.
/// </summary>
internal sealed class WindowSpecMethodCallTranslator : IMethodCallTranslator
{
    public SqlExpression? Translate(
        SqlExpression? instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        var declaringType = method.DeclaringType;

        // Static methods on Window class
        if (declaringType == typeof(Window))
        {
            return method.Name switch
            {
                nameof(Window.PartitionBy) => new WindowSpecSqlExpression(
                    [arguments[0]], [], typeMapping: null),
                nameof(Window.OrderBy) => new WindowSpecSqlExpression(
                    [], [new OrderingExpression(arguments[0], ascending: true)], typeMapping: null),
                nameof(Window.OrderByDescending) => new WindowSpecSqlExpression(
                    [], [new OrderingExpression(arguments[0], ascending: false)], typeMapping: null),
                _ => null
            };
        }

        // Instance methods on WindowDefinition
        if (declaringType == typeof(WindowDefinition) && instance is WindowSpecSqlExpression spec)
        {
            return method.Name switch
            {
                nameof(WindowDefinition.PartitionBy) => spec.WithPartition(arguments[0]),
                nameof(WindowDefinition.OrderBy) or nameof(WindowDefinition.ThenBy) =>
                    spec.WithOrdering(arguments[0], ascending: true),
                nameof(WindowDefinition.OrderByDescending) or nameof(WindowDefinition.ThenByDescending) =>
                    spec.WithOrdering(arguments[0], ascending: false),
                _ => null
            };
        }

        return null;
    }
}
