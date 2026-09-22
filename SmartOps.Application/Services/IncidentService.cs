using SmartOps.Application.DTOs.Incidents;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Application.Interfaces.Services;
using SmartOps.Domain.Entities;
using SmartOps.Domain.Enums;

namespace SmartOps.Application.Services;

public class IncidentService : IIncidentService
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IIncidentCommentRepository _incidentCommentRepository;

    public IncidentService(
        IIncidentRepository incidentRepository,
        IUserRepository userRepository,
        ITeamRepository teamRepository,
        IIncidentCommentRepository incidentCommentRepository)
    {
        _incidentRepository = incidentRepository;
        _userRepository = userRepository;
        _teamRepository = teamRepository;
        _incidentCommentRepository = incidentCommentRepository;
    }

    public async Task<IncidentResponse> GetByIdAsync(Guid id)
    {
        var incident = await _incidentRepository.GetByIdAsync(id);

        if (incident is null)
            throw new KeyNotFoundException("Incident not found.");

        return MapToResponse(incident);
    }

    public async Task<List<IncidentResponse>> GetAllAsync()
    {
        var incidents = await _incidentRepository.GetAllAsync();

        return incidents
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<IncidentResponse> CreateAsync(
        CreateIncidentRequest request,
        Guid createdByUserId)
    {
        var user = await _userRepository.GetByIdAsync(createdByUserId);

        if (user is null)
            throw new KeyNotFoundException("Creating user not found.");

        if (!user.IsActive)
            throw new InvalidOperationException(
                "An inactive user cannot create an incident.");

        var incidentNumber = GenerateIncidentNumber();

        var incident = new Incident(
            incidentNumber,
            request.Title.Trim(),
            request.Description.Trim(),
            request.Priority,
            request.Severity,
            createdByUserId);

        await _incidentRepository.AddAsync(incident);

        return MapToResponse(incident);
    }

    public async Task<IncidentResponse> UpdateAsync(
        Guid id,
        UpdateIncidentRequest request)
    {
        var incident = await _incidentRepository.GetByIdAsync(id);

        if (incident is null)
            throw new KeyNotFoundException("Incident not found.");

        if (incident.Status == IncidentStatus.Closed ||
            incident.Status == IncidentStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "A closed or cancelled incident cannot be updated.");
        }

        incident.Update(
            request.Title.Trim(),
            request.Description.Trim(),
            request.Priority,
            request.Severity);

        await _incidentRepository.UpdateAsync(incident);

        return MapToResponse(incident);
    }

    public async Task AssignAsync(
        Guid id,
        AssignIncidentRequest request,
        Guid assignedByUserId)
    {
        var incident = await _incidentRepository.GetByIdAsync(id);

        if (incident is null)
            throw new KeyNotFoundException("Incident not found.");

        var assigningUser = await _userRepository
            .GetByIdAsync(assignedByUserId);

        if (assigningUser is null)
            throw new KeyNotFoundException(
                "Assigning user not found.");

        if (!assigningUser.IsActive)
            throw new InvalidOperationException(
                "An inactive user cannot assign an incident.");

        var team = await _teamRepository
            .GetByIdAsync(request.TeamId);

        if (team is null)
            throw new KeyNotFoundException("Team not found.");

        if (request.UserId.HasValue)
        {
            var assignedUser = await _userRepository
                .GetByIdAsync(request.UserId.Value);

            if (assignedUser is null)
                throw new KeyNotFoundException(
                    "Assigned user not found.");

            if (!assignedUser.IsActive)
                throw new InvalidOperationException(
                    "Cannot assign an incident to an inactive user.");
        }

        incident.Assign(
            request.TeamId,
            request.UserId);

        await _incidentRepository.UpdateAsync(incident);
    }

    public async Task StartProgressAsync(Guid id)
    {
        var incident = await _incidentRepository.GetByIdAsync(id);

        if (incident is null)
            throw new KeyNotFoundException("Incident not found.");

        incident.StartProgress();

        await _incidentRepository.UpdateAsync(incident);
    }

    public async Task ResolveAsync(
        Guid id,
        ResolveIncidentRequest request)
    {
        var incident = await _incidentRepository.GetByIdAsync(id);

        if (incident is null)
            throw new KeyNotFoundException("Incident not found.");

        incident.Resolve(
            request.ResolutionNotes.Trim(),
            request.RootCause.Trim());

        await _incidentRepository.UpdateAsync(incident);
    }

    public async Task CloseAsync(Guid id)
    {
        var incident = await _incidentRepository.GetByIdAsync(id);

        if (incident is null)
            throw new KeyNotFoundException("Incident not found.");

        incident.Close();

        await _incidentRepository.UpdateAsync(incident);
    }

    public async Task CancelAsync(Guid id)
    {
        var incident = await _incidentRepository.GetByIdAsync(id);

        if (incident is null)
            throw new KeyNotFoundException("Incident not found.");

        incident.Cancel();

        await _incidentRepository.UpdateAsync(incident);
    }

    public async Task<IncidentCommentResponse> AddCommentAsync(
        Guid incidentId,
        AddIncidentCommentRequest request,
        Guid userId)
    {
        var incident = await _incidentRepository
            .GetByIdAsync(incidentId);

        if (incident is null)
            throw new KeyNotFoundException("Incident not found.");

        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
            throw new KeyNotFoundException("User not found.");

        if (!user.IsActive)
            throw new InvalidOperationException(
                "An inactive user cannot add a comment.");

        var comment = new IncidentComment(
            incidentId,
            userId,
            request.Comment.Trim());

        await _incidentCommentRepository.AddAsync(comment);

        return new IncidentCommentResponse
        {
            Id = comment.Id,
            IncidentId = comment.IncidentId,
            UserId = comment.UserId,
            Comment = comment.Comment
        };
    }

    private static string GenerateIncidentNumber()
    {
        return $"INC-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    }

    private static IncidentResponse MapToResponse(
        Incident incident)
    {
        return new IncidentResponse
        {
            Id = incident.Id,
            IncidentNumber = incident.IncidentNumber,
            Title = incident.Title,
            Description = incident.Description,
            Priority = incident.Priority,
            Severity = incident.Severity,
            Status = incident.Status,
            CreatedByUserId = incident.CreatedByUserId,
            AssignedTeamId = incident.AssignedTeamId,
            AssignedUserId = incident.AssignedUserId,
            ResolvedAt = incident.ResolvedAt,
            ClosedAt = incident.ClosedAt,
            ResolutionNotes = incident.ResolutionNotes,
            RootCause = incident.RootCause,
            CreatedAt = incident.CreatedAt,
            UpdatedAt = incident.UpdatedAt
        };
    }
}