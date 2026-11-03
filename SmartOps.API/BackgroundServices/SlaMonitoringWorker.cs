
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartOps.Domain.Entities;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Infrastructure.Data;

namespace SmartOps.API.BackgroundServices;

public class SlaMonitoringWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SlaMonitoringWorker> _logger;

    private static readonly TimeSpan PollingInterval =
        TimeSpan.FromMinutes(1);

    public SlaMonitoringWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<SlaMonitoringWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation("SLA Monitoring Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await MonitorIncidentsAsync(stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "SLA Monitoring Worker encountered an error.");
            }

            try
            {
                await Task.Delay(PollingInterval, stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation("SLA Monitoring Worker stopped.");
    }

    private async Task MonitorIncidentsAsync(
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<SmartOpsDbContext>();

        var incidents = await db.Incidents
            .AsNoTracking()
            .Where(x =>
                (x.ResponseDueAt.HasValue &&
                 (!x.RespondedAt.HasValue ||
                  x.RespondedAt > x.ResponseDueAt))
                ||
                (x.ResolutionDueAt.HasValue &&
                 (!x.ResolvedAt.HasValue ||
                  x.ResolvedAt > x.ResolutionDueAt)))
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;

        _logger.LogInformation(
            "SLA Monitoring checked {Count} incidents.",
            incidents.Count);

        foreach (var incident in incidents)
        {
            if (incident.ResponseDueAt.HasValue &&
                (!incident.RespondedAt.HasValue ||
                 incident.RespondedAt > incident.ResponseDueAt))
            {
                var percentage = CalculateSlaPercentage(
                    now - incident.CreatedAt,
                    incident.ResponseDueAt.Value - incident.CreatedAt);

                await CreateThresholdEventsAsync(
                    db, incident, "Response", percentage,
                    cancellationToken);
            }

            if (incident.ResolutionDueAt.HasValue &&
                (!incident.ResolvedAt.HasValue ||
                 incident.ResolvedAt > incident.ResolutionDueAt))
            {
                var percentage = CalculateSlaPercentage(
                    now - incident.CreatedAt,
                    incident.ResolutionDueAt.Value - incident.CreatedAt);

                await CreateThresholdEventsAsync(
                    db, incident, "Resolution", percentage,
                    cancellationToken);
            }
        }
    }

    private async Task CreateThresholdEventsAsync(
        SmartOpsDbContext db,
        Incident incident,
        string slaType,
        double percentage,
        CancellationToken cancellationToken)
    {
        int[] thresholds = { 50, 80, 100 };

        foreach (var threshold in thresholds)
        {
            if (percentage < threshold)
                continue;

            string? recipientEmail = null;
            string? recipientName = null;
            string recipientRole;

            if (threshold == 50)
            {
                recipientRole = "Engineer";

                if (!incident.AssignedUserId.HasValue)
                    continue;

                var engineer = await db.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.Id == incident.AssignedUserId.Value
                             && x.IsActive,
                        cancellationToken);

                if (engineer == null)
                    continue;

                recipientEmail = engineer.Email;
                recipientName = engineer.FullName;
            }
            else if (threshold == 80)
            {
                recipientRole = "Team Lead";

                if (!incident.AssignedTeamId.HasValue)
                    continue;

                var team = await db.Teams
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.Id == incident.AssignedTeamId.Value,
                        cancellationToken);

                if (team?.TeamLeadId == null)
                    continue;

                var teamLead = await db.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.Id == team.TeamLeadId.Value
                             && x.IsActive,
                        cancellationToken);

                if (teamLead == null)
                    continue;

                recipientEmail = teamLead.Email;
                recipientName = teamLead.FullName;
            }
            else
            {
                recipientRole = "Manager";
            }

            var deduplicationKey =
                $"{incident.Id}:{slaType}:{threshold}";

            var eventType = threshold == 100
                ? "SLA.Breached"
                : "SLA.Warning";

            if (threshold < 100)
            {
                var payload = BuildPayload(
                    incident, slaType, threshold,
                    percentage, recipientRole,
                    recipientEmail!, recipientName);

                var outbox = new AutomationOutbox(
                    eventType,
                    payload,
                    deduplicationKey);

                var inserted = await db.AddOutboxEventIfNotExistsAsync(
                    outbox, cancellationToken);

                if (inserted)
                {
                    _logger.LogInformation(
                        "SLA {Threshold}% event created for {IncidentNumber}.",
                        threshold, incident.IncidentNumber);
                }
            }
            else
            {
                var managers = await (
                    from user in db.Users.AsNoTracking()
                    join userRole in db.UserRoles.AsNoTracking()
                        on user.Id equals userRole.UserId
                    join role in db.Roles.AsNoTracking()
                        on userRole.RoleId equals role.Id
                    where user.IsActive && role.Name == "Manager"
                    select new
                    {
                        user.Email,
                        user.FullName
                    })
                    .ToListAsync(cancellationToken);

                if (managers.Count == 0)
                {
                    _logger.LogWarning(
                        "No active Managers found for {IncidentNumber}.",
                        incident.IncidentNumber);

                    continue;
                }

                var managerPayload = JsonSerializer.Serialize(new
                {
                    eventId = Guid.NewGuid(),
                    eventType,
                    incidentId = incident.Id,
                    incidentNumber = incident.IncidentNumber,
                    title = incident.Title,
                    priority = incident.Priority.ToString(),
                    severity = incident.Severity.ToString(),
                    status = incident.Status.ToString(),
                    slaType,
                    thresholdPercent = threshold,
                    currentPercentage = Math.Round(percentage, 2),
                    recipientRole,
                    recipients = managers,
                    message = $"{slaType} SLA breached for {incident.IncidentNumber}"
                });

                var managerOutbox = new AutomationOutbox(
                    eventType,
                    managerPayload,
                    deduplicationKey);

                var inserted = await db.AddOutboxEventIfNotExistsAsync(
                    managerOutbox, cancellationToken);

                if (inserted)
                {
                    _logger.LogInformation(
                        "SLA breach event created for {IncidentNumber}.",
                        incident.IncidentNumber);
                }
            }
        }
    }

    private static string BuildPayload(
        Incident incident,
        string slaType,
        int threshold,
        double percentage,
        string recipientRole,
        string recipientEmail,
        string? recipientName)
    {
        return JsonSerializer.Serialize(new
        {
            eventId = Guid.NewGuid(),
            eventType = threshold == 100
                ? "SLA.Breached"
                : "SLA.Warning",
            incidentId = incident.Id,
            incidentNumber = incident.IncidentNumber,
            title = incident.Title,
            priority = incident.Priority.ToString(),
            severity = incident.Severity.ToString(),
            status = incident.Status.ToString(),
            slaType,
            thresholdPercent = threshold,
            currentPercentage = Math.Round(percentage, 2),
            recipientRole,
            recipientEmail,
            recipientName,
            message = $"{slaType} SLA reached {threshold}% for {incident.IncidentNumber}"
        });
    }

    private static double CalculateSlaPercentage(
        TimeSpan elapsedTime,
        TimeSpan allowedTime)
    {
        if (allowedTime.TotalSeconds <= 0)
            return 0;

        return Math.Round(
            Math.Max(0,
                elapsedTime.TotalSeconds /
                allowedTime.TotalSeconds * 100),
            2);
    }
}
