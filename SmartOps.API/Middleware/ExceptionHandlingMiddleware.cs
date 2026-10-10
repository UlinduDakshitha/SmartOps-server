
using System.Net;
using System.Text.Json;

namespace SmartOps.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogError(
                    ex,
                    "An exception occurred after the response started. " +
                    "Request: {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                throw;
            }

            _logger.LogError(
                ex,
                "Unhandled exception. Request: {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            KeyNotFoundException =>
                ((int)HttpStatusCode.NotFound,
                 exception.Message),

            UnauthorizedAccessException =>
                ((int)HttpStatusCode.Forbidden,
                 exception.Message),

            ArgumentException =>
                ((int)HttpStatusCode.BadRequest,
                 exception.Message),

            InvalidOperationException =>
                ((int)HttpStatusCode.Conflict,
                 exception.Message),

            _ =>
                ((int)HttpStatusCode.InternalServerError,
                 "An unexpected error occurred.")
        };

        if (statusCode >= 500 && _environment.IsDevelopment())
        {
            message = exception.Message;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            statusCode,
            message,
            timestamp = DateTimeOffset.UtcNow
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}
