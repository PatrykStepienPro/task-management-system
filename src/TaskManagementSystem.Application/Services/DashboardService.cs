using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Services;

public class DashboardService(IAppDbContext db) : IDashboardService
{
    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        var now = DateTime.UtcNow;

        var totalProjects = await db.Projects.CountAsync();
        var tasks = await db.Tasks.Include(t => t.Project).ToListAsync();

        var overdue = tasks.Count(t =>
            t.DueDate.HasValue &&
            t.DueDate.Value < now &&
            t.Status != TaskItemStatus.Done &&
            t.Status != TaskItemStatus.Cancelled);

        var tasksPerProject = tasks
            .GroupBy(t => t.Project.Name)
            .Select(g => new ProjectTaskCountDto
            {
                ProjectName = g.Key,
                TaskCount = g.Count()
            })
            .OrderByDescending(x => x.TaskCount)
            .ToList();

        return new DashboardStatsDto
        {
            TotalProjects = totalProjects,
            TotalTasks = tasks.Count,
            TasksTodo = tasks.Count(t => t.Status == TaskItemStatus.Todo),
            TasksInProgress = tasks.Count(t => t.Status == TaskItemStatus.InProgress),
            TasksDone = tasks.Count(t => t.Status == TaskItemStatus.Done),
            TasksCancelled = tasks.Count(t => t.Status == TaskItemStatus.Cancelled),
            OverdueTasks = overdue,
            TasksPerProject = tasksPerProject
        };
    }
}
