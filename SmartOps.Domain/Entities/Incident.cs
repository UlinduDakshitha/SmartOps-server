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

    public DateTime? ResponseDueAt { get; private set; }

    public DateTime? ResolutionDueAt { get; private set; }

    public DateTime? RespondedAt { get; private set; }
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
        if (Status != IncidentStatus.New &&
            Status != IncidentStatus.Assigned)
        {
            throw new InvalidOperationException(
                "Only a new or assigned incident can be assigned.");
        }

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
        RespondedAt = DateTime.UtcNow;
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
    public void SetSLA(
        DateTime responseDueAt,
        DateTime resolutionDueAt)
    {
        if (responseDueAt <= CreatedAt)
            throw new ArgumentException(
                "Response due time must be after incident creation time.",
                nameof(responseDueAt));

        if (resolutionDueAt <= responseDueAt)
            throw new ArgumentException(
                "Resolution due time must be after response due time.",
                nameof(resolutionDueAt));

        ResponseDueAt = responseDueAt;
        ResolutionDueAt = resolutionDueAt;
    }
    public bool IsResponseSlaAtRisk()
    {
        if (!ResponseDueAt.HasValue || RespondedAt.HasValue)
            return false;

        var remainingTime = ResponseDueAt.Value - DateTime.UtcNow;

        return remainingTime.TotalMinutes <= 30 &&
               remainingTime.TotalMinutes > 0;
    }

    public bool IsResolutionSlaAtRisk()
    {
        if (!ResolutionDueAt.HasValue || ResolvedAt.HasValue)
            return false;

        var remainingTime = ResolutionDueAt.Value - DateTime.UtcNow;

        return remainingTime.TotalMinutes <= 30 &&
               remainingTime.TotalMinutes > 0;
    }
    public bool IsResponseSlaBreached()
    {
        if (!ResponseDueAt.HasValue)
            return false;

        if (RespondedAt.HasValue)
            return RespondedAt.Value > ResponseDueAt.Value;

        return DateTime.UtcNow > ResponseDueAt.Value;
    }
    public bool IsResolutionSlaBreached()
    {
        if (!ResolutionDueAt.HasValue)
            return false;

        if (ResolvedAt.HasValue)
            return ResolvedAt.Value > ResolutionDueAt.Value;

        return DateTime.UtcNow > ResolutionDueAt.Value;
    }
    public void Update(
        string title,
        string description,
        Priority priority,
        Severity severity)
    {
        if (Status == IncidentStatus.Closed ||
            Status == IncidentStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "A closed or cancelled incident cannot be updated.");
        }

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(
                "Incident title is required.",
                nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "Incident description is required.",
                nameof(description));

        Title = title;
        Description = description;
        Priority = priority;
        Severity = severity;
        UpdatedAt = DateTime.UtcNow;
    }
}