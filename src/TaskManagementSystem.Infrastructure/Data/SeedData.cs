using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();

        if (await db.Projects.AnyAsync())
            return;

        var projects = new List<Project>
        {
            new()
            {
                Name = "Website Redesign",
                Description = "Redesign the company website with modern UI/UX",
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new()
            {
                Name = "Mobile App",
                Description = "Build cross-platform mobile app for customers",
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new()
            {
                Name = "API Integration",
                Description = "Integrate third-party payment and shipping APIs",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            }
        };

        db.Projects.AddRange(projects);
        await db.SaveChangesAsync();

        var tasks = new List<TaskItem>
        {
            new() { Title = "Design new homepage mockup", Priority = Priority.High, Status = TaskItemStatus.Done, ProjectId = projects[0].Id, CreatedAt = DateTime.UtcNow.AddDays(-28), DueDate = DateTime.UtcNow.AddDays(-15) },
            new() { Title = "Implement responsive layout", Priority = Priority.High, Status = TaskItemStatus.InProgress, ProjectId = projects[0].Id, CreatedAt = DateTime.UtcNow.AddDays(-20), DueDate = DateTime.UtcNow.AddDays(5) },
            new() { Title = "SEO optimization", Priority = Priority.Medium, Status = TaskItemStatus.Todo, ProjectId = projects[0].Id, CreatedAt = DateTime.UtcNow.AddDays(-15), DueDate = DateTime.UtcNow.AddDays(14) },
            new() { Title = "Write content for About page", Priority = Priority.Low, Status = TaskItemStatus.Todo, ProjectId = projects[0].Id, CreatedAt = DateTime.UtcNow.AddDays(-10) },
            new() { Title = "Set up React Native project", Priority = Priority.Critical, Status = TaskItemStatus.Done, ProjectId = projects[1].Id, CreatedAt = DateTime.UtcNow.AddDays(-18) },
            new() { Title = "Design login and onboarding screens", Priority = Priority.High, Status = TaskItemStatus.InProgress, ProjectId = projects[1].Id, CreatedAt = DateTime.UtcNow.AddDays(-15), DueDate = DateTime.UtcNow.AddDays(3) },
            new() { Title = "Push notifications integration", Priority = Priority.Medium, Status = TaskItemStatus.Todo, ProjectId = projects[1].Id, CreatedAt = DateTime.UtcNow.AddDays(-12), DueDate = DateTime.UtcNow.AddDays(21) },
            new() { Title = "Offline mode support", Priority = Priority.Low, Status = TaskItemStatus.Cancelled, ProjectId = projects[1].Id, CreatedAt = DateTime.UtcNow.AddDays(-10) },
            new() { Title = "Evaluate Stripe API", Priority = Priority.High, Status = TaskItemStatus.Done, ProjectId = projects[2].Id, CreatedAt = DateTime.UtcNow.AddDays(-8) },
            new() { Title = "Implement payment webhook", Priority = Priority.Critical, Status = TaskItemStatus.InProgress, ProjectId = projects[2].Id, CreatedAt = DateTime.UtcNow.AddDays(-6), DueDate = DateTime.UtcNow.AddDays(2) },
            new() { Title = "Shipping rate calculation", Priority = Priority.Medium, Status = TaskItemStatus.Todo, ProjectId = projects[2].Id, CreatedAt = DateTime.UtcNow.AddDays(-4), DueDate = DateTime.UtcNow.AddDays(-1) },
            new() { Title = "Write integration tests", Priority = Priority.High, Status = TaskItemStatus.Todo, ProjectId = projects[2].Id, CreatedAt = DateTime.UtcNow.AddDays(-2), DueDate = DateTime.UtcNow.AddDays(10) }
        };

        db.Tasks.AddRange(tasks);
        await db.SaveChangesAsync();
    }
}
