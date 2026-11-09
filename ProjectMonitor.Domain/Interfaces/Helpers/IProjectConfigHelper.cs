using ProjectMonitor.Domain.DTOs.Configuration;

namespace ProjectMonitor.Domain.Interfaces.Helpers
{
    public interface IProjectConfigHelper
    {
        /// <summary>Loads every project and its enabled website/API checks from the database.</summary>
        Task<List<ProjectConfig>> GetProjects();

        /// <summary>Loads the projects and updates the dashboard's in-memory list to match.</summary>
        Task<List<ProjectConfig>> SyncDashboardState();
    }
}
