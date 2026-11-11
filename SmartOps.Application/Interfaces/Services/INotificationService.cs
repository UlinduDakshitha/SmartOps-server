using SmartOps.Application.DTOs.Notifications;

namespace SmartOps.Application.Interfaces.Services;

public interface INotificationService
{
    Task<List<NotificationResponse>> GetByUserIdAsync(Guid userId);

    Task<NotificationResponse> CreateAsync(
        Guid userId,
        string title,
        string message,
        string type);

    Task MarkAsReadAsync(
        Guid notificationId,
        Guid userId);

    Task MarkAsUnreadAsync(
        Guid notificationId,
        Guid userId);
}