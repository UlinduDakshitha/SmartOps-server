namespace SmartOps.Application.DTOs.Users;

public class UserListResponse
{
    public List<UserResponse> Users { get; set; } = new();

    public int TotalCount { get; set; }
}