using SmartOps.Application.DTOs.Users;
using SmartOps.Application.Interfaces;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Application.Interfaces.Services;
using SmartOps.Domain.Entities;

namespace SmartOps.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponse> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User not found.");
        }

        return MapToResponse(user);
    }

    public async Task<UserListResponse> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return new UserListResponse
        {
            Users = users
                .Select(MapToResponse)
                .ToList(),

            TotalCount = users.Count
        };
    }

    public async Task<UserResponse> CreateAsync(
        CreateUserRequest request)
    {
        var email = request.Email
            .Trim()
            .ToLowerInvariant();

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

        return MapToResponse(user);
    }

    public async Task<UserResponse> UpdateAsync(
        Guid id,
        UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User not found.");
        }

        // User entity currently has no update method.
        // We will add the domain update method next.

        user.Update(
            request.FullName.Trim(),
            request.Email.Trim().ToLowerInvariant());

        await _userRepository.UpdateAsync(user);

        return MapToResponse(user);
    }

    public async Task DeactivateAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User not found.");
        }

        user.Deactivate();

        await _userRepository.UpdateAsync(user);
    }

    public async Task ActivateAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User not found.");
        }

        user.Activate();

        await _userRepository.UpdateAsync(user);
    }

    private static UserResponse MapToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}