using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Services;

public class ProjectService(IAppDbContext db) : IProjectService
{
    public async Task<IEnumerable<ProjectDto>> GetAllAsync()
    {
        return await db.Projects
            .Include(p => p.Tasks)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                TaskCount = p.Tasks.Count
            })
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<ProjectDto> GetByIdAsync(int id)
    {
        var project = await db.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new NotFoundException(nameof(Project), id);

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            TaskCount = project.Tasks.Count
        };
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
    {
        var project = new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync();

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            TaskCount = 0
        };
    }

    public async Task<ProjectDto> UpdateAsync(int id, UpdateProjectDto dto)
    {
        var project = await db.Projects.FindAsync(id)
            ?? throw new NotFoundException(nameof(Project), id);

        project.Name = dto.Name;
        project.Description = dto.Description;

        await db.SaveChangesAsync();

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            TaskCount = await db.Tasks.CountAsync(t => t.ProjectId == id)
        };
    }

    public async Task DeleteAsync(int id)
    {
        var project = await db.Projects.FindAsync(id)
            ?? throw new NotFoundException(nameof(Project), id);

        db.Projects.Remove(project);
        await db.SaveChangesAsync();
    }
}
