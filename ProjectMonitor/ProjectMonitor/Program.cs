using BreganUtils.ProjectMonitor;
using Hangfire;
using Hangfire.Dashboard.Dark;
using Hangfire.PostgreSql;
using ProjectMonitor.Api;
using Serilog;

//Create logger
Log.Logger = new LoggerConfiguration().WriteTo.Async(x => x.File("Logs/log.log", retainedFileCountLimit: 7, rollingInterval: RollingInterval.Day)).WriteTo.Console().CreateLogger();

AppConfig.LoadConfig();

#if DEBUG
ProjectMonitorConfig.SetupMonitor("debug", AppConfig.ApiKey);
#else
ProjectMonitorConfig.SetupMonitor("release", AppConfig.ApiKey);
#endif

ProjectMonitorCommon.ReportProjectUp("Project Monitor");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
JobStorage.Current = new PostgreSqlStorage(AppConfig.HFConnectionString, new PostgreSqlStorageOptions { SchemaName = "projectmonitor" });

builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(AppConfig.HFConnectionString, new PostgreSqlStorageOptions { SchemaName = "projectmonitor" })
        .UseDarkDashboard()
);

builder.Services.AddHangfireServer(options => options.SchedulePollingInterval = TimeSpan.FromSeconds(5));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "allowUrls",
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000", "https://dashboard.bregan.me");
                          policy.WithHeaders("Content-Type");
                          policy.WithMethods("GET", "POST", "DELETE");
                      });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

HangfireJobs.SetupHangfireJobs();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();