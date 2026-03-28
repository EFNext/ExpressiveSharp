using System.Linq.Expressions;
using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;
using Microsoft.EntityFrameworkCore.Query;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;

/// <summary>
/// Prevents <see cref="Window"/>, <see cref="WindowDefinition"/>, and <see cref="WindowFunction"/>
/// method calls from being client-evaluated. These must remain as expression tree nodes
/// for the method call translators to handle.
/// </summary>
internal sealed class WindowFunctionEvaluatableExpressionFilter : IEvaluatableExpressionFilterPlugin
{
    public bool IsEvaluatableExpression(Expression expression)
    {
        if (expression is MethodCallExpression methodCall)
        {
            var declaringType = methodCall.Method.DeclaringType;
            if (declaringType == typeof(Window)
                || declaringType == typeof(WindowDefinition)
                || declaringType == typeof(WindowFunction))
            {
                return false;
            }
        }

        return true;
    }
}
