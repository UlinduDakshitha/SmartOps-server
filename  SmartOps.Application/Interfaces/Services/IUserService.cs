using SmartOps.Application.DTOs.Users;

namespace SmartOps.Application.Interfaces.Services;

public interface IUserService
{
    Task<UserResponse> GetByIdAsync(Guid id);

    Task<UserListResponse> GetAllAsync();

    Task<UserResponse> CreateAsync(
        CreateUserRequest request);

    Task<UserResponse> UpdateAsync(
        Guid id,
        UpdateUserRequest request);

    Task DeactivateAsync(Guid id);

    Task ActivateAsync(Guid id);
}