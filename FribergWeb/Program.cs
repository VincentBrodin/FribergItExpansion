using FribergWeb.Components;
using FribergWeb.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["DownstreamApi:BaseUrl"] ?? "http://localhost:5250") });
builder.Services.AddScoped<CarService>();


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
