using SmartOps.Application.DTOs.Notifications;

namespace SmartOps.Application.Interfaces.Services;

public interface INotificationService
{
    Task<List<NotificationResponse>> GetByUserIdAsync(
        Guid userId);

    Task MarkAsReadAsync(Guid notificationId);

    Task MarkAsUnreadAsync(Guid notificationId);
}