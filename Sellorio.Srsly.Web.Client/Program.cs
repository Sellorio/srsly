using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Sellorio.Blazor.Components;
using Sellorio.Clients.Rest;
using Sellorio.Srsly.Client;
using Sellorio.Srsly.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAoServices();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(serviceProvider => serviceProvider.GetRequiredService<JwtAuthenticationStateProvider>());
builder.Services.AddScoped<IRestClientAuthorizationProvider>(serviceProvider => serviceProvider.GetRequiredService<JwtAuthenticationStateProvider>());
builder.Services.AddScoped<AuthenticationHeaderHandler>();
builder.Services.AddHttpClient("ServerApi", o => o.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();
builder.Services.AddScoped(serviceProvider => serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("ServerApi"));
builder.Services.TryAddRestClient<IAuthenticationService, AuthenticationService>();
builder.Services.AddSrslyClientSideServices();

await builder.Build().RunAsync();
