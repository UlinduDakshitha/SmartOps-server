using SmartOps.Domain.Common;
using SmartOps.Domain.Enums;

namespace SmartOps.Domain.Entities;

public class SLAPolicy : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;

    public Priority Priority { get; private set; }

    public int ResponseTimeMinutes { get; private set; }

    public int ResolutionTimeMinutes { get; private set; }

    private SLAPolicy()
    {
    }

    public SLAPolicy(
        string name,
        Priority priority,
        int responseTimeMinutes,
        int resolutionTimeMinutes)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "SLA policy name is required.",
                nameof(name));

        if (responseTimeMinutes <= 0)
            throw new ArgumentException(
                "Response time must be greater than zero.",
                nameof(responseTimeMinutes));

        if (resolutionTimeMinutes <= 0)
            throw new ArgumentException(
                "Resolution time must be greater than zero.",
                nameof(resolutionTimeMinutes));

        Name = name;
        Priority = priority;
        ResponseTimeMinutes = responseTimeMinutes;
        ResolutionTimeMinutes = resolutionTimeMinutes;
    }

    public void Update(
        string name,
        Priority priority,
        int responseTimeMinutes,
        int resolutionTimeMinutes)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "SLA policy name is required.",
                nameof(name));

        if (responseTimeMinutes <= 0)
            throw new ArgumentException(
                "Response time must be greater than zero.",
                nameof(responseTimeMinutes));

        if (resolutionTimeMinutes <= 0)
            throw new ArgumentException(
                "Resolution time must be greater than zero.",
                nameof(resolutionTimeMinutes));

        Name = name;
        Priority = priority;
        ResponseTimeMinutes = responseTimeMinutes;
        ResolutionTimeMinutes = resolutionTimeMinutes;

        UpdatedAt = DateTime.UtcNow;
    }
}