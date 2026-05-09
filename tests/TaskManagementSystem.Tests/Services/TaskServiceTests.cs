using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Services;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;
using TaskManagementSystem.Infrastructure.Data;

namespace TaskManagementSystem.Tests.Services;

public class TaskServiceTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly TaskService _sut;
    private readonly Project _project;

    public TaskServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _sut = new TaskService(_db);

        _project = new Project { Name = "Test Project", CreatedAt = DateTime.UtcNow };
        _db.Projects.Add(_project);
        _db.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoTasks()
    {
        var result = await _sut.GetAllAsync(null, null, null);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_CreatesTask()
    {
        var dto = new CreateTaskItemDto
        {
            Title = "New Task",
            Priority = Priority.High,
            Status = TaskItemStatus.Todo,
            ProjectId = _project.Id
        };

        var result = await _sut.CreateAsync(dto);

        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("New Task");
        result.Priority.Should().Be(Priority.High);
        result.ProjectId.Should().Be(_project.Id);
        result.ProjectName.Should().Be("Test Project");
    }

    [Fact]
    public async Task CreateAsync_ThrowsNotFoundException_WhenProjectNotExists()
    {
        var dto = new CreateTaskItemDto { Title = "X", ProjectId = 999 };

        var act = async () => await _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetAllAsync_FiltersBy_Status()
    {
        await _sut.CreateAsync(new CreateTaskItemDto { Title = "T1", Status = TaskItemStatus.Todo, ProjectId = _project.Id });
        await _sut.CreateAsync(new CreateTaskItemDto { Title = "T2", Status = TaskItemStatus.Done, ProjectId = _project.Id });

        var result = await _sut.GetAllAsync(TaskItemStatus.Todo, null, null);

        result.Should().HaveCount(1);
        result.First().Title.Should().Be("T1");
    }

    [Fact]
    public async Task GetAllAsync_FiltersBy_Priority()
    {
        await _sut.CreateAsync(new CreateTaskItemDto { Title = "Low", Priority = Priority.Low, ProjectId = _project.Id });
        await _sut.CreateAsync(new CreateTaskItemDto { Title = "High", Priority = Priority.High, ProjectId = _project.Id });

        var result = await _sut.GetAllAsync(null, Priority.High, null);

        result.Should().HaveCount(1);
        result.First().Title.Should().Be("High");
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTask()
    {
        var created = await _sut.CreateAsync(new CreateTaskItemDto
        {
            Title = "Original",
            Priority = Priority.Low,
            Status = TaskItemStatus.Todo,
            ProjectId = _project.Id
        });

        var result = await _sut.UpdateAsync(created.Id, new UpdateTaskItemDto
        {
            Title = "Updated",
            Priority = Priority.Critical,
            Status = TaskItemStatus.Done
        });

        result.Title.Should().Be("Updated");
        result.Priority.Should().Be(Priority.Critical);
        result.Status.Should().Be(TaskItemStatus.Done);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFoundException_WhenNotExists()
    {
        var act = async () => await _sut.UpdateAsync(999, new UpdateTaskItemDto { Title = "X" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_RemovesTask()
    {
        var created = await _sut.CreateAsync(new CreateTaskItemDto
        {
            Title = "Delete Me",
            ProjectId = _project.Id
        });

        await _sut.DeleteAsync(created.Id);

        var all = await _sut.GetAllAsync(null, null, null);
        all.Should().BeEmpty();
    }

    public void Dispose() => _db.Dispose();
}
