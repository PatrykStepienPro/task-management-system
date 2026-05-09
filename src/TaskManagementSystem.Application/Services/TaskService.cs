using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Services;

public class TaskService(IAppDbContext db) : ITaskService
{
    public async Task<IEnumerable<TaskItemDto>> GetAllAsync(TaskItemStatus? status, Priority? priority, int? projectId)
    {
        var query = db.Tasks.Include(t => t.Project).AsQueryable();

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (priority.HasValue)
            query = query.Where(t => t.Priority == priority.Value);

        if (projectId.HasValue)
            query = query.Where(t => t.ProjectId == projectId.Value);

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => ToDto(t))
            .ToListAsync();
    }

    public async Task<TaskItemDto> GetByIdAsync(int id)
    {
        var task = await db.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new NotFoundException(nameof(TaskItem), id);

        return ToDto(task);
    }

    public async Task<TaskItemDto> CreateAsync(CreateTaskItemDto dto)
    {
        _ = await db.Projects.FindAsync(dto.ProjectId)
            ?? throw new NotFoundException(nameof(Project), dto.ProjectId);

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            Status = dto.Status,
            DueDate = dto.DueDate,
            ProjectId = dto.ProjectId,
            CreatedAt = DateTime.UtcNow
        };

        db.Tasks.Add(task);
        await db.SaveChangesAsync();

        return await GetByIdAsync(task.Id);
    }

    public async Task<TaskItemDto> UpdateAsync(int id, UpdateTaskItemDto dto)
    {
        var task = await db.Tasks.FindAsync(id)
            ?? throw new NotFoundException(nameof(TaskItem), id);

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Priority = dto.Priority;
        task.Status = dto.Status;
        task.DueDate = dto.DueDate;

        await db.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var task = await db.Tasks.FindAsync(id)
            ?? throw new NotFoundException(nameof(TaskItem), id);

        db.Tasks.Remove(task);
        await db.SaveChangesAsync();
    }

    private static TaskItemDto ToDto(TaskItem t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        Priority = t.Priority,
        Status = t.Status,
        CreatedAt = t.CreatedAt,
        DueDate = t.DueDate,
        ProjectId = t.ProjectId,
        ProjectName = t.Project?.Name ?? string.Empty
    };
}
