using System.Linq;

namespace ExpressiveSharp
{
    /// <summary>
    /// Queryable sequence with expression-rewrite support. Extends <see cref="IQueryable{T}"/>
    /// so existing LINQ/EF Core extensions remain available, and exposes delegate-based
    /// overloads of common operators that the source generator rewrites into expression trees,
    /// allowing modern C# syntax (null-conditional, pattern matching, etc.) in inline lambdas.
    /// </summary>
    public interface IExpressiveQueryable<T> : IQueryable<T>
    {
    }
}
