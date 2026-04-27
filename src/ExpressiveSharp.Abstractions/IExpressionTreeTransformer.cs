using System.Linq.Expressions;

namespace ExpressiveSharp;

/// <summary>
/// Transforms an expression tree at runtime. Implementations must be pure.
/// </summary>
public interface IExpressionTreeTransformer
{
    Expression Transform(Expression expression);
}
