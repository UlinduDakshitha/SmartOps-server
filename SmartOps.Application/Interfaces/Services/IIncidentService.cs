using SmartOps.Application.DTOs.Incidents;

namespace SmartOps.Application.Interfaces.Services;

public interface IIncidentService
{
    Task<IncidentResponse> GetByIdAsync(Guid id);

    Task<List<IncidentResponse>> GetAllAsync();

    Task<IncidentListResponse> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? status = null,
        string? priority = null,
        string? severity = null);

    Task<IncidentResponse> CreateAsync(
        CreateIncidentRequest request,
        Guid createdByUserId);

    Task<IncidentResponse> UpdateAsync(
        Guid id,
        UpdateIncidentRequest request,
        Guid updatedByUserId);

    Task AssignAsync(
        Guid id,
        AssignIncidentRequest request,
        Guid assignedByUserId);

    Task StartProgressAsync(
        Guid id,
        Guid startedByUserId);

    Task ResolveAsync(
        Guid id,
        ResolveIncidentRequest request,
        Guid resolvedByUserId);

    Task CloseAsync(
        Guid id,
        Guid closedByUserId);

    Task CancelAsync(
        Guid id,
        Guid cancelledByUserId);

    Task<IncidentCommentResponse> AddCommentAsync(
        Guid incidentId,
        AddIncidentCommentRequest request,
        Guid userId);
}