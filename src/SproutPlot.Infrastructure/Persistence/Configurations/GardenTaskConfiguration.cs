using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Infrastructure.Persistence.Configurations;

/// <summary>EF Core mapping for <see cref="GardenTask"/>.</summary>
public sealed class GardenTaskConfiguration : IEntityTypeConfiguration<GardenTask>
{
    public void Configure(EntityTypeBuilder<GardenTask> builder)
    {
        builder.ToTable("Tasks", "app");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title).HasMaxLength(150);
        builder.Property(t => t.Notes).HasMaxLength(1000);
        builder.Property(t => t.Type).HasConversion<int>();
        builder.Property(t => t.Status).HasConversion<int>();

        builder.HasIndex(t => new { t.GardenId, t.DueOn });
        builder.HasIndex(t => new { t.Status, t.DueOn });

        builder.HasOne(t => t.Garden)
            .WithMany()
            .HasForeignKey(t => t.GardenId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Plant)
            .WithMany()
            .HasForeignKey(t => t.PlantId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
