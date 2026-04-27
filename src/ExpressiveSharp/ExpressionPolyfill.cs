using System.Diagnostics;
using System.Linq.Expressions;

namespace ExpressiveSharp;

public static class ExpressionPolyfill
{
    /// <summary>
    /// Converts a lambda using modern C# syntax (e.g., <c>?.</c>) into an
    /// <see cref="Expression{TDelegate}"/>. The call is intercepted at compile time by
    /// the ExpressiveSharp source generator — it never executes at runtime.
    /// </summary>
    /// <example>
    /// <code>
    /// var expr = ExpressionPolyfill.Create((Order o) => o.Tag?.Length);
    /// var expr = ExpressionPolyfill.Create&lt;Func&lt;Order, int?&gt;&gt;(o => o.Tag?.Length);
    /// </code>
    /// </example>
    public static Expression<TDelegate> Create<TDelegate>(
        TDelegate lambda) where TDelegate : Delegate
        => throw new UnreachableException(
            "Must be intercepted by the ExpressiveSharp source generator. " +
            "Ensure the generator package is installed and the InterceptorsNamespaces MSBuild property is configured.");

    /// <summary>
    /// Like <see cref="Create{TDelegate}(TDelegate)"/>, but applies the given transformers
    /// to the generated expression tree at runtime before returning it.
    /// </summary>
    public static Expression<TDelegate> Create<TDelegate>(
        TDelegate lambda,
        params IExpressionTreeTransformer[] transformers) where TDelegate : Delegate
        => throw new UnreachableException(
            "Must be intercepted by the ExpressiveSharp source generator. " +
            "Ensure the generator package is installed and the InterceptorsNamespaces MSBuild property is configured.");
}
