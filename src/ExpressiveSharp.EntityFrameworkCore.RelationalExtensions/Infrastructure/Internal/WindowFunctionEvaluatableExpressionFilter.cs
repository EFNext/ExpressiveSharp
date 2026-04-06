using System.Linq.Expressions;
using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;
using Microsoft.EntityFrameworkCore.Query;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;

/// <summary>
/// Prevents window function marker method calls and property accesses from being
/// client-evaluated. These must remain as expression tree nodes for the
/// method/member translators to handle.
/// </summary>
internal sealed class WindowFunctionEvaluatableExpressionFilter : IEvaluatableExpressionFilterPlugin
{
    public bool IsEvaluatableExpression(Expression expression)
    {
        if (expression is MethodCallExpression methodCall)
        {
            var declaringType = methodCall.Method.DeclaringType;
            if (declaringType == typeof(Window)
                || declaringType == typeof(PartitionedWindowDefinition)
                || declaringType == typeof(OrderedWindowDefinition)
                || declaringType == typeof(WindowFunction)
                || declaringType == typeof(WindowFrameBound))
            {
                return false;
            }
        }

        // WindowFrameBound.UnboundedPreceding/CurrentRow/UnboundedFollowing are
        // static property getters — surface as MemberExpression nodes, not method calls.
        if (expression is MemberExpression memberAccess
            && memberAccess.Member.DeclaringType == typeof(WindowFrameBound))
        {
            return false;
        }

        return true;
    }
}
