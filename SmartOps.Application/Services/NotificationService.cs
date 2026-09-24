using SmartOps.Application.DTOs.Notifications;
using SmartOps.Application.Interfaces;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Application.Interfaces.Services;
using SmartOps.Domain.Entities;

namespace SmartOps.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationPublisher _notificationPublisher;

    public NotificationService(
        INotificationRepository notificationRepository,
        INotificationPublisher notificationPublisher)
    {
        _notificationRepository = notificationRepository;
        _notificationPublisher = notificationPublisher;
    }
    public async Task<NotificationResponse> CreateAsync(
        Guid userId,
        string title,
        string message,
        string type)
    {
        var notification = new Notification(
            userId,
            title.Trim(),
            message.Trim(),
            type.Trim());

        await _notificationRepository.AddAsync(notification);
        
        await _notificationPublisher.PublishAsync(
            userId,
            notification.Title,
            notification.Message,
            notification.Type);

        return MapToResponse(notification);
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

    public async Task MarkAsReadAsync(
        Guid notificationId,
        Guid userId)
    {
        var notification = await _notificationRepository
            .GetByIdAsync(notificationId);

        if (notification is null)
            throw new KeyNotFoundException(
                "Notification not found.");

        if (notification.UserId != userId)
            throw new UnauthorizedAccessException(
                "You are not authorized to update this notification.");

        notification.MarkAsRead();

        await _notificationRepository.UpdateAsync(notification);
    }

    public async Task MarkAsUnreadAsync(
        Guid notificationId,
        Guid userId)
    {
        var notification = await _notificationRepository
            .GetByIdAsync(notificationId);

        if (notification is null)
            throw new KeyNotFoundException(
                "Notification not found.");

        if (notification.UserId != userId)
            throw new UnauthorizedAccessException(
                "You are not authorized to update this notification.");

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