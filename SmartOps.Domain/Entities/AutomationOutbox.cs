namespace SmartOps.Domain.Entities;

public class AutomationOutbox
{
    public Guid Id { get; private set; }

    public string EventType { get; private set; } = string.Empty;

    public string Payload { get; private set; } = string.Empty;

    public string Status { get; private set; } = "Pending";

    public int RetryCount { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? ProcessedAt { get; private set; }

    public DateTime? NextRetryAt { get; private set; }

    public string? LastError { get; private set; }

    public string? DeduplicationKey { get; private set; }
    
    private AutomationOutbox()
    {
    }

    public AutomationOutbox(
        string eventType,
        string payload,
        string? deduplicationKey = null)
    {
        Id = Guid.NewGuid();
        EventType = eventType;
        Payload = payload;
        DeduplicationKey = deduplicationKey;
        Status = "Pending";
        RetryCount = 0;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsProcessing()
    {
        Status = "Processing";
    }

    public void MarkAsCompleted()
    {
        Status = "Completed";
        ProcessedAt = DateTime.UtcNow;
        LastError = null;
        NextRetryAt = null;
    }

    public void MarkAsFailed(string error, DateTime nextRetryAt)
    {
        Status = "Pending";
        RetryCount++;
        LastError = error;
        NextRetryAt = nextRetryAt;
    }

    public void MarkAsDeadLetter(string error)
    {
        Status = "DeadLetter";
        RetryCount++;
        LastError = error;
        NextRetryAt = null;
    }
}