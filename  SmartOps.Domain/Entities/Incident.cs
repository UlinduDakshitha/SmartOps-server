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
    
    public string? ResolutionNotes { get; private set; }

    public string? RootCause { get; private set; }

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
    public void Assign(Guid teamId, Guid? userId = null)
    {
        if (Status != IncidentStatus.New)
            throw new InvalidOperationException(
                "Only a new incident can be assigned.");

        if (teamId == Guid.Empty)
            throw new ArgumentException(
                "Team is required.",
                nameof(teamId));

        AssignedTeamId = teamId;
        AssignedUserId = userId;
        Status = IncidentStatus.Assigned;
    }
    public void StartProgress()
    {
        if (Status != IncidentStatus.Assigned)
            throw new InvalidOperationException(
                "Only an assigned incident can be moved to in progress.");

        Status = IncidentStatus.InProgress;
    }
    public void Resolve(string resolutionNotes, string rootCause)
    {
        if (Status != IncidentStatus.InProgress)
            throw new InvalidOperationException(
                "Only an incident in progress can be resolved.");

        if (string.IsNullOrWhiteSpace(resolutionNotes))
            throw new ArgumentException(
                "Resolution notes are required.",
                nameof(resolutionNotes));

        if (string.IsNullOrWhiteSpace(rootCause))
            throw new ArgumentException(
                "Root cause is required.",
                nameof(rootCause));

        ResolutionNotes = resolutionNotes;
        RootCause = rootCause;
        Status = IncidentStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;
    }
    public void Close()
    {
        if (Status != IncidentStatus.Resolved)
            throw new InvalidOperationException(
                "Only a resolved incident can be closed.");

        Status = IncidentStatus.Closed;
        ClosedAt = DateTime.UtcNow;
    }
    public void Cancel()
    {
        if (Status == IncidentStatus.Resolved ||
            Status == IncidentStatus.Closed)
        {
            throw new InvalidOperationException(
                "A resolved or closed incident cannot be cancelled.");
        }

        Status = IncidentStatus.Cancelled;
    }
}