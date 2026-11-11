using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartOps.Domain.Entities;

namespace SmartOps.Infrastructure.Data.Configurations;

public class SLAPolicyConfiguration : IEntityTypeConfiguration<SLAPolicy>
{
    public void Configure(EntityTypeBuilder<SLAPolicy> builder)
    {
        builder.ToTable("SLAPolicies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Priority)
            .IsRequired();

        builder.Property(x => x.ResponseTimeMinutes)
            .IsRequired();

        builder.Property(x => x.ResolutionTimeMinutes)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.HasIndex(x => x.Priority)
            .IsUnique();
    }
}