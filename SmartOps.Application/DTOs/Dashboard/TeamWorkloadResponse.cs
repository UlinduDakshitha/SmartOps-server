namespace SmartOps.Application.DTOs.Dashboard;

public class TeamWorkloadResponse
{
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;

    public int TotalIncidents { get; set; }
    public int NewIncidents { get; set; }
    public int AssignedIncidents { get; set; }
    public int InProgressIncidents { get; set; }
    public int ResolvedIncidents { get; set; }
    public int ClosedIncidents { get; set; }
}