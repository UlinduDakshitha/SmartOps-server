using SmartOps.Domain.Enums;

namespace SmartOps.Application.DTOs.Incidents;

public class CreateIncidentRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Priority Priority { get; set; }

    public Severity Severity { get; set; }
}