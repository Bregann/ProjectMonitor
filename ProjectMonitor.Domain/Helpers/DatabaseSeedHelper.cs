using Microsoft.EntityFrameworkCore;
using ProjectMonitor.Domain.Database.Context;
using ProjectMonitor.Domain.Database.Models;
using ProjectMonitor.Domain.Enums;

namespace ProjectMonitor.Domain.Helpers
{
    public class DatabaseSeedHelper
    {
        /// <summary>DEBUG only: fills the test container with settings so the app runs locally.</summary>
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

        /// <summary>
        /// Adds the existing projects the first time the app starts with an empty Projects table,
        /// so they don't have to be typed in. After that, everything is managed on the site.
        /// </summary>
        public static async Task SeedDefaultProjects(AppDbContext context)
        {
            if (await context.Projects.AnyAsync())
            {
                return;
            }

            (string Name, string? Description, string Repository, string? ApiUrl)[] defaults =
            [
                ("Orbit", "Finance manager", "Orbit", "https://orbitapi.bregan.me"),
                ("RetroTrack", "RetroAchievements tracker", "RetroTrack", "https://rtapi.bregan.me"),
                ("Clogger", null, "Clogger", "https://cloggerapi.bregan.me"),
                ("CommsSender", "Telegram and push notifications", "CommsSender", "https://commssenderapi.bregan.me"),
                ("ShutterStream", null, "ShutterStream", null),
                ("BreganTwitchBot", null, "BreganTwitchBot", null),
                ("Renovate", "Dependency updates for every project", "renovate-runner", null),
                ("Project Monitor", "This dashboard", "ProjectMonitor", null)
            ];

            foreach ((string name, string? description, string repository, string? apiUrl) in defaults)
            {
                Project project = new()
                {
                    Slug = ProjectKeyHelper.ToSlug(name),
                    Name = name,
                    Description = description,
                    Repository = repository,
                    ApiKey = ProjectKeyHelper.GenerateApiKey(),
                    CreatedAtUtc = DateTime.UtcNow
                };

                if (apiUrl is not null)
                {
                    project.Checks.Add(new HttpCheck { Name = "API", Url = apiUrl, FailureLevel = HealthLevel.Critical });
                }

                context.Projects.Add(project);
            }

            await context.SaveChangesAsync();
        }
    }
}
