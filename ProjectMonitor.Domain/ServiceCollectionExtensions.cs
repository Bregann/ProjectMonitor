using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectMonitor.Domain.Alerts;
using ProjectMonitor.Domain.Configuration;
using ProjectMonitor.Domain.Data;
using ProjectMonitor.Domain.GitHub;
using ProjectMonitor.Domain.State;
using ProjectMonitor.Domain.Uptime;

namespace ProjectMonitor.Domain
{
    public static class ServiceCollectionExtensions
    {
        /// <param name="connectionString">PostgreSQL connection string. When null, an in-memory database is used (local development only).</param>
        public static IServiceCollection AddProjectMonitor(this IServiceCollection services, IConfiguration configuration, string? connectionString)
        {
            services.Configure<MonitorOptions>(configuration.GetSection(MonitorOptions.SectionName));

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                services.AddDbContext<MonitorDbContext>(o => o.UseInMemoryDatabase("ProjectMonitor"));
            }
            else
            {
                services.AddDbContext<MonitorDbContext>(o => o.UseNpgsql(connectionString));
            }

            services.AddSingleton<DashboardState>();

            services.AddHttpClient<IGitHubClient, GitHubClient>(GitHubClient.Configure);
            services.AddHttpClient(UptimeProbe.HttpClientName, c => c.DefaultRequestHeaders.UserAgent.ParseAdd("ProjectMonitor-Uptime"));
            services.AddHttpClient(AlertService.HttpClientName, c => c.Timeout = TimeSpan.FromSeconds(15));
            services.AddSingleton<IUptimeProbe, UptimeProbe>();

            services.AddHostedService<GitHubPollingService>();
            services.AddHostedService<UptimeMonitorService>();
            services.AddHostedService<AlertService>();

            return services;
        }
    }
}
