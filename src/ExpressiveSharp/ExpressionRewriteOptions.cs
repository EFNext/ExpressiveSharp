namespace ExpressiveSharp
{
    /// <summary>
    /// Transformers applied at runtime to the expression trees generated for inline lambdas
    /// in an <c>IExpressiveQueryable&lt;T&gt;</c> chain.
    /// </summary>
    public sealed class ExpressionRewriteOptions
    {
        public IExpressionTreeTransformer[]? Transformers { get; init; }
    }
}
