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
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
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

        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        var passwordValid = _passwordHasher
            .VerifyPassword(
                request.Password,
                user.PasswordHash);

        if (!passwordValid)
        {
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

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt =
                _jwtTokenService.GetAccessTokenExpiration()
        };
    }
}