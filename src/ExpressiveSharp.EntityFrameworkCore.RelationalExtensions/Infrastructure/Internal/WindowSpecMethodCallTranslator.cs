using System.Reflection;
using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;

/// <summary>
/// Translates <see cref="Window"/> static methods, <see cref="PartitionedWindowDefinition"/>/
/// <see cref="OrderedWindowDefinition"/> instance methods, and <see cref="WindowFrameBound"/>
/// factory methods into intermediate SQL expression nodes
/// (<see cref="WindowSpecSqlExpression"/>, <see cref="WindowFrameBoundSqlExpression"/>).
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

        // Static factory methods on WindowFrameBound (the no-arg variants
        // UnboundedPreceding/CurrentRow/UnboundedFollowing are properties and are
        // handled by WindowFrameBoundMemberTranslator instead).
        if (declaringType == typeof(WindowFrameBound))
        {
            return method.Name switch
            {
                nameof(WindowFrameBound.Preceding) when TryGetIntConstant(arguments[0], out var offset) =>
                    new WindowFrameBoundSqlExpression(new WindowFrameBoundInfo(WindowFrameBoundKind.Preceding, offset)),
                nameof(WindowFrameBound.Following) when TryGetIntConstant(arguments[0], out var offset) =>
                    new WindowFrameBoundSqlExpression(new WindowFrameBoundInfo(WindowFrameBoundKind.Following, offset)),
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
                nameof(OrderedWindowDefinition.RowsBetween) when arguments is [WindowFrameBoundSqlExpression start, WindowFrameBoundSqlExpression end] =>
                    spec.WithFrame(WindowFrameType.Rows, start.BoundInfo, end.BoundInfo),
                nameof(OrderedWindowDefinition.RangeBetween) when arguments is [WindowFrameBoundSqlExpression start, WindowFrameBoundSqlExpression end] =>
                    spec.WithFrame(WindowFrameType.Range, start.BoundInfo, end.BoundInfo),
                _ => null
            };
        }

        return null;
    }

    private static bool TryGetIntConstant(SqlExpression expression, out int value)
    {
        if (expression is SqlConstantExpression { Value: int i })
        {
            value = i;
            return true;
        }

        value = 0;
        return false;
    }
}
