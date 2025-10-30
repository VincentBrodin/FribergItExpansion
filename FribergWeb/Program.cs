using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FribergWeb;
using FribergWeb.Services;
using FribergWeb.Providers;
using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5250") });
builder.Services.AddScoped<JwtSecurityTokenHandler>();
builder.Services.AddScoped<ApiAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<ApiAuthStateProvider>());
builder.Services.AddScoped<CarService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddBlazoredLocalStorage();

var host = builder.Build();
var authProvider = host.Services.GetRequiredService<ApiAuthStateProvider>();
await authProvider.GetAuthenticationStateAsync();
await host.RunAsync();
