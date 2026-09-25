using SmartOps.Application.DTOs.Roles;
using SmartOps.Application.DTOs.Users;
using SmartOps.Application.Interfaces;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Application.Interfaces.Services;
using SmartOps.Domain.Entities;

namespace SmartOps.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
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

        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        var existingUser = await _userRepository
            .GetByEmailAsync(email);

        if (existingUser is not null &&
            existingUser.Id != user.Id)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        user.Update(
            request.FullName.Trim(),
            email);

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

    public async Task<List<RoleResponse>> GetRolesAsync(
        Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User not found.");
        }

        var roles = await _userRoleRepository
            .GetRolesByUserIdAsync(userId);

        return roles
            .Select(role => new RoleResponse
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            })
            .ToList();
    }

    public async Task AssignRoleAsync(
        Guid userId,
        Guid roleId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User not found.");
        }

        var role = await _roleRepository.GetByIdAsync(roleId);

        if (role is null)
        {
            throw new KeyNotFoundException(
                "Role not found.");
        }

        var alreadyAssigned = await _userRoleRepository
            .ExistsAsync(userId, roleId);

        if (alreadyAssigned)
        {
            throw new InvalidOperationException(
                "This role is already assigned to the user.");
        }

        var userRole = new UserRole(
            userId,
            roleId);

        await _userRoleRepository.AddAsync(userRole);
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