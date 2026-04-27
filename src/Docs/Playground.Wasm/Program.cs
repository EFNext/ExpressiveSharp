using ExpressiveSharp.Docs.Playground.Wasm.Components;
using ExpressiveSharp.Docs.Playground.Wasm.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

ManagedSqliteStub.Register();

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.RegisterCustomElement<PlaygroundHost>("expressive-playground");

// BaseAddress must point to the playground's own directory (where _framework/
// lives) so PlaygroundReferences can fetch reference DLLs. On a VitePress page
// HostEnvironment.BaseAddress is the docs site root, not the playground
// subdirectory — detect by checking whether the base ends with /playground/.
var baseAddress = builder.HostEnvironment.BaseAddress;
if (!baseAddress.TrimEnd('/').EndsWith("/_playground", StringComparison.OrdinalIgnoreCase)
    && !baseAddress.TrimEnd('/').EndsWith("/playground", StringComparison.OrdinalIgnoreCase))
    baseAddress = baseAddress.TrimEnd('/') + "/_playground/";
builder.Services.AddSingleton(sp => new HttpClient
{
    BaseAddress = new Uri(baseAddress)
});

builder.Services.AddSingleton<PlaygroundReferences>();
builder.Services.AddSingleton<PlaygroundRuntime>();

var host = builder.Build();

var runtime = host.Services.GetRequiredService<PlaygroundRuntime>();
var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();

var providerRef = DotNetObjectReference.Create(new MonacoLanguageProviderBridge(runtime));
await jsRuntime.InvokeVoidAsync("monacoInterop.registerCompletionProvider", providerRef);
await jsRuntime.InvokeVoidAsync("monacoInterop.registerHoverProvider", providerRef);

await host.RunAsync();

// Bridge object exposed to JS via DotNetObjectReference. Monaco's completion
// and hover providers call back into these [JSInvokable] methods.
internal sealed class MonacoLanguageProviderBridge
{
    private readonly PlaygroundRuntime _runtime;

    public MonacoLanguageProviderBridge(PlaygroundRuntime runtime) => _runtime = runtime;

    [JSInvokable]
    public async Task<MonacoCompletionList?> ProvideCompletionItems(string modelUri, MonacoPosition position)
    {
        if (!_runtime.IsInitialized) return null;
        return await _runtime.LanguageServices.GetCompletionsAsync(modelUri, position);
    }

    [JSInvokable]
    public async Task<MonacoHover?> ProvideHover(string modelUri, MonacoPosition position)
    {
        if (!_runtime.IsInitialized) return null;
        return await _runtime.LanguageServices.GetHoverAsync(modelUri, position);
    }
}
