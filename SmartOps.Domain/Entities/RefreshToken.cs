using SmartOps.Domain.Common;

namespace SmartOps.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }

    public string Token { get; private set; } = string.Empty;

    public DateTime ExpiresAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public bool IsRevoked => RevokedAt.HasValue;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public bool IsActive => !IsRevoked && !IsExpired;

    private RefreshToken()
    {
    }

    public RefreshToken(
        Guid userId,
        string token,
        DateTime expiresAt)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "User is required.",
                nameof(userId));

        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException(
                "Token is required.",
                nameof(token));

        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException(
                "Token expiration must be in the future.",
                nameof(expiresAt));

        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
    }

    public void Revoke()
    {
        if (IsRevoked)
            throw new InvalidOperationException(
                "Token has already been revoked.");

        RevokedAt = DateTime.UtcNow;
    }
}