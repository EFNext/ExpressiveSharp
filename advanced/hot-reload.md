---
url: 'https://efnext.github.io/ExpressiveSharp/advanced/hot-reload.md'
---
# Hot Reload

ExpressiveSharp supports .NET hot reload. When you edit the body of an `[Expressive]` member during a `dotnet watch` (or IDE hot-reload) session, the next query execution uses the new expression — no manual cache clear, app restart, or rebuild required.

## How It Works

Three things happen on a hot-reload event:

1. **Source generator re-runs incrementally.** Roslyn re-emits the per-assembly `ExpressionRegistry` so the factory methods produce lambdas built from the new IL.
2. **The runtime invokes ExpressiveSharp's `[MetadataUpdateHandler]`.** This handler calls `ResetMap()` on each affected assembly's generated registry (rebuilding the `MethodHandle → LambdaExpression` dictionary) and clears the resolver and replacer caches.
3. **The next query expands fresh.** EF Core (and MongoDB) integrations call `ExpandExpressives()` on every query execution, so the new expression body is woven in immediately.

EF Core's compiled-query cache keys off the post-expansion tree shape. A structural change to an `[Expressive]` body produces a different cache key, so a fresh SQL compilation happens automatically.

## Caveats

::: warning `EF.CompileQuery` results are snapshotted
`EF.CompileQuery` invokes expansion **once**, at compile time, and returns a delegate. Hot-reloading an `[Expressive]` member after the delegate is built does **not** retroactively update it. To pick up the new body, recreate the compiled query.
:::

::: warning Manually captured expression trees are frozen
If you call `.ExpandExpressives()` yourself and store the result in a field or static, the tree is captured at that moment. Hot reload does not rewrite already-materialized trees held in user memory. Prefer the per-execution path (`AsExpressive()` + LINQ, or the `ExpressiveQueryCompiler` decorator wired by `UseExpressives()`).
:::

::: tip Constant-only edits behave like normal EF Core parameterization
Changing `p.Status == 1` to `p.Status == 2` inside an `[Expressive]` member reuses the same compiled SQL with a different parameter — exactly the same behaviour as inline literals. This is the EF Core constant-parameterization rule, not an ExpressiveSharp limitation.
:::

::: info Rude edits still need a restart
Signature changes, member removal, attribute edits, and other [rude edits](https://learn.microsoft.com/en-us/aspnet/core/test/hot-reload) require an app restart — same as any other `dotnet watch` rude edit. Body-only changes are the supported case.
:::

## Verifying It Works

1. Start your app under `dotnet watch run` with EF Core SQL logging enabled (`LogTo(Console.WriteLine, LogLevel.Information)`).

2. Run a query that uses an `[Expressive]` member:

   ```csharp
   public sealed class Product
   {
       public int Quantity { get; set; }
       public int Price { get; set; }

       [Expressive]
       public int Total => Quantity * Price;
   }

   // ...
   var rows = ctx.Products.AsExpressive().Where(p => p.Total > 100).ToList();
   ```

3. Observe the SQL: `WHERE [p].[Quantity] * [p].[Price] > @__p_0`.

4. Edit `Total` to `Quantity * Price * 2`. Save.

5. Trigger the same query path. The new SQL should reflect the updated computation.

If the SQL does not update, check that you are not (a) calling through `EF.CompileQuery`, or (b) reusing a captured `Expression` tree built before the edit.
