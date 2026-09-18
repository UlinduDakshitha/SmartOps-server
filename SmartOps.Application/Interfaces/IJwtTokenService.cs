namespace SmartOps.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(
        Guid userId,
        string email,
        IEnumerable<string> roles);

    string GenerateRefreshToken();

    DateTime GetAccessTokenExpiration();
}