using SmartOps.Domain.Common;
using SmartOps.Domain.Enums;

namespace SmartOps.Domain.Entities;

public class Incident : AuditableEntity
{
    public string IncidentNumber { get; private set; } = string.Empty;
    
    public string Title { get; private set; } = string.Empty;
    
    public string Description { get; private set; } = string.Empty;
    
    public Priority Priority { get; private set; }
    
    public Severity Severity { get; private set; }
    
    public IncidentStatus Status { get; private set; }

    public Guid CreatedByUserId { get; private set; }
    
    public Guid? AssignedTeamId { get; private set; }
    
    public Guid? AssignedUserId { get; private set; }

    public DateTime? ResolvedAt { get; private set; }
    
    public DateTime? ClosedAt { get; private set; }

    private Incident()
    {
    }

    public Incident(
        string incidentNumber,
        string title,
        string description,
        Priority priority,
        Severity severity,
        Guid createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(incidentNumber))
            throw new ArgumentException("Incident number is required.", nameof(incidentNumber));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Incident title is required.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Incident description is required.", nameof(description));

        if (createdByUserId == Guid.Empty)
            throw new ArgumentException("Created by user is required.", nameof(createdByUserId));

        IncidentNumber = incidentNumber;
        Title = title;
        Description = description;
        Priority = priority;
        Severity = severity;
        CreatedByUserId = createdByUserId;

        Status = IncidentStatus.New;
    }
}