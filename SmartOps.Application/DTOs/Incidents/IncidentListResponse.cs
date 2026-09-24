namespace SmartOps.Application.DTOs.Incidents;

public class IncidentListResponse
{
    public List<IncidentResponse> Items { get; set; } = new();

    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}