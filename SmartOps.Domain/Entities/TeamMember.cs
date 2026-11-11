namespace SmartOps.Domain.Entities;

public class TeamMember
{
    public Guid TeamId { get; private set; }
    public Guid UserId { get; private set; }

    private TeamMember()
    {
    }

    public TeamMember(
        Guid teamId,
        Guid userId)
    {
        if (teamId == Guid.Empty)
            throw new ArgumentException(
                "Team is required.",
                nameof(teamId));

        if (userId == Guid.Empty)
            throw new ArgumentException(
                "User is required.",
                nameof(userId));

        TeamId = teamId;
        UserId = userId;
    }
}