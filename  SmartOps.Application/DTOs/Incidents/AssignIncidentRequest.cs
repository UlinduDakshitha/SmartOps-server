namespace SmartOps.Application.DTOs.Incidents;

public class AssignIncidentRequest
{
    public Guid TeamId { get; set; }

    public Guid? UserId { get; set; }
}