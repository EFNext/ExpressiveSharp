using System.Linq;

namespace ExpressiveSharp
{
    /// <summary>
    /// Represents a queryable sequence with expression-rewrite support. Extends <see cref="IQueryable{T}"/> so that all
    /// standard EF Core and LINQ queryable extensions remain available, while also exposing
    /// delegate-based overloads of common LINQ operators (Where, Select, OrderBy, etc.) that allow
    /// modern C# syntax — such as null-conditional operators — to be used in inline lambdas.
    /// </summary>
    /// <remarks>
    /// The delegate-based overloads are intercepted at compile time by the
    /// ExpressiveSharp source generator, which rewrites the lambda body into an
    /// expression tree using the same rewrite rules as <see cref="ExpressiveAttribute"/>.
    /// </remarks>
    public interface IRewritableQueryable<T> : IQueryable<T>
    {
    }
}
