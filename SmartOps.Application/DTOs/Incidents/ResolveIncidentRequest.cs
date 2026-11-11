namespace SmartOps.Application.DTOs.Incidents;

public class ResolveIncidentRequest
{
    public string ResolutionNotes { get; set; } = string.Empty;

    public string RootCause { get; set; } = string.Empty;
}