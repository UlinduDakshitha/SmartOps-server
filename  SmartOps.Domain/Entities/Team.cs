using SmartOps.Domain.Common;

namespace SmartOps.Domain.Entities;

public class Team : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

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
}