using SmartOps.Domain.Common;

namespace SmartOps.Domain.Entities;

public class IncidentAssignment : BaseEntity
{
    public Guid IncidentId { get; private set; }
    public Guid TeamId { get; private set; }
    public Guid? AssignedUserId { get; private set; }

    public Guid AssignedByUserId { get; private set; }

    public DateTime AssignedAt { get; private set; }
    public DateTime? UnassignedAt { get; private set; }

    private IncidentAssignment()
    {
    }

    public IncidentAssignment(
        Guid incidentId,
        Guid teamId,
        Guid? assignedUserId,
        Guid assignedByUserId)
    {
        if (incidentId == Guid.Empty)
            throw new ArgumentException(
                "Incident is required.",
                nameof(incidentId));

        if (teamId == Guid.Empty)
            throw new ArgumentException(
                "Team is required.",
                nameof(teamId));

        if (assignedByUserId == Guid.Empty)
            throw new ArgumentException(
                "Assigned by user is required.",
                nameof(assignedByUserId));

        IncidentId = incidentId;
        TeamId = teamId;
        AssignedUserId = assignedUserId;
        AssignedByUserId = assignedByUserId;

        AssignedAt = DateTime.UtcNow;
    }

    public void Unassign()
    {
        if (UnassignedAt.HasValue)
            throw new InvalidOperationException(
                "This assignment has already been unassigned.");

        UnassignedAt = DateTime.UtcNow;
    }
}