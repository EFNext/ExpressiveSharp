using System.Diagnostics;
using System.Linq.Expressions;

namespace ExpressiveSharp;

public static class ExpressionPolyfill
{
    /// <summary>
    /// Converts a lambda that uses modern C# syntax (e.g., <c>?.</c>) into a valid
    /// <see cref="Expression{TDelegate}"/>. The call is intercepted at compile time by
    /// the ExpressiveSharp source generator — it never executes at runtime.
    /// </summary>
    /// <example>
    /// <code>
    /// // Type argument is inferred from the explicitly-typed lambda parameter:
    /// var expr = ExpressionPolyfill.Create((Order o) => o.Tag?.Length);
    ///
    /// // Explicit type argument also works:
    /// var expr = ExpressionPolyfill.Create&lt;Func&lt;Order, int?&gt;&gt;(o => o.Tag?.Length);
    /// </code>
    /// </example>
    /// <typeparam name="TDelegate">The delegate type of the lambda (usually inferred).</typeparam>
    /// <param name="lambda">The lambda expression to polyfill.</param>
    /// <returns>An expression tree equivalent of <paramref name="lambda"/>.</returns>
    public static Expression<TDelegate> Create<TDelegate>(
        TDelegate lambda) where TDelegate : Delegate
        => throw new UnreachableException(
            "Must be intercepted by the ExpressiveSharp source generator. " +
            "Ensure the generator package is installed and the InterceptorsNamespaces MSBuild property is configured.");

    /// <summary>
    /// Converts a lambda that uses modern C# syntax (e.g., <c>?.</c>) into a valid
    /// <see cref="Expression{TDelegate}"/>, then applies the specified transformers.
    /// The expression tree is built at compile time by the source generator; the
    /// transformers are applied at runtime before the result is returned.
    /// </summary>
    /// <typeparam name="TDelegate">The delegate type of the lambda (usually inferred).</typeparam>
    /// <param name="lambda">The lambda expression to polyfill.</param>
    /// <param name="transformers">Transformers to apply to the expression tree at runtime.</param>
    /// <returns>A transformed expression tree equivalent of <paramref name="lambda"/>.</returns>
    public static Expression<TDelegate> Create<TDelegate>(
        TDelegate lambda,
        params IExpressionTreeTransformer[] transformers) where TDelegate : Delegate
        => throw new UnreachableException(
            "Must be intercepted by the ExpressiveSharp source generator. " +
            "Ensure the generator package is installed and the InterceptorsNamespaces MSBuild property is configured.");
}
