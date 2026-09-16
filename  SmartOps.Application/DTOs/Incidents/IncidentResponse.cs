using SmartOps.Domain.Enums;

namespace SmartOps.Application.DTOs.Incidents;

public class IncidentResponse
{
    public Guid Id { get; set; }

    public string IncidentNumber { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Priority Priority { get; set; }

    public Severity Severity { get; set; }

    public IncidentStatus Status { get; set; }

    public Guid CreatedByUserId { get; set; }

    public Guid? AssignedTeamId { get; set; }

    public Guid? AssignedUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public string? ResolutionNotes { get; set; }

    public string? RootCause { get; set; }
}