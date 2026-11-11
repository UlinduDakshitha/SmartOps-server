using SmartOps.Domain.Common;

namespace SmartOps.Domain.Entities;

public class Team : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public Guid? TeamLeadId { get; private set; }
    private Team()
    {
    }

    public Team(
        string name,
        string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Team name is required.",
                nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "Team description is required.",
                nameof(description));

        Name = name;
        Description = description;
    }

    public void Update(
        string name,
        string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Team name is required.",
                nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "Team description is required.",
                nameof(description));

        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void AssignTeamLead(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "Team lead is required.",
                nameof(userId));

        TeamLeadId = userId;
    }
    
    public void RemoveTeamLead()
    {
        TeamLeadId = null;
    }
}