using TaskManagementSystem.Application.DTOs;

namespace TaskManagementSystem.Application.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetAllAsync();
    Task<ProjectDto> GetByIdAsync(int id);
    Task<ProjectDto> CreateAsync(CreateProjectDto dto);
    Task<ProjectDto> UpdateAsync(int id, UpdateProjectDto dto);
    Task DeleteAsync(int id);
}
