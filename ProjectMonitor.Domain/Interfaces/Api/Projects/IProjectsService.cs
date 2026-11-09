using ProjectMonitor.Domain.DTOs.Projects;
using ProjectMonitor.Domain.DTOs.Projects.Requests;

namespace ProjectMonitor.Domain.Interfaces.Api.Projects
{
    public interface IProjectsService
    {
        Task<List<ManageProjectDto>> GetProjects();
        Task<SaveResult> AddProject(SaveProjectRequest request);
        Task<SaveResult> UpdateProject(int projectId, SaveProjectRequest request);
        Task DeleteProject(int projectId);
        Task<string?> RegenerateApiKey(int projectId);
        Task<SaveResult> AddCheck(int projectId, SaveCheckRequest request);
        Task<SaveResult> UpdateCheck(int checkId, SaveCheckRequest request);
        Task DeleteCheck(int checkId);
    }
}
