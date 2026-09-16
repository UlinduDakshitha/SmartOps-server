using SmartOps.Application.DTOs.Incidents;

namespace SmartOps.Application.Interfaces.Services;

public interface IIncidentService
{
    Task<IncidentResponse> GetByIdAsync(Guid id);

    Task<List<IncidentResponse>> GetAllAsync();

    Task<IncidentResponse> CreateAsync(
        CreateIncidentRequest request,
        Guid createdByUserId);

    Task<IncidentResponse> UpdateAsync(
        Guid id,
        UpdateIncidentRequest request);

    Task AssignAsync(
        Guid id,
        AssignIncidentRequest request,
        Guid assignedByUserId);

    Task StartProgressAsync(Guid id);

    Task ResolveAsync(
        Guid id,
        ResolveIncidentRequest request);

    Task CloseAsync(Guid id);

    Task CancelAsync(Guid id);

    Task<IncidentCommentResponse> AddCommentAsync(
        Guid incidentId,
        AddIncidentCommentRequest request,
        Guid userId);
}