
using System.Net.Http.Headers;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SmartOps.Infrastructure.Data;

namespace SmartOps.API.BackgroundServices;

public class AutomationOutboxWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AutomationOutboxWorker> _logger;

    private const int MaxRetries = 5;
    private static readonly TimeSpan PollingInterval =
        TimeSpan.FromSeconds(5);

    public AutomationOutboxWorker(
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpClientFactory,
        ILogger<AutomationOutboxWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Automation Outbox Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var processed = await ProcessNextEventAsync(
                    stoppingToken);

                if (!processed)
                {
                    await Task.Delay(
                        PollingInterval,
                        stoppingToken);
                }
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
                    "Automation Outbox Worker encountered an error.");

                await Task.Delay(
                    PollingInterval,
                    stoppingToken);
            }
        }

        _logger.LogInformation(
            "Automation Outbox Worker stopped.");
    }

    private async Task<bool> ProcessNextEventAsync(
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<SmartOpsDbContext>();

        var now = DateTime.UtcNow;

        var pendingEvent = await dbContext.AutomationOutbox
            .Where(x =>
                x.Status == "Pending" &&
                (x.NextRetryAt == null ||
                 x.NextRetryAt <= now))
            .OrderBy(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (pendingEvent is null)
            return false;

        // Claim the event atomically.
        var claimed = await dbContext.AutomationOutbox
            .Where(x =>
                x.Id == pendingEvent.Id &&
                x.Status == "Pending")
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    x => x.Status, "Processing"),
                cancellationToken);

        if (claimed == 0)
            return true;

        var eventId = pendingEvent.Id;

        try
        {
            var configuration = scope.ServiceProvider
                .GetRequiredService<IConfiguration>();

            var webhookUrl =
                configuration["N8n:WebhookUrl"];

            if (string.IsNullOrWhiteSpace(webhookUrl))
            {
                throw new InvalidOperationException(
                    "n8n webhook URL is not configured.");
            }

            var client = _httpClientFactory
                .CreateClient("N8n");

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                webhookUrl);

            request.Content = new StringContent(
                pendingEvent.Payload,
                Encoding.UTF8,
                "application/json");

            request.Headers.Add(
                "X-SmartOps-Event-Id",
                eventId.ToString());

            request.Headers.Add(
                "X-SmartOps-Event-Type",
                pendingEvent.EventType);
            
            var webhookSecret =
                configuration["N8n:WebhookSecret"];

            if (string.IsNullOrWhiteSpace(webhookSecret))
            {
                throw new InvalidOperationException(
                    "n8n webhook secret is not configured.");
            }

            request.Headers.Add(
                "X-SmartOps-Secret",
                webhookSecret);
            
            using var response = await client.SendAsync(
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var completed = await dbContext.AutomationOutbox
                .Where(x =>
                    x.Id == eventId &&
                    x.Status == "Processing")
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(x => x.Status, "Completed")
                        .SetProperty(x => x.ProcessedAt, DateTime.UtcNow)
                        .SetProperty(x => x.LastError, (string?)null)
                        .SetProperty(x => x.NextRetryAt, (DateTime?)null),
                    cancellationToken);

            if (completed > 0)
            {
                _logger.LogInformation(
                    "Automation event {EventId} delivered successfully.",
                    eventId);
            }
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            var failedEvent = await dbContext.AutomationOutbox
                .FirstOrDefaultAsync(
                    x => x.Id == eventId &&
                         x.Status == "Processing",
                    cancellationToken);

            if (failedEvent is null)
                return true;

            var nextRetryCount = failedEvent.RetryCount + 1;

            if (nextRetryCount >= MaxRetries)
            {
                failedEvent.MarkAsDeadLetter(ex.Message);

                _logger.LogError(
                    ex,
                    "Automation event {EventId} moved to DeadLetter.",
                    eventId);
            }
            else
            {
                var delaySeconds = Math.Min(
                    30 * Math.Pow(2, failedEvent.RetryCount),
                    900);

                var nextRetryAt = DateTime.UtcNow.AddSeconds(
                    delaySeconds);

                failedEvent.MarkAsFailed(
                    ex.Message,
                    nextRetryAt);

                _logger.LogWarning(
                    ex,
                    "Automation event {EventId} failed. Retry {Retry}/{Max}.",
                    eventId,
                    nextRetryCount,
                    MaxRetries);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return true;
    }
}
