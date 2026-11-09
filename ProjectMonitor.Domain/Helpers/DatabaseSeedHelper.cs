using ProjectMonitor.Domain.Database.Context;
using ProjectMonitor.Domain.Database.Models;
using ProjectMonitor.Domain.Enums;

namespace ProjectMonitor.Domain.Helpers
{
    /// <summary>DEBUG only: fills the test container with settings so the app runs locally.</summary>
    public class DatabaseSeedHelper
    {
        public static async Task SeedDatabase(AppDbContext context)
        {
            Dictionary<EnvironmentalSettingEnum, string> settings = new()
            {
                [EnvironmentalSettingEnum.DashboardPassword] = "password",

                // Optional: export PMGitHubToken before running to see real alerts locally (Actions/PRs work without it)
                [EnvironmentalSettingEnum.GitHubToken] = Environment.GetEnvironmentVariable("PMGitHubToken") ?? ""
            };

            context.EnvironmentalSettings.AddRange(settings.Select(x => new EnvironmentalSetting { Key = x.Key.ToString(), Value = x.Value }));
            await context.SaveChangesAsync();
        }
    }
}
