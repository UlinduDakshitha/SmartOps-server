using SmartOps.Application.DTOs.Notifications;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Application.Interfaces.Services;
using SmartOps.Domain.Entities;

namespace SmartOps.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(
        INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<List<NotificationResponse>> GetByUserIdAsync(
        Guid userId)
    {
        var notifications =
            await _notificationRepository.GetByUserIdAsync(userId);

        return notifications
            .Select(MapToResponse)
            .ToList();
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var notification =
            await _notificationRepository.GetByIdAsync(notificationId);

        if (notification is null)
            throw new KeyNotFoundException(
                "Notification not found.");

        notification.MarkAsRead();

        await _notificationRepository.UpdateAsync(notification);
    }

    public async Task MarkAsUnreadAsync(Guid notificationId)
    {
        var notification =
            await _notificationRepository.GetByIdAsync(notificationId);

        if (notification is null)
            throw new KeyNotFoundException(
                "Notification not found.");

        notification.MarkAsUnread();

        await _notificationRepository.UpdateAsync(notification);
    }

    private static NotificationResponse MapToResponse(
        Notification notification)
    {
        return new NotificationResponse
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };
    }
}