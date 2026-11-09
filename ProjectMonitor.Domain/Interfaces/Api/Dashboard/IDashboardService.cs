using ProjectMonitor.Domain.DTOs.Dashboard;

namespace ProjectMonitor.Domain.Interfaces.Api.Dashboard
{
    public interface IDashboardService
    {
        /// <summary>Raised whenever new uptime or GitHub data arrives, so pages can refresh live.</summary>
        event Action? DataChanged;

        AtAGlanceDto GetAtAGlance();
        ProjectDetailsDto? GetProjectDetails(string slug);
        List<NavProjectDto> GetNavProjects();
        DataFreshnessDto GetDataFreshness();
    }
}
