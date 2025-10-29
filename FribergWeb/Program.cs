using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;
using FribergWeb.Components;
using FribergWeb.Providers;
using FribergWeb.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["DownstreamApi:BaseUrl"] ?? "http://localhost:5250") });

builder.Services.AddScoped<JwtSecurityTokenHandler>();
builder.Services.AddScoped<CarService>();
builder.Services.AddScoped<ApiAuthStateProvider>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddBlazoredLocalStorage();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
