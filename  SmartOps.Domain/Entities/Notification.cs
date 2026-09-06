using SmartOps.Domain.Common;

namespace SmartOps.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; private set; }

    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;

    public string Type { get; private set; } = string.Empty;

    public bool IsRead { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private Notification()
    {
    }

    public Notification(
        Guid userId,
        string title,
        string message,
        string type)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "User is required.",
                nameof(userId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(
                "Notification title is required.",
                nameof(title));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException(
                "Notification message is required.",
                nameof(message));

        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException(
                "Notification type is required.",
                nameof(type));

        UserId = userId;
        Title = title;
        Message = message;
        Type = type;

        IsRead = false;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }

    public void MarkAsUnread()
    {
        IsRead = false;
    }
}