
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartOps.Application.Interfaces.Repositories;

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
        _logger.LogInformation(
            "SLA Monitoring Worker started.");

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
                    ex,
                    "SLA Monitoring Worker encountered an error.");
            }

            try
            {
                await Task.Delay(
                    PollingInterval,
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation(
            "SLA Monitoring Worker stopped.");
    }

    private async Task MonitorIncidentsAsync(
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var incidentRepository = scope.ServiceProvider
            .GetRequiredService<IIncidentRepository>();

        var incidents = await incidentRepository
            .GetOpenIncidentsForSlaMonitoringAsync();

        _logger.LogInformation(
            "SLA Monitoring checked {IncidentCount} open incidents.",
            incidents.Count);

        foreach (var incident in incidents)
        {
            var now = DateTime.UtcNow;

            // Response SLA calculation
            if (incident.ResponseDueAt.HasValue &&
                !incident.RespondedAt.HasValue)
            {
                var responseDuration =
                    incident.ResponseDueAt.Value - incident.CreatedAt;

                var responseElapsed =
                    now - incident.CreatedAt;

                var responsePercentage = CalculateSlaPercentage(
                    responseElapsed,
                    responseDuration);

                _logger.LogInformation(
                    "Incident {IncidentNumber} Response SLA: {Percentage:F2}%",
                    incident.IncidentNumber,
                    responsePercentage);
            }

            // Resolution SLA calculation
            if (incident.ResolutionDueAt.HasValue &&
                !incident.ResolvedAt.HasValue)
            {
                var resolutionDuration =
                    incident.ResolutionDueAt.Value - incident.CreatedAt;

                var resolutionElapsed =
                    now - incident.CreatedAt;

                var resolutionPercentage = CalculateSlaPercentage(
                    resolutionElapsed,
                    resolutionDuration);

                _logger.LogInformation(
                    "Incident {IncidentNumber} Resolution SLA: {Percentage:F2}%",
                    incident.IncidentNumber,
                    resolutionPercentage);
            }
        }
    }
    private static double CalculateSlaPercentage(
        TimeSpan elapsedTime,
        TimeSpan allowedTime)
    {
        if (allowedTime.TotalSeconds <= 0)
            return 0;

        var percentage =
            (elapsedTime.TotalSeconds / allowedTime.TotalSeconds) * 100;

        return Math.Round(
            Math.Max(0, percentage),
            2);
    }
}
