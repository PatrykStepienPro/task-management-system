namespace TaskManagementSystem.Web.Models;

public class DashboardStatsDto
{
    public int TotalProjects { get; set; }
    public int TotalTasks { get; set; }
    public int TasksTodo { get; set; }
    public int TasksInProgress { get; set; }
    public int TasksDone { get; set; }
    public int TasksCancelled { get; set; }
    public int OverdueTasks { get; set; }
    public List<ProjectTaskCountDto> TasksPerProject { get; set; } = new();
}

public class ProjectTaskCountDto
{
    public string ProjectName { get; set; } = string.Empty;
    public int TaskCount { get; set; }
}
