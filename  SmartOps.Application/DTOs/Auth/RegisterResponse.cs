namespace SmartOps.Application.DTOs.Auth;

public class RegisterResponse
{
    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}