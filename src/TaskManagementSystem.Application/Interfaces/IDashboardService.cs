using TaskManagementSystem.Application.DTOs;

namespace TaskManagementSystem.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync();
}
