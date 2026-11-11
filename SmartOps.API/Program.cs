
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

using SmartOps.Infrastructure.Data;
using SmartOps.Application.Interfaces;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Application.Interfaces.Services;
using SmartOps.Application.Services;
using SmartOps.Infrastructure.Identity;
using SmartOps.Infrastructure.Repositories;
using SmartOps.API.Middleware;
using SmartOps.Application.Validators.Auth;
using SmartOps.Domain.Common;
using SmartOps.API.Hubs;
using SmartOps.API.Services;

using FluentValidation;
using Serilog;
using System.Text;
using SmartOps.API.BackgroundServices;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/smartops-.log",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Controllers and API
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

// CORS for Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// PostgreSQL Database
builder.Services.AddDbContext<SmartOpsDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<SmartOpsDbContext>();

// JWT Authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException(
                "JWT secret key is not configured.");

        var issuer = builder.Configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "JWT issuer is not configured.");

        var audience = builder.Configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "JWT audience is not configured.");

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey)),

                ValidateIssuer = true,
                ValidIssuer = issuer,

                ValidateAudience = true,
                ValidAudience = audience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
    });

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });
});

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole(RoleNames.Administrator));

    options.AddPolicy("TeamLeadOrAdmin", policy =>
        policy.RequireRole(
            RoleNames.Administrator,
            RoleNames.TeamLead));

    options.AddPolicy("SupportOrTeamLeadOrAdmin", policy =>
        policy.RequireRole(
            RoleNames.Administrator,
            RoleNames.TeamLead,
            RoleNames.SupportAgent));

    options.AddPolicy("IncidentHandler", policy =>
        policy.RequireRole(
            RoleNames.Administrator,
            RoleNames.TeamLead,
            RoleNames.SupportAgent,
            RoleNames.Engineer));

    options.AddPolicy("ManagerOrAdmin", policy =>
        policy.RequireRole(
            RoleNames.Administrator,
            RoleNames.Manager));
});

// Validation
builder.Services
    .AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

// Dependency Injection

// Identity and Authentication
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

// Teams
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
builder.Services.AddScoped<ITeamService, TeamService>();

// Incidents
builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
builder.Services.AddScoped<IIncidentCommentRepository, IncidentCommentRepository>();
builder.Services.AddScoped<IIncidentService, IncidentService>();
builder.Services.AddScoped<IIncidentHistoryRepository, IncidentHistoryRepository>();
builder.Services.AddScoped<IIncidentHistoryService, IncidentHistoryService>();
builder.Services.AddScoped<IIncidentAssignmentRepository, IncidentAssignmentRepository>();

// Audit Logs
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// Dashboard
builder.Services.AddScoped<IDashboardService, DashboardService>();

// SLA
builder.Services.AddScoped<ISLAPolicyRepository, SLAPolicyRepository>();
builder.Services.AddScoped<ISLAPolicyService, SLAPolicyService>();

// Notifications
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationPublisher, NotificationPublisher>();

builder.Services.AddHttpClient("N8n", client =>
{
    client.Timeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddHostedService<AutomationOutboxWorker>();
builder.Services.AddHostedService<SlaMonitoringWorker>();

var app = builder.Build();

// Database Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var dbContext =
        services.GetRequiredService<SmartOpsDbContext>();

    var configuration =
        services.GetRequiredService<IConfiguration>();

    var passwordHasher =
        services.GetRequiredService<IPasswordHasher>();

    await DbSeeder.SeedAsync(
        dbContext,
        configuration,
        passwordHasher);
}

// HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("AngularClient");

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

// SignalR
app.MapHub<NotificationHub>("/hubs/notifications");

// IMPORTANT: Map controller endpoints
app.MapControllers();

// Health Check
app.MapHealthChecks("/health");

app.Run();
