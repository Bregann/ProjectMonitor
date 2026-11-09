using Hangfire;
using Microsoft.EntityFrameworkCore;
using ProjectMonitor.Domain.Database.Context;
using ProjectMonitor.Domain.Database.Models;
using ProjectMonitor.Domain.DTOs.Projects;
using ProjectMonitor.Domain.DTOs.Projects.Requests;
using ProjectMonitor.Domain.Enums;
using ProjectMonitor.Domain.Helpers;
using ProjectMonitor.Domain.Interfaces.Api.Projects;
using ProjectMonitor.Domain.Interfaces.Helpers;
using ProjectMonitor.Domain.Interfaces.Jobs;
using Serilog;

namespace ProjectMonitor.Domain.Services.Projects
{
    public class ProjectsService(AppDbContext context, IProjectConfigHelper projectConfigHelper, IBackgroundJobClient backgroundJobs) : IProjectsService
    {
        public async Task<List<ManageProjectDto>> GetProjects()
        {
            List<Project> projects = await context.Projects
                .AsNoTracking()
                .Include(x => x.Checks)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return projects.Select(p => new ManageProjectDto
            {
                Id = p.Id,
                Slug = p.Slug,
                Name = p.Name,
                Description = p.Description,
                Repository = p.Repository,
                ApiKey = p.ApiKey,
                Checks = p.Checks.OrderBy(c => c.CheckType).ThenBy(c => c.Name).Select(ToCheckDto).ToList()
            }).ToList();
        }

        public async Task<SaveResult> AddProject(SaveProjectRequest request)
        {
            string? error = ValidateProject(request);

            if (error is not null)
            {
                return SaveResult.Fail(error);
            }

            if (await context.Projects.AnyAsync(x => x.Name == request.Name.Trim()))
            {
                return SaveResult.Fail("There's already a project with that name.");
            }

            Project project = new()
            {
                Slug = await UniqueSlug(ProjectKeyHelper.ToSlug(request.Name)),
                Name = request.Name.Trim(),
                Description = Clean(request.Description),
                Repository = Clean(request.Repository),
                ApiKey = ProjectKeyHelper.GenerateApiKey(),
                CreatedAtUtc = DateTime.UtcNow
            };

            context.Projects.Add(project);
            await context.SaveChangesAsync();

            Log.Information("[Projects] Added {Project}", project.Name);
            await RefreshData();
            return SaveResult.Ok(project.Id);
        }

        public async Task<SaveResult> UpdateProject(int projectId, SaveProjectRequest request)
        {
            string? error = ValidateProject(request);

            if (error is not null)
            {
                return SaveResult.Fail(error);
            }

            Project? project = await context.Projects.FirstOrDefaultAsync(x => x.Id == projectId);

            if (project is null)
            {
                return SaveResult.Fail("That project no longer exists.");
            }

            if (await context.Projects.AnyAsync(x => x.Id != projectId && x.Name == request.Name.Trim()))
            {
                return SaveResult.Fail("There's already a project with that name.");
            }

            project.Name = request.Name.Trim();
            project.Description = Clean(request.Description);
            project.Repository = Clean(request.Repository);
            await context.SaveChangesAsync();

            await RefreshData();
            return SaveResult.Ok(project.Id);
        }

        public async Task DeleteProject(int projectId)
        {
            int deleted = await context.Projects.Where(x => x.Id == projectId).ExecuteDeleteAsync();

            if (deleted > 0)
            {
                Log.Information("[Projects] Deleted project {ProjectId}", projectId);
                await projectConfigHelper.SyncDashboardState();
            }
        }

        public async Task<string?> RegenerateApiKey(int projectId)
        {
            Project? project = await context.Projects.FirstOrDefaultAsync(x => x.Id == projectId);

            if (project is null)
            {
                return null;
            }

            project.ApiKey = ProjectKeyHelper.GenerateApiKey();
            await context.SaveChangesAsync();

            Log.Information("[Projects] Regenerated the API key for {Project}", project.Name);
            return project.ApiKey;
        }

        public async Task<SaveResult> AddCheck(int projectId, SaveCheckRequest request)
        {
            if (!await context.Projects.AnyAsync(x => x.Id == projectId))
            {
                return SaveResult.Fail("That project no longer exists.");
            }

            string? error = ValidateCheck(request);

            if (error is not null)
            {
                return SaveResult.Fail(error);
            }

            ProjectCheck check = CreateCheck(request);
            check.ProjectId = projectId;

            context.ProjectChecks.Add(check);
            await context.SaveChangesAsync();

            await RefreshData();
            return SaveResult.Ok(check.Id);
        }

        public async Task<SaveResult> UpdateCheck(int checkId, SaveCheckRequest request)
        {
            ProjectCheck? existing = await context.ProjectChecks.FirstOrDefaultAsync(x => x.Id == checkId);

            if (existing is null)
            {
                return SaveResult.Fail("That check no longer exists.");
            }

            if (existing.CheckType != request.Type)
            {
                return SaveResult.Fail("A check's type can't be changed. Delete it and add a new one instead.");
            }

            string? error = ValidateCheck(request);

            if (error is not null)
            {
                return SaveResult.Fail(error);
            }

            ApplyFields(existing, request);
            await context.SaveChangesAsync();

            await RefreshData();
            return SaveResult.Ok(existing.Id);
        }

        public async Task DeleteCheck(int checkId)
        {
            await context.ProjectChecks.Where(x => x.Id == checkId).ExecuteDeleteAsync();
            await RefreshData();
        }

        /// <summary>Update the dashboard straight away and re-run the jobs, so changes show without waiting for the schedule.</summary>
        private async Task RefreshData()
        {
            await projectConfigHelper.SyncDashboardState();
            backgroundJobs.Enqueue<IUptimeService>(x => x.CheckEndpoints());
            backgroundJobs.Enqueue<IGitHubService>(x => x.PollRepositories());
            backgroundJobs.Enqueue<IChecksService>(x => x.EvaluateAll());
        }

        private async Task<string> UniqueSlug(string slug)
        {
            string candidate = slug;
            int suffix = 2;

            while (await context.Projects.AnyAsync(x => x.Slug == candidate))
            {
                candidate = $"{slug}-{suffix++}";
            }

            return candidate;
        }

        public static string? ValidateProject(SaveProjectRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return "Give the project a name.";
            }

            if (request.Name.Trim().Length > 100)
            {
                return "The name can be at most 100 characters.";
            }

            if (request.Repository is { Length: > 0 } repo && (repo.Contains('/') || repo.Contains(' ')))
            {
                return "Enter just the repository name (e.g. \"Orbit\"), not the owner or a URL.";
            }

            return null;
        }

        public static string? ValidateCheck(SaveCheckRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return "Give the check a name.";
            }

            if (request.FailureLevel is not (HealthLevel.Warning or HealthLevel.Critical))
            {
                return "A failing check must be either a warning or critical.";
            }

            switch (request.Type)
            {
                case CheckTypeEnum.Http:
                    if (!Uri.TryCreate(request.Url?.Trim(), UriKind.Absolute, out Uri? uri) || uri.Scheme is not ("http" or "https"))
                    {
                        return "Enter a full URL starting with http:// or https://.";
                    }

                    if (request.ExpectedStatusCodes.Any(x => x is < 100 or > 599))
                    {
                        return "Status codes must be between 100 and 599.";
                    }

                    return null;

                case CheckTypeEnum.Heartbeat:
                    return request.MaxAgeMinutes is null or < 1 ? "Enter how many minutes the project can go without reporting." : null;

                case CheckTypeEnum.Value:
                    if (string.IsNullOrWhiteSpace(request.ValueKey))
                    {
                        return "Enter the name of the value to check (the key the project reports).";
                    }

                    if (request.Condition is ValueConditionEnum.EqualTo or ValueConditionEnum.NotEqualTo && string.IsNullOrWhiteSpace(request.ExpectedValue))
                    {
                        return "Enter the value to compare against.";
                    }

                    if (request.Condition is ValueConditionEnum.GreaterThan or ValueConditionEnum.LessThan or ValueConditionEnum.WithinMinutes
                        && !double.TryParse(request.ExpectedValue, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _))
                    {
                        return request.Condition == ValueConditionEnum.WithinMinutes ? "Enter the number of minutes." : "Enter a number to compare against.";
                    }

                    return request.MaxAgeMinutes is < 1 ? "The reporting time limit must be at least 1 minute." : null;

                default:
                    return "Unknown check type.";
            }
        }

        private static ProjectCheck CreateCheck(SaveCheckRequest request)
        {
            ProjectCheck check = request.Type switch
            {
                CheckTypeEnum.Http => new HttpCheck { Name = "", Url = "" },
                CheckTypeEnum.Heartbeat => new HeartbeatCheck { Name = "" },
                CheckTypeEnum.Value => new ValueCheck { Name = "", ValueKey = "" },
                _ => throw new ArgumentOutOfRangeException(nameof(request), request.Type, "Unknown check type")
            };

            ApplyFields(check, request);
            return check;
        }

        private static void ApplyFields(ProjectCheck check, SaveCheckRequest request)
        {
            check.Name = request.Name.Trim();
            check.FailureLevel = request.FailureLevel;
            check.IsEnabled = request.IsEnabled;

            switch (check)
            {
                case HttpCheck http:
                    http.Url = request.Url!.Trim();
                    http.ExpectedStatusCodes = request.ExpectedStatusCodes.Distinct().Order().ToList();
                    break;
                case HeartbeatCheck heartbeat:
                    heartbeat.MaxAgeMinutes = request.MaxAgeMinutes!.Value;
                    break;
                case ValueCheck value:
                    value.ValueKey = request.ValueKey!.Trim();
                    value.Condition = request.Condition;
                    value.ExpectedValue = request.Condition is ValueConditionEnum.IsTrue or ValueConditionEnum.IsFalse ? null : request.ExpectedValue?.Trim();
                    value.MaxAgeMinutes = request.MaxAgeMinutes;
                    break;
            }
        }

        private static CheckDto ToCheckDto(ProjectCheck check)
        {
            CheckDto dto = new()
            {
                Id = check.Id,
                Type = check.CheckType,
                Name = check.Name,
                FailureLevel = check.FailureLevel,
                IsEnabled = check.IsEnabled,
                Summary = CheckDescriptionHelper.Describe(check)
            };

            switch (check)
            {
                case HttpCheck http:
                    dto.Url = http.Url;
                    dto.ExpectedStatusCodes = http.ExpectedStatusCodes;
                    break;
                case HeartbeatCheck heartbeat:
                    dto.MaxAgeMinutes = heartbeat.MaxAgeMinutes;
                    break;
                case ValueCheck value:
                    dto.ValueKey = value.ValueKey;
                    dto.Condition = value.Condition;
                    dto.ExpectedValue = value.ExpectedValue;
                    dto.MaxAgeMinutes = value.MaxAgeMinutes;
                    break;
            }

            return dto;
        }

        private static string? Clean(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
