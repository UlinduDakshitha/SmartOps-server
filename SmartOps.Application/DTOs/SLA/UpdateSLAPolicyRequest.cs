using SmartOps.Domain.Enums;

namespace SmartOps.Application.DTOs.SLA;

public class UpdateSLAPolicyRequest
{
    public string Name { get; set; } = string.Empty;

    public Priority Priority { get; set; }

    public int ResponseTimeMinutes { get; set; }

    public int ResolutionTimeMinutes { get; set; }
}