using SmartOps.Domain.Common;

namespace SmartOps.Domain.Entities;

public class User : AuditableEntity
{
    public string FullName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    private User()
    {
    }

    public User(
        string fullName,
        string email,
        string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException(
                "Full name is required.",
                nameof(fullName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException(
                "Email is required.",
                nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException(
                "Password hash is required.",
                nameof(passwordHash));

        FullName = fullName;
        Email = email;
        PasswordHash = passwordHash;
        IsActive = true;
    }
}