using ExpressiveSharp.Docs.Playground.Core.Services;
using ExpressiveSharp.Docs.Playground.Core.Services.Scenarios;
using ExpressiveSharp.Docs.PlaygroundModel.Webshop;
using ExpressiveSharp.EntityFrameworkCore;
using ExpressiveSharp.MongoDB.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ExpressiveSharp;

namespace ExpressiveSharp.Docs.Prerenderer;

internal sealed record RenderedSample(
    string Key,
    string Snippet,
    string? Setup,
    Dictionary<string, RenderedTarget> Targets);

internal sealed record RenderedTarget(
    string Label,
    string Language,
    string Output,
    bool IsError = false);

internal sealed class SampleRenderer : IAsyncDisposable
{
    private readonly SnippetCompiler _compiler;

    // A fresh context per sample is essential: reusing a DbContext (or shared
    // MongoDB root) across samples corrupts EF Core's query cache when one
    // sample's translation fails. Downstream samples then surface errors like
    // "Expression of type 'System.Object' cannot be used for return type
    // 'System.String'" even though the snippet is translatable in isolation.

    private static WebshopDbContext BuildSqlServerContext() =>
        new(new DbContextOptionsBuilder<WebshopDbContext>()
            .UseSqlServer("Server=.;Database=playground")
            .UseExpressives()
            .EnableServiceProviderCaching(false)
            .Options);

    private static WebshopDbContext BuildCosmosContext() =>
        new(new DbContextOptionsBuilder<WebshopDbContext>()
            .UseCosmos("AccountEndpoint=https://localhost:8081/;AccountKey=dW5pdHRlc3Q=", "playground")
            .UseExpressives()
            .EnableServiceProviderCaching(false)
            .Options);

    private static IWebshopQueryRoots BuildMongoRoots()
    {
        var db = new MongoClient("mongodb://localhost:27017").GetDatabase("playground");
        return new MongoRootsImpl(
            db.GetCollection<Customer>("customers").AsExpressive(),
            db.GetCollection<Order>("orders").AsExpressive(),
            db.GetCollection<Product>("products").AsExpressive(),
            db.GetCollection<LineItem>("line_items").AsExpressive());
    }

    private sealed class DbContextRoots : IWebshopQueryRoots
    {
        private readonly WebshopDbContext _ctx;
        public DbContextRoots(WebshopDbContext ctx) { _ctx = ctx; }
        public IExpressiveQueryable<Customer> Customers => _ctx.Customers;
        public IExpressiveQueryable<Order> Orders => _ctx.Orders;
        public IExpressiveQueryable<Product> Products => _ctx.Products;
        public IExpressiveQueryable<LineItem> LineItems => _ctx.LineItems;
    }

    private sealed class MongoRootsImpl : IWebshopQueryRoots
    {
        public MongoRootsImpl(
            IExpressiveQueryable<Customer> customers,
            IExpressiveQueryable<Order> orders,
            IExpressiveQueryable<Product> products,
            IExpressiveQueryable<LineItem> lineItems)
        {
            Customers = customers;
            Orders = orders;
            Products = products;
            LineItems = lineItems;
        }

        public IExpressiveQueryable<Customer> Customers { get; }
        public IExpressiveQueryable<Order> Orders { get; }
        public IExpressiveQueryable<Product> Products { get; }
        public IExpressiveQueryable<LineItem> LineItems { get; }
    }

    public SampleRenderer(SnippetCompiler compiler)
    {
        _compiler = compiler;
    }

    public RenderedSample Render(DocSample sample)
    {
        var scenario = ScenarioRegistry.Resolve(sample.ScenarioId);
        var formatted = FormatSnippet(sample.Snippet);
        var result = _compiler.Compile(sample.Snippet, sample.Setup, scenario);

        if (!result.Success)
        {
            var errors = string.Join(Environment.NewLine, result.Diagnostics.Select(d => d.ToString()));
            throw new InvalidOperationException(
                $"Sample in {sample.FilePath} (index {sample.Index}) failed to compile:\n{errors}");
        }

        if (result.Assembly is null)
            throw new InvalidOperationException(
                $"Sample in {sample.FilePath} (index {sample.Index}) compiled but produced no assembly.");

        var snippetType = result.Assembly.GetType(SnippetCompiler.SnippetTypeFullName)
            ?? throw new InvalidOperationException(
                $"Snippet type '{SnippetCompiler.SnippetTypeFullName}' not found.");
        var runMethod = snippetType.GetMethod("Run", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            ?? throw new InvalidOperationException("Snippet.Run method not found.");

        Func<object, IQueryable> invoke = arg =>
            (IQueryable?)runMethod.Invoke(null, new[] { arg })
                ?? throw new InvalidOperationException("Snippet.Run returned null.");

        // Reset ExpressiveSharp's process-level resolver caches between samples,
        // then restrict registry scanning to only the current snippet's assembly.
        // Without this, previous samples' assemblies accumulate in the AppDomain,
        // their [ExpressiveFor] registrations overlap with the current sample's,
        // and FindExternalExpression throws "Multiple mappings found".
        ExpressiveSharp.Services.ExpressiveResolver.ResetAllCaches();
        var snippetAssembly = result.Assembly;
        ExpressiveSharp.Services.ExpressiveResolver.SetAssemblyScanFilter(
            asm => asm == snippetAssembly
                || asm == typeof(ExpressiveSharp.ExpressiveAttribute).Assembly
                || asm == typeof(ExpressiveSharp.EntityFrameworkCore.ExpressiveDbSet<>).Assembly);

        // Fresh scenario instance per sample — isolates per-sample EF Core
        // query-cache state so one sample's failure can't poison the next.
        using var instanceScope = new ScenarioInstanceScope(scenario);
        var targets = new Dictionary<string, RenderedTarget>();

        foreach (var renderTarget in scenario.RenderTargets)
        {
            try
            {
                var queryArgument = renderTarget.GetQueryArgument?.Invoke(instanceScope.Instance) ?? instanceScope.Instance.QueryArgument;
                var queryable = invoke(queryArgument);
                var output = renderTarget.Render(queryable, instanceScope.Instance);
                targets[renderTarget.Id] = new RenderedTarget(renderTarget.Label, renderTarget.OutputLanguage, output);
            }
            catch (Exception ex)
            {
                targets[renderTarget.Id] = new RenderedTarget(
                    renderTarget.Label,
                    renderTarget.OutputLanguage,
                    FormatErrorMessage(ex),
                    IsError: true);
            }
        }

        // Prerenderer-only providers: fresh DbContexts / Mongo roots per sample.
        // Their client libraries throw on WASM so they can't live in Core.
        if (scenario.Id == "webshop")
        {
            using var sqlServer = BuildSqlServerContext();
            using var cosmos = BuildCosmosContext();
            RenderPrerendererTarget(targets, invoke, "sqlserver", "EF Core + SQL Server", "sql",
                new DbContextRoots(sqlServer), static (q, _) => q.ToQueryString());
            RenderPrerendererTarget(targets, invoke, "cosmos", "EF Core + Cosmos DB", "sql",
                new DbContextRoots(cosmos), static (q, _) => q.ToQueryString());
            RenderPrerendererTarget(targets, invoke, "mongodb", "MongoDB", "javascript",
                BuildMongoRoots(), static (q, _) => FormatMongoOutput(q.ToString()!));
        }

        // Generator output
        var generatorOutput = FormatGeneratorOutput(result.GeneratedSources);
        targets["generator"] = new RenderedTarget("Generator output", "csharp", generatorOutput);

        return new RenderedSample(sample.StableKey, formatted, sample.Setup, targets);
    }

    private static void RenderPrerendererTarget(
        Dictionary<string, RenderedTarget> targets,
        Func<object, IQueryable> invoke,
        string id, string label, string language,
        object queryArgument,
        Func<IQueryable, IScenarioInstance?, string> render)
    {
        try
        {
            var queryable = invoke(queryArgument);
            targets[id] = new RenderedTarget(label, language, render(queryable, null));
        }
        catch (Exception ex)
        {
            targets[id] = new RenderedTarget(label, language,
                FormatErrorMessage(ex),
                IsError: true);
        }
    }

    // Extracts a clean, readable error message from the exception chain.
    // Strips EF Core's verbose LINQ expression dump and leaves just the
    // "could not be translated" reason plus the fwlink hint if present.
    private static string FormatErrorMessage(Exception ex)
    {
        // Unwrap TargetInvocationException and TypeInitializationException —
        // they add no signal, the inner message is the real reason the query
        // could not be translated.
        while (ex is System.Reflection.TargetInvocationException or TypeInitializationException && ex.InnerException is not null)
            ex = ex.InnerException;

        if (Environment.GetEnvironmentVariable("PRERENDERER_VERBOSE") == "1")
            Console.Error.WriteLine($"\n--- Exception from {ex.GetType().Name} ---\n{ex}\n---");

        var msg = ex.Message;
        // EF Core often prepends "The LINQ expression '...' could not be translated."
        // followed by "Additional information: ..." with the real reason.
        var additionalIdx = msg.IndexOf("Additional information:", StringComparison.Ordinal);
        if (additionalIdx >= 0)
        {
            var reason = msg[(additionalIdx + "Additional information:".Length)..].Trim();
            // Trim the fwlink footer too
            var fwlinkIdx = reason.IndexOf("See https://", StringComparison.Ordinal);
            if (fwlinkIdx >= 0) reason = reason[..fwlinkIdx].Trim();
            return reason;
        }
        return msg;
    }

    // Synchronously disposes the scenario instance at end-of-scope so the
    // DbContext underlying the in-scenario render targets (SQLite, Postgres
    // in WebshopScenarioInstance) is torn down before the next sample runs.
    private readonly struct ScenarioInstanceScope : IDisposable
    {
        public IScenarioInstance Instance { get; }

        public ScenarioInstanceScope(IPlaygroundScenario scenario)
        {
            Instance = scenario.CreateInstance();
        }

        public void Dispose()
        {
            try { Instance.DisposeAsync().AsTask().GetAwaiter().GetResult(); }
            catch { /* best-effort */ }
        }
    }

    // Formats a C# expression across multiple lines:
    //  1. SnippetFormatter breaks outer fluent chains (.Where().Select() → one per line)
    //  2. A custom syntax rewriter breaks switch expression arms onto their own lines
    //  3. Roslyn's Formatter normalizes indentation
    private static string FormatSnippet(string snippet)
    {
        var preformatted = SnippetFormatter.Format(snippet);
        var wrapped = "_ = " + preformatted + ";";
        var root = CSharpSyntaxTree.ParseText(wrapped).GetRoot();

        // Break switch expression arms onto separate lines
        root = new SwitchExpressionBreaker().Visit(root);

        using var workspace = new Microsoft.CodeAnalysis.AdhocWorkspace();
        var formatted = Formatter.Format(root, workspace).ToFullString().Trim();

        // Collapse accidental blank lines the rewriter may introduce
        formatted = System.Text.RegularExpressions.Regex.Replace(formatted, @"\n\s*\n", "\n");

        if (formatted.StartsWith("_ = ", StringComparison.Ordinal))
            formatted = formatted[4..];
        if (formatted.EndsWith(";", StringComparison.Ordinal))
            formatted = formatted[..^1];
        return formatted.Trim();
    }

    private sealed class SwitchExpressionBreaker : Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter
    {
        public override Microsoft.CodeAnalysis.SyntaxNode? VisitSwitchExpression(
            Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax node)
        {
            var visited = (Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax)base.VisitSwitchExpression(node)!;
            var newline = Microsoft.CodeAnalysis.CSharp.SyntaxFactory.EndOfLine("\n");

            // Put each arm on its own line by injecting a newline before each arm's leading trivia
            var newArms = visited.Arms.Select(arm =>
                arm.WithLeadingTrivia(arm.GetLeadingTrivia().Insert(0, newline)));

            return visited
                .WithOpenBraceToken(visited.OpenBraceToken.WithTrailingTrivia(newline))
                .WithArms(Microsoft.CodeAnalysis.CSharp.SyntaxFactory.SeparatedList(newArms, visited.Arms.GetSeparators()))
                .WithCloseBraceToken(visited.CloseBraceToken.WithLeadingTrivia(newline));
        }
    }

    // Pretty-prints a MongoDB aggregation pipeline. Raw output is a single line:
    //   collection.Aggregate([{ "$match" : {...} }, { "$project" : {...} }])
    // Output expands each stage and the BSON content inside each stage.
    private static string FormatMongoOutput(string raw)
    {
        var openIdx = raw.IndexOf("Aggregate([", StringComparison.Ordinal);
        if (openIdx < 0) return raw;
        var prefix = raw[..(openIdx + "Aggregate(".Length)];
        var closeIdx = raw.LastIndexOf("])", StringComparison.Ordinal);
        if (closeIdx < 0) return raw;
        var stagesJson = raw.Substring(openIdx + "Aggregate([".Length, closeIdx - openIdx - "Aggregate([".Length);
        var suffix = raw[(closeIdx + 1)..];

        var stages = SplitTopLevel(stagesJson);

        var sb = new System.Text.StringBuilder();
        sb.Append(prefix).AppendLine("[");
        for (var i = 0; i < stages.Count; i++)
        {
            var stage = stages[i].Trim();
            var pretty = PrettyPrintBson(stage, indentDepth: 1);
            sb.Append("    ").Append(pretty);
            if (i < stages.Count - 1) sb.Append(',');
            sb.AppendLine();
        }
        sb.Append(']').Append(suffix);
        return sb.ToString();
    }

    // Pretty-prints relaxed JSON (Mongo-style with spaces around colons).
    // Breaks objects and arrays onto multiple lines when they contain nested
    // objects/arrays; keeps short primitive-only objects inline.
    private static string PrettyPrintBson(string json, int indentDepth)
    {
        var sb = new System.Text.StringBuilder();
        var depth = indentDepth;
        var inString = false;
        for (var i = 0; i < json.Length; i++)
        {
            var c = json[i];
            if (c == '"' && (i == 0 || json[i - 1] != '\\')) inString = !inString;

            if (inString)
            {
                sb.Append(c);
                continue;
            }

            if (c == '{' || c == '[')
            {
                sb.Append(c);
                // Check if contents are simple (no nested objects/arrays)
                var closing = c == '{' ? '}' : ']';
                var end = FindMatchingBrace(json, i, c, closing);
                var inner = end > i ? json[(i + 1)..end] : "";
                if (end > i && !ContainsNested(inner))
                {
                    // Keep short object/array inline
                    sb.Append(inner).Append(closing);
                    i = end;
                    continue;
                }
                depth++;
                sb.AppendLine();
                sb.Append(new string(' ', depth * 4));
            }
            else if (c == '}' || c == ']')
            {
                depth--;
                sb.AppendLine();
                sb.Append(new string(' ', depth * 4));
                sb.Append(c);
            }
            else if (c == ',')
            {
                sb.Append(',');
                sb.AppendLine();
                sb.Append(new string(' ', depth * 4));
                // Skip the following space if present
                if (i + 1 < json.Length && json[i + 1] == ' ') i++;
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    private static int FindMatchingBrace(string s, int start, char open, char close)
    {
        var depth = 0;
        var inString = false;
        for (var i = start; i < s.Length; i++)
        {
            var c = s[i];
            if (c == '"' && (i == 0 || s[i - 1] != '\\')) inString = !inString;
            if (inString) continue;
            if (c == open) depth++;
            else if (c == close)
            {
                depth--;
                if (depth == 0) return i;
            }
        }
        return -1;
    }

    private static bool ContainsNested(string s)
    {
        var inString = false;
        for (var i = 0; i < s.Length; i++)
        {
            var c = s[i];
            if (c == '"' && (i == 0 || s[i - 1] != '\\')) inString = !inString;
            if (inString) continue;
            if (c == '{' || c == '[') return true;
        }
        return false;
    }

    private static List<string> SplitTopLevel(string s)
    {
        var parts = new List<string>();
        var depth = 0;
        var start = 0;
        var inString = false;
        for (var i = 0; i < s.Length; i++)
        {
            var c = s[i];
            if (c == '"' && (i == 0 || s[i - 1] != '\\')) inString = !inString;
            if (inString) continue;
            if (c == '{' || c == '[') depth++;
            else if (c == '}' || c == ']') depth--;
            else if (c == ',' && depth == 0)
            {
                parts.Add(s[start..i]);
                start = i + 1;
            }
        }
        parts.Add(s[start..]);
        return parts;
    }

    private static string FormatGeneratorOutput(IReadOnlyList<GeneratedSource> sources)
    {
        if (sources.Count == 0)
            return "// (no generator output for this snippet)";

        var sb = new System.Text.StringBuilder();
        for (var i = 0; i < sources.Count; i++)
        {
            if (i > 0) sb.AppendLine().AppendLine();
            sb.Append("// === ").Append(sources[i].HintName).AppendLine(" ===");
            sb.Append(sources[i].Source);
        }
        return sb.ToString();
    }

    // All per-sample state is now disposed in Render() itself; nothing to
    // clean up at the renderer level.
    public ValueTask DisposeAsync() => default;
}
