using SmartOps.Application.DTOs.Dashboard;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Application.Interfaces.Services;
using SmartOps.Domain.Enums;

namespace SmartOps.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IIncidentRepository _incidentRepository;

    public DashboardService(
        IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<DashboardResponse> GetSummaryAsync()
    {
        var incidents = await _incidentRepository.GetAllAsync();

        return new DashboardResponse
        {
            TotalIncidents = incidents.Count,

            NewIncidents = incidents.Count(
                x => x.Status == IncidentStatus.New),

            AssignedIncidents = incidents.Count(
                x => x.Status == IncidentStatus.Assigned),

            InProgressIncidents = incidents.Count(
                x => x.Status == IncidentStatus.InProgress),

            ResolvedIncidents = incidents.Count(
                x => x.Status == IncidentStatus.Resolved),

            ClosedIncidents = incidents.Count(
                x => x.Status == IncidentStatus.Closed),

            CancelledIncidents = incidents.Count(
                x => x.Status == IncidentStatus.Cancelled),

            ResponseSlaBreachedCount = incidents.Count(
                x => x.IsResponseSlaBreached()),

            ResolutionSlaBreachedCount = incidents.Count(
                x => x.IsResolutionSlaBreached()),
            
            ResponseSlaAtRiskCount = incidents.Count(x => x.IsResponseSlaAtRisk()),
            ResolutionSlaAtRiskCount = incidents.Count(x => x.IsResolutionSlaAtRisk()),
            
            LowPriorityCount = incidents.Count(x => x.Priority == Priority.Low),
            MediumPriorityCount = incidents.Count(x => x.Priority == Priority.Medium),
            HighPriorityCount = incidents.Count(x => x.Priority == Priority.High),
            CriticalPriorityCount = incidents.Count(x => x.Priority == Priority.Critical),

            LowSeverityCount = incidents.Count(x => x.Severity == Severity.Low),
            MediumSeverityCount = incidents.Count(x => x.Severity == Severity.Medium),
            HighSeverityCount = incidents.Count(x => x.Severity == Severity.High),
            CriticalSeverityCount = incidents.Count(x => x.Severity == Severity.Critical),
            
            AverageResolutionTimeMinutes = incidents
            .Where(x => x.ResolvedAt.HasValue)
            .Select(x => (x.ResolvedAt!.Value - x.CreatedAt).TotalMinutes)
            .DefaultIfEmpty(0)
            .Average(),
        };
        
        
    }
    public async Task<List<MonthlyIncidentTrendResponse>> GetMonthlyIncidentTrendAsync()
    {
        var incidents = await _incidentRepository.GetAllAsync();

        return incidents
            .GroupBy(x => new
            {
                x.CreatedAt.Year,
                x.CreatedAt.Month
            })
            .OrderBy(x => x.Key.Year)
            .ThenBy(x => x.Key.Month)
            .Select(x => new MonthlyIncidentTrendResponse
            {
                Year = x.Key.Year,
                Month = x.Key.Month,

                MonthName = new DateTime(
                    x.Key.Year,
                    x.Key.Month,
                    1
                ).ToString("MMMM"),

                IncidentCount = x.Count(),

                ResolvedCount = x.Count(i =>
                    i.ResolvedAt.HasValue &&
                    i.ResolvedAt.Value.Year == x.Key.Year &&
                    i.ResolvedAt.Value.Month == x.Key.Month),

                ClosedCount = x.Count(i =>
                    i.ClosedAt.HasValue &&
                    i.ClosedAt.Value.Year == x.Key.Year &&
                    i.ClosedAt.Value.Month == x.Key.Month)
            })
            .ToList();
    }
}