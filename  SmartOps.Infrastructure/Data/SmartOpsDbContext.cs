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

        modelBuilder.Entity<UserRole>()
            .HasKey(x => new { x.UserId, x.RoleId });

        modelBuilder.Entity<TeamMember>()
            .HasKey(x => new { x.TeamId, x.UserId });
    }
}