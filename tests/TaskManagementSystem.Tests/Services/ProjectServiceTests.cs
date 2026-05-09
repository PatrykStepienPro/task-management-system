using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Services;
using TaskManagementSystem.Infrastructure.Data;

namespace TaskManagementSystem.Tests.Services;

public class ProjectServiceTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly ProjectService _sut;

    public ProjectServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _sut = new ProjectService(_db);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoProjects()
    {
        var result = await _sut.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_CreatesAndReturnsProject()
    {
        var dto = new CreateProjectDto { Name = "Test Project", Description = "Desc" };

        var result = await _sut.CreateAsync(dto);

        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("Test Project");
        result.Description.Should().Be("Desc");
        result.TaskCount.Should().Be(0);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProjects_OrderedByName()
    {
        await _sut.CreateAsync(new CreateProjectDto { Name = "Zebra" });
        await _sut.CreateAsync(new CreateProjectDto { Name = "Alpha" });

        var result = (await _sut.GetAllAsync()).ToList();

        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Alpha");
        result[1].Name.Should().Be("Zebra");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsProject_WhenExists()
    {
        var created = await _sut.CreateAsync(new CreateProjectDto { Name = "My Project" });

        var result = await _sut.GetByIdAsync(created.Id);

        result.Id.Should().Be(created.Id);
        result.Name.Should().Be("My Project");
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenNotExists()
    {
        var act = async () => await _sut.GetByIdAsync(999);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*999*");
    }

    [Fact]
    public async Task UpdateAsync_UpdatesProjectFields()
    {
        var created = await _sut.CreateAsync(new CreateProjectDto { Name = "Old Name" });

        var result = await _sut.UpdateAsync(created.Id, new UpdateProjectDto
        {
            Name = "New Name",
            Description = "New Desc"
        });

        result.Name.Should().Be("New Name");
        result.Description.Should().Be("New Desc");
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFoundException_WhenNotExists()
    {
        var act = async () => await _sut.UpdateAsync(999, new UpdateProjectDto { Name = "X" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_RemovesProject()
    {
        var created = await _sut.CreateAsync(new CreateProjectDto { Name = "To Delete" });

        await _sut.DeleteAsync(created.Id);

        var all = await _sut.GetAllAsync();
        all.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_ThrowsNotFoundException_WhenNotExists()
    {
        var act = async () => await _sut.DeleteAsync(999);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    public void Dispose() => _db.Dispose();
}
