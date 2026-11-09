using Microsoft.EntityFrameworkCore;
using ProjectMonitor.Domain.Database.Context;
using ProjectMonitor.Domain.DTOs.Configuration;
using ProjectMonitor.Domain.Interfaces.Helpers;

namespace ProjectMonitor.Domain.Helpers
{
    public class ProjectConfigHelper(AppDbContext context, IDashboardStateHelper state) : IProjectConfigHelper
    {
        public async Task<List<ProjectConfig>> GetProjects()
        {
            var projects = await context.Projects
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    x.Id,
                    x.Slug,
                    x.Name,
                    x.Description,
                    x.Repository,
                    Endpoints = context.HttpChecks
                        .Where(c => c.ProjectId == x.Id && c.IsEnabled)
                        .OrderBy(c => c.Name)
                        .Select(c => new EndpointConfig { Id = c.Id, Name = c.Name, Url = c.Url, ExpectedStatusCodes = c.ExpectedStatusCodes, FailureLevel = c.FailureLevel })
                        .ToList()
                })
                .ToListAsync();

            return projects.Select(x => new ProjectConfig
            {
                Id = x.Id,
                Slug = x.Slug,
                Name = x.Name,
                Description = x.Description,
                Repository = x.Repository,
                Endpoints = x.Endpoints
            }).ToList();
        }

        public async Task<List<ProjectConfig>> SyncDashboardState()
        {
            List<ProjectConfig> projects = await GetProjects();
            state.SyncProjects(projects);
            return projects;
        }
    }
}
