using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.DTOs;

public class TaskItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Priority Priority { get; set; }
    public TaskItemStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateTaskItemDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
    public DateTime? DueDate { get; set; }
    public int ProjectId { get; set; }
}

public class UpdateTaskItemDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Priority Priority { get; set; }
    public TaskItemStatus Status { get; set; }
    public DateTime? DueDate { get; set; }
}
