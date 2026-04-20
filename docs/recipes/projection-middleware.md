# Computed Properties in Projection Middleware

If your computed property returns an empty value -- or is silently dropped from the response -- when consumed by **HotChocolate**, **AutoMapper's `ProjectTo`**, **Mapperly's projection mode**, or any other framework that emits `Select(src => new Entity { Member = src.Member, ... })` over your entity type, this recipe is the fix.

## Why plain `[Expressive]` isn't enough

HotChocolate's `[UseProjection]` middleware (and similar mechanisms in other libraries) generates a projection expression from the GraphQL selection set. Empirically, when the GraphQL query asks for only `fullName`:

```graphql
query { users { fullName } }
```

HotChocolate inspects the `User.FullName` property, finds it is **read-only** (no setter), and silently drops it from the projection. The generated SQL is `SELECT 1 FROM Users` -- nothing is fetched. At materialization time, HC constructs fresh `User` instances with all fields at their defaults (`FirstName = ""`, `LastName = ""`), calls the `FullName` getter, and the formula returns `", "`. The response looks successful but the data is wrong.

The same mechanism affects AutoMapper's `ProjectTo<Entity>`, Mapperly's generated projections, and any hand-rolled `Select(u => new User { ... })` that projects into the source type itself.

## The fix: `[ExpressiveFor(..., Synthesize = true)]`

Write the formula as a stub on a `partial` class and let the generator synthesize a settable property for you. The synthesized property is writable (so the projection middleware emits a binding) while still registering the formula for SQL translation. The dual-direction runtime behavior is:

- **In memory**, reading the property evaluates the stub formula from dependencies (same as plain `[Expressive]`).
- **After materialization from SQL**, reading the property returns the stored value (which the middleware's binding wrote via the `init` accessor).

## Before and after

**Before** -- plain `[Expressive]` on a read-only property. Broken for projection middleware.

```csharp
public class User
{
    public string FirstName { get; set; } = "";
    public string LastName  { get; set; } = "";

    [Expressive]
    public string FullName => LastName + ", " + FirstName;
}
```

GraphQL response: `{ "users": [{ "fullName": ", " }, { "fullName": ", " }] }` -- wrong.
SQL emitted: `SELECT 1 FROM Users` -- nothing fetched.

**After** -- `Synthesize = true` on a formula stub.

```csharp
public partial class User
{
    public string FirstName { get; set; } = "";
    public string LastName  { get; set; } = "";

    // The generator emits a settable FullName property whose getter falls through
    // to this stub when no value has been materialized yet.
    [ExpressiveFor("FullName", Synthesize = true)]
    private string FullNameExpression => LastName + ", " + FirstName;
}
```

GraphQL response: `{ "users": [{ "fullName": "Lovelace, Ada" }, { "fullName": "Turing, Alan" }] }` -- correct.
SQL emitted: `SELECT u.LastName || ', ' || u.FirstName AS "FullName" FROM Users u` -- formula pushed into SQL.

No HC glue code is required beyond the normal `.UseExpressives()` on the DbContext options. The convention auto-ignores the synthesized property in EF's model (so no `FullName` column is created), and the projection rewrite happens automatically when the query compiler intercepts.

::: warning Target name must be a string literal
`nameof(FullName)` won't resolve here because `FullName` doesn't exist until the generator emits it. Pass the name as a string literal: `[ExpressiveFor("FullName", Synthesize = true)]`.
:::

## Full HotChocolate example

```csharp
// Entity
public partial class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName  { get; set; } = "";
    public string Email     { get; set; } = "";

    [ExpressiveFor("FullName", Synthesize = true)]
    private string FullNameExpression => LastName + ", " + FirstName;

    [ExpressiveFor("DisplayLabel", Synthesize = true)]
    private string DisplayLabelExpression => FullName + " <" + Email + ">";
}

// DbContext
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
}

// Program.cs
builder.Services.AddDbContext<AppDbContext>(o => o
    .UseSqlServer(connectionString)
    .UseExpressives());

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddProjections();

// Query type
public class Query
{
    [UseProjection]
    public IQueryable<User> GetUsers([Service] AppDbContext db) => db.Users;
}
```

A GraphQL query for `{ users { displayLabel } }` now produces:

```sql
SELECT (u.LastName || ', ' || u.FirstName) || ' <' || u.Email || '>' AS "DisplayLabel"
FROM Users u
```

Notice how `DisplayLabel` composes with `FullName` (which is itself synthesized) -- the transitive rewrite is handled automatically by the expression resolver.

## Full AutoMapper example

AutoMapper's `ProjectTo<T>()` emits the same `new T { ... }` pattern as HotChocolate, so synthesized members work the same way:

```csharp
var config = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<User, User>();  // same-type projection
});

var users = await db.Users
    .ProjectTo<User>(config)
    .ToListAsync();

// SQL emitted: SELECT u.Id, u.FirstName, u.LastName, u.Email,
//              (u.LastName || ', ' || u.FirstName) AS "FullName",
//              (... || u.Email || '>') AS "DisplayLabel"
//              FROM Users u
```

## When the target property already exists

If you already have a settable auto-property (e.g. because it is used for DTO shape or deserialization), use the plain `[ExpressiveFor]` form without `Synthesize`:

```csharp
public class User
{
    public string FirstName { get; set; } = "";
    public string LastName  { get; set; } = "";
    public string FullName  { get; set; } = "";   // existing auto-property

    [ExpressiveFor(nameof(FullName))]
    private string FullNameExpression => LastName + ", " + FirstName;
}
```

The two forms produce the same SQL behaviour; the difference is who declares the target property. Use `Synthesize = true` when the property exists only to support projection middleware; use plain `[ExpressiveFor]` when the property has its own reason to exist.

## See Also

- [`[ExpressiveFor]` Mapping](../reference/expressive-for#synthesizing-a-property-with-synthesize-true) -- full `Synthesize = true` reference
- [Migrating from Projectables](../guide/migration-from-projectables) -- side-by-side migration paths for `UseMemberBody`
- [Computed Entity Properties](./computed-properties) -- plain `[Expressive]` computed values for DTO projections
