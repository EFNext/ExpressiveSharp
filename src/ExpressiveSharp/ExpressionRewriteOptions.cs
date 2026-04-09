namespace ExpressiveSharp
{
    /// <summary>
    /// Options for controlling how inline lambdas in <c>IExpressiveQueryable&lt;T&gt;</c> chains are processed.
    /// </summary>
    /// <remarks>
    /// Transformers listed here are applied at runtime to the expression trees
    /// generated for inline lambdas in the query chain.
    /// </remarks>
    public sealed class ExpressionRewriteOptions
    {
        /// <summary>
        /// Transformers to apply to the expression trees in this query chain.
        /// </summary>
        public IExpressionTreeTransformer[]? Transformers { get; init; }
    }
}
