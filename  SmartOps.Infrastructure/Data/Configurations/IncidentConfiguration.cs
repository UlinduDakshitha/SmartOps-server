using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartOps.Domain.Entities;

namespace SmartOps.Infrastructure.Data.Configurations;

public class IncidentConfiguration : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> builder)
    {
        builder.ToTable("Incidents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.IncidentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.IncidentNumber)
            .IsUnique();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(x => x.Priority)
            .IsRequired();

        builder.Property(x => x.Severity)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.CreatedByUserId)
            .IsRequired();

        builder.Property(x => x.AssignedTeamId)
            .IsRequired(false);

        builder.Property(x => x.AssignedUserId)
            .IsRequired(false);

        builder.Property(x => x.ResolutionNotes)
            .HasMaxLength(5000)
            .IsRequired(false);

        builder.Property(x => x.RootCause)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.ResolvedAt)
            .IsRequired(false);

        builder.Property(x => x.ClosedAt)
            .IsRequired(false);
    }
}