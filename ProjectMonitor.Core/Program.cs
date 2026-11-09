using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using ProjectMonitor.Core;
using ProjectMonitor.Core.Components;
using ProjectMonitor.Domain.Database.Context;
using ProjectMonitor.Domain.DTOs.Configuration;
using ProjectMonitor.Domain.Helpers;
using ProjectMonitor.Domain.Interfaces.Api.Dashboard;
using ProjectMonitor.Domain.Interfaces.Helpers;
using ProjectMonitor.Domain.Interfaces.Jobs;
using ProjectMonitor.Domain.Services.Alerts;
using ProjectMonitor.Domain.Services.Dashboard;
using ProjectMonitor.Domain.Services.GitHub;
using ProjectMonitor.Domain.Services.Uptime;
using Serilog;
using Serilog.Events;

#if DEBUG
using Hangfire.MemoryStorage;
using Testcontainers.PostgreSql;
#else
using Hangfire.PostgreSql;
#endif

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
    .MinimumLevel.Override("Hangfire", LogEventLevel.Warning)
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Logger Setup");

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.Configure<MonitorConfig>(builder.Configuration.GetSection(MonitorConfig.SectionName));
MonitorConfig monitorConfig = builder.Configuration.GetSection(MonitorConfig.SectionName).Get<MonitorConfig>() ?? new();

// Front end
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();

// Our own services
builder.Services.AddSingleton<IEnvironmentalSettingHelper, EnvironmentalSettingHelper>();
builder.Services.AddSingleton<IDashboardStateHelper, DashboardStateHelper>();
builder.Services.AddSingleton(new FailureStreakTracker(monitorConfig.FailuresBeforeDown));
builder.Services.AddSingleton<AlertTracker>();
builder.Services.AddHttpClient<IGitHubApiHelper, GitHubApiHelper>(GitHubApiHelper.ConfigureHttpClient);
builder.Services.AddHttpClient<IUptimeProbeHelper, UptimeProbeHelper>(c => c.Timeout = TimeSpan.FromSeconds(monitorConfig.UptimeTimeoutSeconds));
builder.Services.AddHttpClient<ICommsSenderClient, CommsSenderClient>(c => c.Timeout = TimeSpan.FromSeconds(15));

builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IUptimeService, UptimeService>();
builder.Services.AddScoped<IGitHubService, GitHubService>();
builder.Services.AddScoped<IAlertService, AlertService>();

// Auth: one password, kept signed in for 30 days
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

builder.Services.AddDataProtection()
    .SetApplicationName("ProjectMonitor")
    .PersistKeysToDbContext<AppDbContext>();

// The container sits behind a reverse proxy that terminates HTTPS
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

#if DEBUG
GlobalConfiguration.Configuration.UseMemoryStorage();

var postgresContainer = new PostgreSqlBuilder("postgres:18")
    .WithDatabase("projectmonitorcontainer")
    .WithUsername("testuser")
    .WithPassword("testpass")
    .WithPortBinding(5432, true)
    .Build();

await postgresContainer.StartAsync();

var connectionString = postgresContainer.GetConnectionString();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseMemoryStorage()
        );
#else
var connectionString = Environment.GetEnvironmentVariable("PMConnString")
    ?? throw new InvalidOperationException("Set the PMConnString environment variable to a PostgreSQL connection string.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

GlobalConfiguration.Configuration.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connectionString));

builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connectionString))
        );
#endif

builder.Services.AddHangfireServer(options => options.SchedulePollingInterval = TimeSpan.FromSeconds(5));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();

#if DEBUG
    // protection to only seed the test container
    if (dbContext.Database.GetConnectionString()?.Contains("projectmonitorcontainer") == true && !await dbContext.EnvironmentalSettings.AnyAsync())
    {
        await DatabaseSeedHelper.SeedDatabase(dbContext);
    }
#endif
}

var environmentalSettingHelper = app.Services.GetRequiredService<IEnvironmentalSettingHelper>();
await environmentalSettingHelper.LoadEnvironmentalSettings();

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

app.MapHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new HangfireDashboardAuthFilter()],
    AppPath = "/"
}, JobStorage.Current);

HangfireJobSetup.SetupRecurringJobs();

app.Run();
