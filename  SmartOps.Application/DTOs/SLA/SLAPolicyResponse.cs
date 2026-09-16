using SmartOps.Domain.Enums;

namespace SmartOps.Application.DTOs.SLA;

public class SLAPolicyResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Priority Priority { get; set; }

    public int ResponseTimeMinutes { get; set; }

    public int ResolutionTimeMinutes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}