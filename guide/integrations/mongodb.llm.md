# MongoDB

The `ExpressiveSharp.MongoDB` package integrates ExpressiveSharp with the official `MongoDB.Driver` LINQ provider. Modern C# syntax — null-conditional operators, switch expressions, pattern matching — and `[Expressive]` members are translated into MongoDB aggregation pipelines.

## Installation

```bash
dotnet add package ExpressiveSharp.MongoDB
```

Depends on `ExpressiveSharp` (core runtime) and `MongoDB.Driver` 3.x.

## Basic Setup

Call `.AsExpressive()` on an `IMongoCollection<T>` to get an `IExpressiveMongoQueryable<T>`:

```csharp
using ExpressiveSharp.MongoDB.Extensions;
using MongoDB.Driver;

var client = new MongoClient("mongodb://localhost:27017");
var db = client.GetDatabase("shop");

var customers = db.GetCollection<Customer>("customers").AsExpressive();
var orders = db.GetCollection<Order>("orders").AsExpressive();
```

That's it. Both modern syntax and `[Expressive]` member expansion are now active:

```csharp
db
    .Customers
    .Where(c => c.Email != null && c.Orders.Count() > 5)
    .Select(c => new { c.Name, OrderCount = c.Orders.Count() })
```

**Generated SQL (SQLite):**

```sql
SELECT "c"."Name", (
    SELECT COUNT(*)
    FROM "Orders" AS "o0"
    WHERE "c"."Id" = "o0"."CustomerId") AS "OrderCount"
FROM "Customers" AS "c"
WHERE "c"."Email" IS NOT NULL AND (
    SELECT COUNT(*)
    FROM "Orders" AS "o"
    WHERE "c"."Id" = "o"."CustomerId") > 5
```


## What AsExpressive() Does

`AsExpressive()` wraps MongoDB's LINQ provider with ExpressiveSharp's query provider:

1. **Expands `[Expressive]` member references** — walks the expression tree and replaces opaque property/method accesses with the generated expression trees
2. **Applies MongoDB-friendly transformers** — strips null-conditional patterns, flattens blocks, normalizes tuple access
3. **Delegates execution to MongoDB's aggregation pipeline** — the rewritten tree is handed back to the MongoDB LINQ provider unchanged in shape

No custom MQL is emitted — MongoDB's own translator does all the heavy lifting after ExpressiveSharp has normalized the tree.

## `[Expressive]` Properties Are Unmapped from BSON

ExpressiveSharp provides a MongoDB `IClassMapConvention` that unmaps every `[Expressive]`-decorated property from the BSON class map, so the property's backing field is not persisted to documents. This matters most for [Projectable properties](../../reference/projectable-properties), which have a writable `init` accessor and would otherwise be serialized as a real BSON field.

::: warning Ordering constraint
MongoDB builds and caches a class map the first time you call `IMongoDatabase.GetCollection<T>()` for a given `T`. A convention registered *after* that call does not apply to the cached map. If any of your document types use `[Expressive]`, register the convention before the first `GetCollection<T>` call:

```csharp
using ExpressiveSharp.MongoDB.Infrastructure;

// At application startup, before any GetCollection<T>:
ExpressiveMongoIgnoreConvention.EnsureRegistered();

var client = new MongoClient(connectionString);
var db = client.GetDatabase("shop");
var customers = db.GetCollection<Customer>("customers");  // class map built now
```

The convention is also registered automatically when you construct `ExpressiveMongoCollection<T>` or call `collection.AsExpressive()` — but only if that happens before any `GetCollection<T>` call for a type with `[Expressive]` properties. The explicit `EnsureRegistered()` call is the most reliable pattern.
:::

## Async Methods

All MongoDB async LINQ methods (from `MongoQueryable`) work with modern syntax via interceptors. They are stubs on `IExpressiveMongoQueryable<T>` that forward to their `MongoQueryable` counterparts:

```csharp
// Predicate / element access
var exists = await customers.AnyAsync(c => c.Orders.Count() > 0);
var first = await customers.FirstOrDefaultAsync(c => c.Email != null);
var count = await customers.CountAsync(c => c.Country == "US");

// Aggregation
var total = await orders.SumAsync(o => o.Price * o.Quantity);
var avg = await orders.AverageAsync(o => o.Price);
```

## Inspecting the Pipeline

Call `.ToString()` on an `IExpressiveMongoQueryable<T>` to see the generated aggregation pipeline without executing it:

```csharp
var query = customers
    .Where(c => c.Email != null)
    .Select(c => new { c.Name, c.Email });

Console.WriteLine(query.ToString());
```

Output:

```
shop.customers.Aggregate([
    { "$match" : { "Email" : { "$ne" : null } } },
    { "$project" : { "Name" : "$Name", "Email" : "$Email", "_id" : 0 } }
])
```

## Caveats

- **No navigation properties.** MongoDB is a document store; `[Expressive]` members that reach across collections (`customer.Orders`) assume the related data is embedded as a subdocument. If your schema uses references across collections, project and `$lookup` explicitly.
- **No cross-field `[Expressive]` with untracked fields.** MongoDB's LINQ provider requires every field referenced in a projection or filter to exist in the document schema. An `[Expressive]` member that references a non-persisted field won't translate.

## Next Steps

- [IExpressiveQueryable\<T\>](../expressive-queryable) — the core provider-agnostic API
- [[Expressive] Properties](../expressive-properties) — computed properties in depth
- [Custom Providers](./custom-providers) — use `.AsExpressive()` on any `IQueryable<T>`
