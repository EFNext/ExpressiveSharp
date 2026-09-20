using System.Linq;

namespace ExpressiveSharp
{
    /// <summary>
    /// An <see cref="IExpressiveQueryable{T}"/> whose elements are already ordered. Returned by
    /// <c>OrderBy</c>/<c>OrderByDescending</c> and required as the receiver of
    /// <c>ThenBy</c>/<c>ThenByDescending</c>, so composing a secondary sort key onto an unordered
    /// source is a compile error rather than a runtime failure. Declares no members.
    /// </summary>
    public interface IOrderedExpressiveQueryable<T> : IExpressiveQueryable<T>, IOrderedQueryable<T>
    {
    }
}
