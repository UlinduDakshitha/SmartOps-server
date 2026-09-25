using SmartOps.Application.DTOs.Auth;
using SmartOps.Application.Interfaces;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Application.Interfaces.Services;
using SmartOps.Domain.Entities;

namespace SmartOps.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IAuditLogService _auditLogService;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IAuditLogService auditLogService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _auditLogService = auditLogService;
    }

    public async Task<RegisterResponse> RegisterAsync(
        RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _userRepository.ExistsByEmailAsync(email))
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        var passwordHash = _passwordHasher
            .HashPassword(request.Password);

        var user = new User(
            request.FullName.Trim(),
            email,
            passwordHash);

        await _userRepository.AddAsync(user);

        var defaultRole = await _roleRepository
            .GetByNameAsync("Support Agent");

        if (defaultRole is null)
        {
            throw new InvalidOperationException(
                "Default role was not found.");
        }

        var userRole = new UserRole(
            user.Id,
            defaultRole.Id);

        await _userRoleRepository.AddAsync(userRole);
        
        await _auditLogService.LogAsync(
            user.Id,
            "User Registered",
            "User",
            user.Id,
            null,
            $"Email: {user.Email}");

        return new RegisterResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Message = "User registered successfully."
        };
    }

    public async Task<LoginResponse> LoginAsync(
        LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _userRepository
            .GetByEmailAsync(email);

        if (user is null || !user.IsActive)
        {
            await _auditLogService.LogAsync(
                user?.Id,
                "Failed Login Attempt",
                "User",
                user?.Id,
                null,
                $"Email: {email}");

            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        var passwordValid = _passwordHasher
            .VerifyPassword(
                request.Password,
                user.PasswordHash);

        if (user is null || !user.IsActive)
        {
            await _auditLogService.LogAsync(
                user?.Id,
                "Failed Login Attempt",
                "User",
                user?.Id,
                null,
                $"Email: {email}");

            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        var userRoles = await _userRoleRepository
            .GetRolesByUserIdAsync(user.Id);

        var roles = userRoles
            .Select(x => x.Name)
            .ToList();

        var accessToken = _jwtTokenService
            .GenerateAccessToken(
                user.Id,
                user.Email,
                roles);

        var refreshToken = _jwtTokenService
            .GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken(
            user.Id,
            refreshToken,
            DateTime.UtcNow.AddDays(7));

        await _refreshTokenRepository
            .AddAsync(refreshTokenEntity);
        await _auditLogService.LogAsync(
            user.Id,
            "User Logged In",
            "User",
            user.Id,
            null,
            $"Email: {user.Email}");
        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt =
                _jwtTokenService.GetAccessTokenExpiration()
        };
    }

    public async Task<LoginResponse> RefreshTokenAsync(
        string refreshToken)
    {
        var storedToken = await _refreshTokenRepository
            .GetByTokenAsync(refreshToken);

        if (storedToken is null || !storedToken.IsActive)
        {
            throw new UnauthorizedAccessException(
                "Invalid or expired refresh token.");
        }

        var user = await _userRepository
            .GetByIdAsync(storedToken.UserId);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAccessException(
                "User is not available.");
        }

        storedToken.Revoke();

        await _refreshTokenRepository
            .UpdateAsync(storedToken);

        var userRoles = await _userRoleRepository
            .GetRolesByUserIdAsync(user.Id);

        var roles = userRoles
            .Select(x => x.Name)
            .ToList();

        var newAccessToken = _jwtTokenService
            .GenerateAccessToken(
                user.Id,
                user.Email,
                roles);

        var newRefreshToken = _jwtTokenService
            .GenerateRefreshToken();

        var newRefreshTokenEntity = new RefreshToken(
            user.Id,
            newRefreshToken,
            DateTime.UtcNow.AddDays(7));

        await _refreshTokenRepository
            .AddAsync(newRefreshTokenEntity);
        
        await _auditLogService.LogAsync(
            user.Id,
            "Refresh Token Used",
            "RefreshToken",
            storedToken.Id,
            null,
            "Refresh token rotated successfully.");

        return new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            AccessTokenExpiresAt =
                _jwtTokenService.GetAccessTokenExpiration()
        };
    }

    public async Task RevokeRefreshTokenAsync(
        string refreshToken)
    {
        var storedToken = await _refreshTokenRepository
            .GetByTokenAsync(refreshToken);

        if (storedToken is null)
        {
            throw new InvalidOperationException(
                "Refresh token was not found.");
        }

        if (!storedToken.IsRevoked)
        {
            storedToken.Revoke();
            
            await _auditLogService.LogAsync(
                storedToken.UserId,
                "Refresh Token Revoked",
                "RefreshToken",
                storedToken.Id,
                null,
                "Refresh token revoked.");

            await _refreshTokenRepository
                .UpdateAsync(storedToken);
        }
    }
}