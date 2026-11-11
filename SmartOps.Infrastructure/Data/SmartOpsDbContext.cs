using Microsoft.EntityFrameworkCore;
using SmartOps.Domain.Entities;

namespace SmartOps.Infrastructure.Data;

public class SmartOpsDbContext : DbContext
{
    public SmartOpsDbContext(DbContextOptions<SmartOpsDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();

    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<IncidentComment> IncidentComments => Set<IncidentComment>();
    public DbSet<IncidentHistory> IncidentHistories => Set<IncidentHistory>();
    public DbSet<IncidentAssignment> IncidentAssignments => Set<IncidentAssignment>();

    public DbSet<SLAPolicy> SLAPolicies => Set<SLAPolicy>();

    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(SmartOpsDbContext).Assembly);

        // Automation Outbox - Prevent duplicate SLA events
        modelBuilder.Entity<AutomationOutbox>()
            .HasIndex(x => x.DeduplicationKey)
            .IsUnique()
            .HasFilter("\"DeduplicationKey\" IS NOT NULL");
    }
   
    public async Task<bool> AddOutboxEventIfNotExistsAsync(
        AutomationOutbox outbox,
        CancellationToken cancellationToken = default)
    {
        var affectedRows = await Database.ExecuteSqlInterpolatedAsync(
            $"""
             INSERT INTO "AutomationOutbox"
             ("Id", "EventType", "Payload", "Status",
              "RetryCount", "CreatedAt", "DeduplicationKey")
             VALUES
             ({outbox.Id}, {outbox.EventType}, {outbox.Payload},
              {outbox.Status}, {outbox.RetryCount}, {outbox.CreatedAt},
              {outbox.DeduplicationKey})
             ON CONFLICT ("DeduplicationKey")
             WHERE "DeduplicationKey" IS NOT NULL
             DO NOTHING
             """,
            cancellationToken);

        return affectedRows > 0;
    }
    public DbSet<AutomationOutbox> AutomationOutbox => Set<AutomationOutbox>();
}