using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskItemDto>> GetAllAsync(TaskItemStatus? status, Priority? priority, int? projectId);
    Task<TaskItemDto> GetByIdAsync(int id);
    Task<TaskItemDto> CreateAsync(CreateTaskItemDto dto);
    Task<TaskItemDto> UpdateAsync(int id, UpdateTaskItemDto dto);
    Task DeleteAsync(int id);
}
