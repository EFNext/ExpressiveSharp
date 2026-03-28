using System.Reflection;
using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;

/// <summary>
/// Translates <see cref="Window"/> static methods and <see cref="PartitionedWindowDefinition"/>/<see cref="OrderedWindowDefinition"/>
/// instance methods into <see cref="WindowSpecSqlExpression"/> intermediate nodes.
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

        // Instance methods on PartitionedWindowDefinition and OrderedWindowDefinition
        if ((declaringType == typeof(PartitionedWindowDefinition) || declaringType == typeof(OrderedWindowDefinition))
            && instance is WindowSpecSqlExpression spec)
        {
            return method.Name switch
            {
                nameof(PartitionedWindowDefinition.PartitionBy) => spec.WithPartition(arguments[0]),
                nameof(PartitionedWindowDefinition.OrderBy) or nameof(OrderedWindowDefinition.ThenBy) =>
                    spec.WithOrdering(arguments[0], ascending: true),
                nameof(PartitionedWindowDefinition.OrderByDescending) or nameof(OrderedWindowDefinition.ThenByDescending) =>
                    spec.WithOrdering(arguments[0], ascending: false),
                _ => null
            };
        }

        return null;
    }
}
