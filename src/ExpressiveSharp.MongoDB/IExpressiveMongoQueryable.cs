namespace ExpressiveSharp.MongoDB;

/// <summary>
/// Represents a MongoDB queryable with expression-rewrite support. Extends
/// <see cref="IExpressiveQueryable{T}"/> to enable delegate-based LINQ methods
/// with modern C# syntax (e.g., <c>?.</c>, switch expressions, pattern matching)
/// when querying MongoDB collections.
/// </summary>
/// <remarks>
/// In MongoDB.Driver v3, the driver's LINQ provider works through standard
/// <see cref="IQueryable{T}"/> with an <see cref="MongoDB.Driver.Linq.IMongoQueryProvider"/>.
/// This interface marks a queryable whose provider is an <see cref="Infrastructure.ExpressiveMongoQueryProvider"/>
/// that automatically expands <c>[Expressive]</c> members before MongoDB translates the query.
/// </remarks>
public interface IExpressiveMongoQueryable<T> : IExpressiveQueryable<T>
{
}
