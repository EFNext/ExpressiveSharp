namespace ExpressiveSharp.MongoDB;

/// <summary>
/// Marks a queryable whose provider is an <see cref="Infrastructure.ExpressiveMongoQueryProvider"/>
/// that automatically expands <c>[Expressive]</c> members before MongoDB translates the query.
/// </summary>
public interface IExpressiveMongoQueryable<T> : IExpressiveQueryable<T>
{
}
