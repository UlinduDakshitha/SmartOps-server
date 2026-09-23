namespace SmartOps.Application.DTOs.Dashboard;

public class DashboardResponse
{
    public int TotalIncidents { get; set; }

    public int NewIncidents { get; set; }

    public int AssignedIncidents { get; set; }

    public int InProgressIncidents { get; set; }

    public int ResolvedIncidents { get; set; }

    public int ClosedIncidents { get; set; }

    public int CancelledIncidents { get; set; }

    public int ResponseSlaBreachedCount { get; set; }

    public int ResolutionSlaBreachedCount { get; set; }
    
    public int LowPriorityCount { get; set; }
    public int MediumPriorityCount { get; set; }
    public int HighPriorityCount { get; set; }
    public int CriticalPriorityCount { get; set; }

    public int LowSeverityCount { get; set; }
    public int MediumSeverityCount { get; set; }
    public int HighSeverityCount { get; set; }
    public int CriticalSeverityCount { get; set; }
}