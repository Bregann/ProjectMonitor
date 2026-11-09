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
using ProjectMonitor.Domain.DTOs.Reports;
using ProjectMonitor.Domain.Helpers;
using ProjectMonitor.Domain.Helpers.Checks;
using ProjectMonitor.Domain.Interfaces.Api.Dashboard;
using ProjectMonitor.Domain.Interfaces.Api.Projects;
using ProjectMonitor.Domain.Interfaces.Api.Reports;
using ProjectMonitor.Domain.Interfaces.Helpers;
using ProjectMonitor.Domain.Interfaces.Jobs;
using ProjectMonitor.Domain.Services.Alerts;
using ProjectMonitor.Domain.Services.Checks;
using ProjectMonitor.Domain.Services.Dashboard;
using ProjectMonitor.Domain.Services.GitHub;
using ProjectMonitor.Domain.Services.Projects;
using ProjectMonitor.Domain.Services.Reports;
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

builder.Services.AddScoped<IProjectConfigHelper, ProjectConfigHelper>();

// One evaluator per check type: add new check types here
builder.Services.AddSingleton<ICheckEvaluator, HeartbeatCheckEvaluator>();
builder.Services.AddSingleton<ICheckEvaluator, ValueCheckEvaluator>();

builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IProjectsService, ProjectsService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IUptimeService, UptimeService>();
builder.Services.AddScoped<IGitHubService, GitHubService>();
builder.Services.AddScoped<IChecksService, ChecksService>();
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

    await DatabaseSeedHelper.SeedDefaultProjects(dbContext);

    // Show the projects straight away, before the first jobs have run
    await scope.ServiceProvider.GetRequiredService<IProjectConfigHelper>().SyncDashboardState();
}

var environmentalSettingHelper = app.Services.GetRequiredService<IEnvironmentalSettingHelper>();
await environmentalSettingHelper.LoadEnvironmentalSettings();

app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

// Friendly error pages for the site only: API callers need the real status codes (401, 400…)
app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/api"),
    branch => branch.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true));

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapGet("/healthz", () => Results.Ok("ok")).AllowAnonymous();

// Projects send values here with their API key (see the README): { "values": { "discordConnected": true } }
app.MapPost("/api/report", async (HttpRequest request, ReportRequest body, IReportService reportService) =>
{
    ReportResultEnum result = await reportService.RecordReport(request.Headers["X-ProjectMonitor-ApiKey"].FirstOrDefault(), body);

    return result switch
    {
        ReportResultEnum.Recorded => Results.Ok(new { recorded = body.Values.Count }),
        ReportResultEnum.InvalidApiKey => Results.Unauthorized(),
        _ => Results.BadRequest(new { message = $"Send up to {ReportService.MaxValuesPerReport} values as strings, numbers or true/false (max {ReportService.MaxValueLength} characters each)." })
    };
}).AllowAnonymous().DisableAntiforgery();
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
