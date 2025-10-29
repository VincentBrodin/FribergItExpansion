using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FribergWeb;
using FribergWeb.Services;
using FribergWeb.Providers;
using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5250") });
builder.Services.AddScoped<JwtSecurityTokenHandler>();
builder.Services.AddScoped<ApiAuthStateProvider>();
builder.Services.AddScoped<CarService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
