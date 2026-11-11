using SmartOps.Application.DTOs.Auth;

namespace SmartOps.Application.Interfaces.Services;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);

    Task<LoginResponse> LoginAsync(LoginRequest request);

    Task<LoginResponse> RefreshTokenAsync(string refreshToken);

    Task RevokeRefreshTokenAsync(string refreshToken);
}