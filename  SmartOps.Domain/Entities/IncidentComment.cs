using SmartOps.Domain.Common;

namespace SmartOps.Domain.Entities;

public class IncidentComment : BaseEntity
{
    public Guid IncidentId { get; private set; }
    public Guid UserId { get; private set; }

    public string Comment { get; private set; } = string.Empty;

    private IncidentComment()
    {
    }

    public IncidentComment(
        Guid incidentId,
        Guid userId,
        string comment)
    {
        if (incidentId == Guid.Empty)
            throw new ArgumentException(
                "Incident is required.",
                nameof(incidentId));

        if (userId == Guid.Empty)
            throw new ArgumentException(
                "User is required.",
                nameof(userId));

        if (string.IsNullOrWhiteSpace(comment))
            throw new ArgumentException(
                "Comment is required.",
                nameof(comment));

        IncidentId = incidentId;
        UserId = userId;
        Comment = comment;
    }
}