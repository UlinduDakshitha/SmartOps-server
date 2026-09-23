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
    private readonly IIncidentAssignmentRepository _incidentAssignmentRepository;
    private readonly IIncidentHistoryRepository _incidentHistoryRepository;
    private readonly ISLAPolicyRepository _slaPolicyRepository;
    private readonly INotificationService _notificationService;
    private readonly IAuditLogService _auditLogService;
    
    public IncidentService(
        IIncidentRepository incidentRepository,
        IUserRepository userRepository,
        ITeamRepository teamRepository,
        IIncidentCommentRepository incidentCommentRepository,
        IIncidentAssignmentRepository incidentAssignmentRepository,
            IIncidentHistoryRepository incidentHistoryRepository,
            ISLAPolicyRepository slaPolicyRepository,
        INotificationService notificationService,
        IAuditLogService auditLogService)
    {
        _incidentRepository = incidentRepository;
        _userRepository = userRepository;
        _teamRepository = teamRepository;
        _incidentCommentRepository = incidentCommentRepository;
        _incidentAssignmentRepository = incidentAssignmentRepository;
        _incidentHistoryRepository = incidentHistoryRepository;
        _slaPolicyRepository = slaPolicyRepository;
        _notificationService = notificationService;
        _auditLogService = auditLogService;
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
        
        var slaPolicy = await _slaPolicyRepository
            .GetByPriorityAsync(request.Priority);

        if (slaPolicy is null)
        {
            throw new InvalidOperationException(
                $"No SLA policy is configured for priority '{request.Priority}'.");
        }

        var responseDueAt = incident.CreatedAt
            .AddMinutes(slaPolicy.ResponseTimeMinutes);

        var resolutionDueAt = incident.CreatedAt
            .AddMinutes(slaPolicy.ResolutionTimeMinutes);

        incident.SetSLA(
            responseDueAt,
            resolutionDueAt);

        await _incidentRepository.AddAsync(incident);

        var history = new IncidentHistory(
            incident.Id,
            createdByUserId,
            "Incident Created",
            null,
            IncidentStatus.New.ToString());

        await _incidentHistoryRepository.AddAsync(history);
        return MapToResponse(incident);
        
        await _auditLogService.LogAsync(
            createdByUserId,
            "Incident Created",
            "Incident",
            incident.Id,
            null,
            $"IncidentNumber: {incident.IncidentNumber}, " +
            $"Priority: {incident.Priority}, " +
            $"Severity: {incident.Severity}");
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

        var oldValue =
            $"Title: {incident.Title}, " +
            $"Priority: {incident.Priority}, " +
            $"Severity: {incident.Severity}";

        incident.Update(
            request.Title.Trim(),
            request.Description.Trim(),
            request.Priority,
            request.Severity);

        await _incidentRepository.UpdateAsync(incident);

        var newValue =
            $"Title: {incident.Title}, " +
            $"Priority: {incident.Priority}, " +
            $"Severity: {incident.Severity}";

        var history = new IncidentHistory(
            incident.Id,
            incident.CreatedByUserId,
            "Incident Updated",
            oldValue,
            newValue);

        await _incidentHistoryRepository.AddAsync(history);

        return MapToResponse(incident);
        
        await _auditLogService.LogAsync(
            incident.CreatedByUserId,
            "Incident Updated",
            "Incident",
            incident.Id,
            oldValue,
            newValue);
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

        var activeAssignment =
            await _incidentAssignmentRepository
                .GetActiveByIncidentIdAsync(id);

        if (activeAssignment is not null)
        {
            activeAssignment.Unassign();

            await _incidentAssignmentRepository
                .UpdateAsync(activeAssignment);
        }

        var oldValue =
            $"TeamId: {incident.AssignedTeamId}, " +
            $"UserId: {incident.AssignedUserId}, " +
            $"Status: {incident.Status}";

        incident.Assign(
            request.TeamId,
            request.UserId);

        await _incidentRepository.UpdateAsync(incident);

        var assignment = new IncidentAssignment(
            id,
            request.TeamId,
            request.UserId,
            assignedByUserId);

        await _incidentAssignmentRepository.AddAsync(assignment);

        var newValue =
            $"TeamId: {incident.AssignedTeamId}, " +
            $"UserId: {incident.AssignedUserId}, " +
            $"Status: {incident.Status}";

        var history = new IncidentHistory(
            incident.Id,
            assignedByUserId,
            "Incident Assigned",
            oldValue,
            newValue);

        await _incidentHistoryRepository.AddAsync(history); 
        
        await _auditLogService.LogAsync(
            assignedByUserId,
            "Incident Assigned",
            "Incident",
            incident.Id,
            oldValue,
            newValue);
        
        if (request.UserId.HasValue)
        {
            await _notificationService.CreateAsync(
                request.UserId.Value,
                "Incident Assigned",
                $"Incident {incident.IncidentNumber} has been assigned to you.",
                "IncidentAssignment");
        }
    }

    public async Task StartProgressAsync(Guid id)
    {
        var incident = await _incidentRepository.GetByIdAsync(id);

        if (incident is null)
            throw new KeyNotFoundException("Incident not found.");

        var oldValue = incident.Status.ToString();

        incident.StartProgress();

        await _incidentRepository.UpdateAsync(incident);

        var newValue = incident.Status.ToString();

        var history = new IncidentHistory(
            incident.Id,
            incident.AssignedUserId ?? incident.CreatedByUserId,
            "Incident Started",
            oldValue,
            newValue);

        await _incidentHistoryRepository.AddAsync(history);
        
        await _auditLogService.LogAsync(
            incident.AssignedUserId ?? incident.CreatedByUserId,
            "Incident Started",
            "Incident",
            incident.Id,
            oldValue,
            newValue);
    }

    public async Task ResolveAsync(
        Guid id,
        ResolveIncidentRequest request)
    {
        var incident = await _incidentRepository.GetByIdAsync(id);

        if (incident is null)
            throw new KeyNotFoundException("Incident not found.");

        var oldValue = incident.Status.ToString();

        incident.Resolve(
            request.ResolutionNotes.Trim(),
            request.RootCause.Trim());

        await _incidentRepository.UpdateAsync(incident);

        var newValue = incident.Status.ToString();

        var history = new IncidentHistory(
            incident.Id,
            incident.AssignedUserId ?? incident.CreatedByUserId,
            "Incident Resolved",
            oldValue,
            newValue);

        await _incidentHistoryRepository.AddAsync(history);
        
        await _auditLogService.LogAsync(
            incident.AssignedUserId ?? incident.CreatedByUserId,
            "Incident Started",
            "Incident",
            incident.Id,
            oldValue,
            newValue);
        
        if (incident.AssignedUserId.HasValue)
        {
            await _notificationService.CreateAsync(
                incident.AssignedUserId.Value,
                "Incident Resolved",
                $"Incident {incident.IncidentNumber} has been resolved.",
                "IncidentResolution");
        }
    }

    public async Task CloseAsync(Guid id)
    {
        var incident = await _incidentRepository.GetByIdAsync(id);

        if (incident is null)
            throw new KeyNotFoundException("Incident not found.");

        var oldValue = incident.Status.ToString();

        incident.Close();

        await _incidentRepository.UpdateAsync(incident);

        var newValue = incident.Status.ToString();

        var history = new IncidentHistory(
            incident.Id,
            incident.AssignedUserId ?? incident.CreatedByUserId,
            "Incident Closed",
            oldValue,
            newValue);

        await _incidentHistoryRepository.AddAsync(history);
        
        await _auditLogService.LogAsync(
            incident.AssignedUserId ?? incident.CreatedByUserId,
            "Incident Closed",
            "Incident",
            incident.Id,
            oldValue,
            newValue);
        
        if (incident.AssignedUserId.HasValue)
        {
            await _notificationService.CreateAsync(
                incident.AssignedUserId.Value,
                "Incident Closed",
                $"Incident {incident.IncidentNumber} has been closed.",
                "IncidentClosure");
        }
    }

    public async Task CancelAsync(Guid id)
    {
        var incident = await _incidentRepository.GetByIdAsync(id);

        if (incident is null)
            throw new KeyNotFoundException("Incident not found.");

        var oldValue = incident.Status.ToString();

        incident.Cancel();

        await _incidentRepository.UpdateAsync(incident);

        var newValue = incident.Status.ToString();

        var history = new IncidentHistory(
            incident.Id,
            incident.AssignedUserId ?? incident.CreatedByUserId,
            "Incident Cancelled",
            oldValue,
            newValue);

        await _incidentHistoryRepository.AddAsync(history);
        
        if (incident.AssignedUserId.HasValue)
        {
            await _notificationService.CreateAsync(
                incident.AssignedUserId.Value,
                "Incident Cancelled",
                $"Incident {incident.IncidentNumber} has been cancelled.",
                "IncidentCancellation");
        }
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
            ResponseDueAt = incident.ResponseDueAt,
            ResolutionDueAt = incident.ResolutionDueAt,
            RespondedAt = incident.RespondedAt,
            ResponseSlaBreached = incident.IsResponseSlaBreached(),
            ResolutionSlaBreached = incident.IsResolutionSlaBreached(),
            ResolvedAt = incident.ResolvedAt,
            ClosedAt = incident.ClosedAt,
            ResolutionNotes = incident.ResolutionNotes,
            RootCause = incident.RootCause,
            CreatedAt = incident.CreatedAt,
            UpdatedAt = incident.UpdatedAt
        };
    }
}