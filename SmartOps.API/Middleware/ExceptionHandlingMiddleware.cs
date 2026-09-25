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
            _logger.LogError(
                ex,
                "An unhandled exception occurred. Request: {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var statusCode = exception switch
        {
            KeyNotFoundException =>
                (int)HttpStatusCode.NotFound,

            UnauthorizedAccessException =>
                (int)HttpStatusCode.Unauthorized,

            ArgumentException =>
                (int)HttpStatusCode.BadRequest,

            InvalidOperationException =>
                (int)HttpStatusCode.Conflict,

            _ =>
                (int)HttpStatusCode.InternalServerError
        };

        var message = exception switch
        {
            KeyNotFoundException =>
                exception.Message,

            UnauthorizedAccessException =>
                exception.Message,

            ArgumentException =>
                exception.Message,

            InvalidOperationException =>
                exception.Message,

            _ when _environment.IsDevelopment() =>
                exception.Message,

            _ =>
                "An unexpected error occurred."
        };

        var response = new
        {
            statusCode,
            message,
            timestamp = DateTime.UtcNow
        };

        context.Response.StatusCode = statusCode;

        context.Response.ContentType =
            "application/json";

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}