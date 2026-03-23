using System.Linq.Expressions;

namespace ExpressiveSharp;

/// <summary>
/// Transforms an expression tree at runtime. Implementations are pure functions
/// that take an expression and return a transformed version.
/// </summary>
public interface IExpressionTreeTransformer
{
    /// <summary>
    /// Transforms the given expression tree.
    /// </summary>
    Expression Transform(Expression expression);
}
