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
            
            LowPriorityCount = incidents.Count(x => x.Priority == Priority.Low),
            MediumPriorityCount = incidents.Count(x => x.Priority == Priority.Medium),
            HighPriorityCount = incidents.Count(x => x.Priority == Priority.High),
            CriticalPriorityCount = incidents.Count(x => x.Priority == Priority.Critical),

            LowSeverityCount = incidents.Count(x => x.Severity == Severity.Low),
            MediumSeverityCount = incidents.Count(x => x.Severity == Severity.Medium),
            HighSeverityCount = incidents.Count(x => x.Severity == Severity.High),
            CriticalSeverityCount = incidents.Count(x => x.Severity == Severity.Critical),
        };
    }
}