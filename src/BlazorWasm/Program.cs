using BlazorWasm;
using BlazorWasm.Clients;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorWasm.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddTransient<CredentialsMessageHandler>();
builder.Services.AddRefitClient<IIdentityClient>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri("http://localhost:5279");
    });

builder.Services.AddRefitClient<INotesClient>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri("http://localhost:5279");
    })
    .AddHttpMessageHandler<CredentialsMessageHandler>();

builder.Services.AddRefitClient<IApiKeyClient>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri("http://localhost:5279");
    })
    .AddHttpMessageHandler<CredentialsMessageHandler>();

builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton<StateProvider>();
builder.Services.AddSingleton<AuthenticationStateProvider>(s => s.GetRequiredService<StateProvider>());
builder.Services.AddMudServices();

await builder.Build().RunAsync();
