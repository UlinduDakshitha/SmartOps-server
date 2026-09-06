using SmartOps.Domain.Common;

namespace SmartOps.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private Role()
    {
    }

    public Role(
        string name,
        string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Role name is required.",
                nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "Role description is required.",
                nameof(description));

        Name = name;
        Description = description;
    }
}