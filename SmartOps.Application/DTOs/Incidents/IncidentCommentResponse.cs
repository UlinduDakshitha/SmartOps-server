namespace SmartOps.Application.DTOs.Incidents;

public class IncidentCommentResponse
{
    public Guid Id { get; set; }

    public Guid IncidentId { get; set; }

    public Guid UserId { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}