using SmartOps.Domain.Common;

namespace SmartOps.Domain.Entities;

public class IncidentHistory : BaseEntity
{
    public Guid IncidentId { get; private set; }
    public Guid UserId { get; private set; }

    public string Action { get; private set; } = string.Empty;
    public string? OldValue { get; private set; }
    public string? NewValue { get; private set; }

    private IncidentHistory()
    {
    }

    public IncidentHistory(
        Guid incidentId,
        Guid userId,
        string action,
        string? oldValue,
        string? newValue)
    {
        if (incidentId == Guid.Empty)
            throw new ArgumentException(
                "Incident is required.",
                nameof(incidentId));

        if (userId == Guid.Empty)
            throw new ArgumentException(
                "User is required.",
                nameof(userId));

        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException(
                "Action is required.",
                nameof(action));

        IncidentId = incidentId;
        UserId = userId;
        Action = action;
        OldValue = oldValue;
        NewValue = newValue;
    }
}