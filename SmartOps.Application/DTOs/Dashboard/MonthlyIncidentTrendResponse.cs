namespace SmartOps.Application.DTOs.Dashboard;

public class MonthlyIncidentTrendResponse
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int IncidentCount { get; set; }
    public int ResolvedCount { get; set; }
    public int ClosedCount { get; set; }
}