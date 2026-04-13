# Projectable Properties

## The problem

You have a computed property on your entity:

```csharp
public class User
{
    public string FirstName { get; set; } = "";
    public string LastName  { get; set; } = "";

    [Expressive]
    public string FullName => LastName + ", " + FirstName;
}
```

This works fine in direct LINQ (`db.Users.Select(u => u.FullName)`) and in DTO projections (`Select(u => new UserDto { Name = u.FullName })`). It **breaks** when something in your stack generates a projection that materializes back into the **same entity type**:

```csharp
// What HotChocolate's [UseProjection] middleware generates for `query { users { fullName } }`:
db.Users.Select(src => new User { FullName = src.FullName });
```

What happens next, silently:

- HotChocolate checks whether `FullName` is writable (it looks at `PropertyInfo.CanWrite`). It isn't — getter only.
- HC **drops** the `FullName = src.FullName` binding from the projection. No warning, no error.
- EF emits `SELECT 1 FROM Users` — nothing is fetched because the projection is empty.
- HC constructs `User` instances with all fields at their defaults (`FirstName = ""`, `LastName = ""`), then reads `user.FullName`.
- The getter evaluates `"" + ", " + ""` and returns `", "`.
- Your GraphQL response is `{ "users": [{ "fullName": ", " }, { "fullName": ", " }] }`.

The response looks valid but the data is garbage. This is the single most common trap when migrating from EFCore.Projectables or wiring up GraphQL against EF Core. The same mechanism affects **AutoMapper's `ProjectTo`**, **Mapperly's projection mode**, and any hand-rolled `Select(u => new User { ... })` that projects into the source type.

## The fix

Turn the property into a Projectable:

```csharp
[Expressive(Projectable = true)]
public string FullName
{
    get => field ?? (LastName + ", " + FirstName);
    init => field = value;
}
```

Now the property is **writable** (via `init`), so the projection middleware emits the binding. The formula still gets pushed down into SQL because ExpressiveSharp extracts it from the right operand of `??`. After the round trip, the response is correct:

```
{ "users": [{ "fullName": "Lovelace, Ada" }, { "fullName": "Turing, Alan" }] }
```

SQL emitted:

```sql
SELECT u.LastName || ', ' || u.FirstName AS "FullName" FROM Users u
```

No glue code beyond `UseExpressives()` on the DbContext. The property is auto-ignored by the EF model convention, so no column is created.

## How it works

A Projectable property has two states, distinguished by the backing field's value:

| State | `field` value | What the getter returns | How it gets here |
|---|---|---|---|
| **Not materialized** | `null` | Evaluates the formula from dependencies | In-memory construction (`new User { FirstName = "Ada" }`) |
| **Materialized** | non-null | The stored value | EF / HC wrote to `init` after computing the formula in SQL |

The `??` operator picks between the two. In both cases, reading `user.FullName` returns the correct value — the difference is only *where* the value came from.

```csharp
// State 1 — in memory, field is null, formula fires
var u1 = new User { FirstName = "Ada", LastName = "Lovelace" };
u1.FullName;  // "Lovelace, Ada" (computed)

// State 2 — after SQL materialization, stored value wins
var u2 = await db.Users.FirstAsync();
u2.FullName;  // "Lovelace, Ada" (stored, originally computed server-side)

// Mutation behavior differs between states
u1.FirstName = "Augusta"; u1.FullName;  // "Lovelace, Augusta" — formula reruns
u2.FirstName = "Augusta"; u2.FullName;  // "Lovelace, Ada" — stored value wins
```

The mutation-after-materialization behavior mirrors how EF's change tracking works: once a value is loaded, it stays put until something deliberately writes to it.

### Gotcha: stale values after dependency mutation

Once the backing field has been written — which happens every time the property is materialized from a SQL projection — mutating the formula's dependencies does **not** update the stored value. This can surprise users coming from [EFCore.Projectables](https://github.com/koenbeuk/EntityFrameworkCore.Projectables), where the formula always reruns on every read.

```csharp
// Loaded via EF/HC projection that included FullName — `field` is now populated.
var user = await db.Users.FirstAsync(u => u.Id == 1);
user.FullName;  // "Lovelace, Ada"

user.FirstName = "Augusta";
user.FullName;  // Still "Lovelace, Ada" — the stored value wins, formula is not rerun.
```

**Why this behaves this way**: the stored value is authoritative. ExpressiveSharp treats a materialized property the same way EF treats any loaded property — if you want a change reflected, you write it explicitly. This keeps the two states (in-memory-computed vs. SQL-materialized) from silently disagreeing with each other.

**The staleness applies only to materialized instances.** Two cases where the formula still fires on every read:

- **Constructed in memory** (`new User { FirstName = "Ada", LastName = "Lovelace" }`) — `field` is null, mutations to `FirstName`/`LastName` propagate as you'd expect.
- **Loaded without projecting the property** (e.g. `db.Users.FirstAsync()` without a `Select` that includes `FullName`) — the `init` accessor was never called, so `field` is still null.

**If you need the formula to rerun after dependency mutation on a materialized instance**, you have a few options:

1. **Re-fetch the entity** — let EF re-run the query with the new values.
2. **Use plain `[Expressive]` with a DTO projection** — if you're not going through projection middleware, a read-only `[Expressive]` on a DTO type is simpler and has no staleness.
3. **Expose a reset method** — for example `public void ResetFullName() { typeof(User).GetField(...).SetValue(this, null); }` to null out the backing field and let the formula fire again on the next read. This is rarely worth the complexity; options 1 and 2 cover the common cases.

## When to use it vs. plain `[Expressive]`

Only turn `Projectable = true` on when you actually have the problem above. The quick test:

> *Does anything in my stack generate `Select(src => new Entity { ... })` over this entity type?*

If you can answer yes (HotChocolate with `[UseProjection]`, AutoMapper `ProjectTo<SameType>`, Mapperly projections, hand-rolled patterns), the property needs to be Projectable. If not — if you only ever project into DTOs or read the property directly — plain `[Expressive]` is simpler and has no restrictions.

## Syntax

### Required shape

```csharp
[Expressive(Projectable = true)]
public string FullName
{
    get => field ?? (LastName + ", " + FirstName);
    init => field = value;
}
```

- **Get accessor**: must be `=> <backingField> ?? (<formula>)`. The generator matches this exact shape — ternaries (`a != null ? a : b`), block bodies with `if`/`return`, and other forms are rejected with [EXP0022](./diagnostics#exp0022).
- **Init/set accessor**: must be `=> <backingField> = value`. Transformations like `value?.Trim()` are rejected with [EXP0023](./diagnostics#exp0023).
- **Class does not need to be `partial`.** **Property does not need to be `partial`.**

### Backing field options

Either the C# 14 `field` keyword (preferred) or a manually declared private nullable field works:

```csharp
// Option A — `field` keyword (C# 14+)
[Expressive(Projectable = true)]
public string FullName
{
    get => field ?? (LastName + ", " + FirstName);
    init => field = value;
}

// Option B — manual backing field
private string? _fullName;

[Expressive(Projectable = true)]
public string FullName
{
    get => _fullName ?? (LastName + ", " + FirstName);
    init => _fullName = value;
}
```

The manual field must be **private**, **on the same type as the property**, and **nullable** (`string?` for reference types, `Nullable<T>` for value types). The `??` needs a distinguishable "not materialized" state.

### `init` vs. `set`

Both are accepted. Pick based on whether callers should be able to override the stored value after construction:

- `init` — value can only be set through object initializers (EF, HC) or the constructor. Recommended default.
- `set` — callers can also assign `user.FullName = "..."` directly. Useful if you want to support manual overrides.

## Restrictions

The generator enforces these at compile time. Each maps to a specific diagnostic for the exact error message:

| Restriction | Diagnostic |
|---|---|
| Property must declare `set` or `init`. | [EXP0021](./diagnostics#exp0021) |
| Get accessor must match `<field> ?? (<formula>)`. | [EXP0022](./diagnostics#exp0022) |
| Init/set must be `<field> = value` (no transformations in v1). | [EXP0023](./diagnostics#exp0023) |
| Property type must be non-nullable. | [EXP0024](./diagnostics#exp0024) |
| Manual backing field must match `Nullable<PropertyType>`. | [EXP0025](./diagnostics#exp0025) |
| Cannot combine with `required`. | [EXP0026](./diagnostics#exp0026) |
| Not allowed on interface properties. | [EXP0028](./diagnostics#exp0028) |
| Not allowed on `override` properties. | [EXP0029](./diagnostics#exp0029) |

## EF Core integration

With `UseExpressives()` on the DbContext options, the `ExpressivePropertiesNotMappedConvention` auto-ignores the property:

- **No column is created.** Migrations generated against the DbContext will not include a `FullName` column.
- **Queries work as expected.** `db.Users.Select(u => u.FullName)` emits SQL with the inlined formula.
- **Projections materialize correctly.** `Select(u => new User { FullName = u.FullName })` produces SQL like `SELECT LastName || ', ' || FirstName AS FullName FROM Users` and writes the result through `init`.

No `[NotMapped]` annotation or manual `modelBuilder.Ignore(...)` call is required.

## Comparison with `[ExpressiveFor]`

`[ExpressiveFor]` is the verbose alternative — the formula lives in a separate static stub instead of on the property:

```csharp
public class User
{
    public string FullName { get; set; } = "";
}

internal static class UserMappings
{
    [ExpressiveFor(typeof(User), nameof(User.FullName))]
    private static string FullName(User u) => u.LastName + ", " + u.FirstName;
}
```

Both produce identical SQL and both work with HC/AutoMapper. Pick based on preference:

- **`[Expressive(Projectable = true)]`** — formula lives on the property. Single declaration site. Recommended default.
- **`[ExpressiveFor]`** — formula in a separate class. More explicit. Required when you can't modify the entity type (third-party code) or need to map cross-type.

See the [migration guide](../guide/migration-from-projectables#migrating-usememberbody) for side-by-side examples.

## Further reading

- [Projection Middleware recipe](../recipes/projection-middleware) — end-to-end HotChocolate + AutoMapper examples.
- [`[Expressive]` Attribute reference](./expressive-attribute) — base attribute.
- [`[ExpressiveFor]` reference](./expressive-for) — verbose alternative.
