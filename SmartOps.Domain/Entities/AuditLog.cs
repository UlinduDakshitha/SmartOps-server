using SmartOps.Domain.Common;

namespace SmartOps.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid? UserId { get; private set; }

    public string Action { get; private set; } = string.Empty;
    public string EntityName { get; private set; } = string.Empty;
    public Guid? EntityId { get; private set; }

    public string? OldValue { get; private set; }
    public string? NewValue { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private AuditLog()
    {
    }

    public AuditLog(
        Guid? userId,
        string action,
        string entityName,
        Guid? entityId,
        string? oldValue,
        string? newValue)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException(
                "Action is required.",
                nameof(action));

        if (string.IsNullOrWhiteSpace(entityName))
            throw new ArgumentException(
                "Entity name is required.",
                nameof(entityName));

        UserId = userId;
        Action = action;
        EntityName = entityName;
        EntityId = entityId;
        OldValue = oldValue;
        NewValue = newValue;

        CreatedAt = DateTime.UtcNow;
    }
}