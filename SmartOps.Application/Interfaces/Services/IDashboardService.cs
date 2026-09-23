using SmartOps.Application.DTOs.Dashboard;

namespace SmartOps.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardResponse> GetSummaryAsync();
}