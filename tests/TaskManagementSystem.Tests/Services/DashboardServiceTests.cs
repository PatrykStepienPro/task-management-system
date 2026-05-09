using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Application.Services;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;
using TaskManagementSystem.Infrastructure.Data;

namespace TaskManagementSystem.Tests.Services;

public class DashboardServiceTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly DashboardService _sut;

    public DashboardServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _sut = new DashboardService(_db);
    }

    [Fact]
    public async Task GetStatsAsync_ReturnsZeros_WhenEmpty()
    {
        var result = await _sut.GetStatsAsync();

        result.TotalProjects.Should().Be(0);
        result.TotalTasks.Should().Be(0);
        result.OverdueTasks.Should().Be(0);
    }

    [Fact]
    public async Task GetStatsAsync_CountsTasksByStatus()
    {
        var project = new Project { Name = "P", CreatedAt = DateTime.UtcNow };
        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        _db.Tasks.AddRange(
            new TaskItem { Title = "T1", Status = TaskItemStatus.Todo, ProjectId = project.Id, CreatedAt = DateTime.UtcNow },
            new TaskItem { Title = "T2", Status = TaskItemStatus.InProgress, ProjectId = project.Id, CreatedAt = DateTime.UtcNow },
            new TaskItem { Title = "T3", Status = TaskItemStatus.Done, ProjectId = project.Id, CreatedAt = DateTime.UtcNow },
            new TaskItem { Title = "T4", Status = TaskItemStatus.Cancelled, ProjectId = project.Id, CreatedAt = DateTime.UtcNow }
        );
        await _db.SaveChangesAsync();

        var result = await _sut.GetStatsAsync();

        result.TotalTasks.Should().Be(4);
        result.TasksTodo.Should().Be(1);
        result.TasksInProgress.Should().Be(1);
        result.TasksDone.Should().Be(1);
        result.TasksCancelled.Should().Be(1);
    }

    [Fact]
    public async Task GetStatsAsync_CountsOverdueTasks()
    {
        var project = new Project { Name = "P", CreatedAt = DateTime.UtcNow };
        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        _db.Tasks.AddRange(
            new TaskItem { Title = "Overdue", Status = TaskItemStatus.Todo, DueDate = DateTime.UtcNow.AddDays(-1), ProjectId = project.Id, CreatedAt = DateTime.UtcNow },
            new TaskItem { Title = "Future", Status = TaskItemStatus.Todo, DueDate = DateTime.UtcNow.AddDays(5), ProjectId = project.Id, CreatedAt = DateTime.UtcNow },
            new TaskItem { Title = "Done-Overdue", Status = TaskItemStatus.Done, DueDate = DateTime.UtcNow.AddDays(-1), ProjectId = project.Id, CreatedAt = DateTime.UtcNow }
        );
        await _db.SaveChangesAsync();

        var result = await _sut.GetStatsAsync();

        result.OverdueTasks.Should().Be(1);
    }

    [Fact]
    public async Task GetStatsAsync_ReturnsTasksPerProject()
    {
        var p1 = new Project { Name = "Alpha", CreatedAt = DateTime.UtcNow };
        var p2 = new Project { Name = "Beta", CreatedAt = DateTime.UtcNow };
        _db.Projects.AddRange(p1, p2);
        await _db.SaveChangesAsync();

        _db.Tasks.AddRange(
            new TaskItem { Title = "T1", ProjectId = p1.Id, CreatedAt = DateTime.UtcNow },
            new TaskItem { Title = "T2", ProjectId = p1.Id, CreatedAt = DateTime.UtcNow },
            new TaskItem { Title = "T3", ProjectId = p2.Id, CreatedAt = DateTime.UtcNow }
        );
        await _db.SaveChangesAsync();

        var result = await _sut.GetStatsAsync();

        result.TasksPerProject.Should().HaveCount(2);
        result.TasksPerProject.First().ProjectName.Should().Be("Alpha");
        result.TasksPerProject.First().TaskCount.Should().Be(2);
    }

    public void Dispose() => _db.Dispose();
}
