using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using ProjectMonitor.Core.Components;
using ProjectMonitor.Domain;
using ProjectMonitor.Domain.Configuration;
using ProjectMonitor.Domain.Data;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
    .WriteTo.Console()
    .CreateLogger();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Same env var style as the other projects; ConnectionStrings__Monitor also works
string? connectionString = builder.Configuration.GetConnectionString("Monitor") ?? Environment.GetEnvironmentVariable("PMConnString");

if (string.IsNullOrWhiteSpace(connectionString))
{
    if (!builder.Environment.IsDevelopment())
    {
        throw new InvalidOperationException("Set the PMConnString environment variable to a PostgreSQL connection string.");
    }

    Log.Warning("No PMConnString set: using an in-memory database, so uptime history is lost on restart");
}

MonitorOptions monitorOptions = builder.Configuration.GetSection(MonitorOptions.SectionName).Get<MonitorOptions>() ?? new();

if (string.IsNullOrWhiteSpace(monitorOptions.DashboardPassword) && !builder.Environment.IsDevelopment())
{
    throw new InvalidOperationException("Set the Monitor__DashboardPassword environment variable: the dashboard shows private project details.");
}

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddProjectMonitor(builder.Configuration, connectionString);
builder.Services.AddDataProtection()
    .SetApplicationName("ProjectMonitor")
    .PersistKeysToDbContext<MonitorDbContext>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.Cookie.Name = "ProjectMonitor.Auth";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// The container sits behind a reverse proxy that terminates HTTPS
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

WebApplication app = builder.Build();

await using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
{
    MonitorDbContext db = scope.ServiceProvider.GetRequiredService<MonitorDbContext>();

    if (db.Database.IsRelational())
    {
        await db.Database.MigrateAsync();
    }
    else
    {
        await db.Database.EnsureCreatedAsync();
    }
}

app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapGet("/healthz", () => Results.Ok("ok")).AllowAnonymous();
app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
