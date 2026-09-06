namespace SmartOps.Domain.Entities;

public class UserRole
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }

    private UserRole()
    {
    }

    public UserRole(
        Guid userId,
        Guid roleId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "User is required.",
                nameof(userId));

        if (roleId == Guid.Empty)
            throw new ArgumentException(
                "Role is required.",
                nameof(roleId));

        UserId = userId;
        RoleId = roleId;
    }
}