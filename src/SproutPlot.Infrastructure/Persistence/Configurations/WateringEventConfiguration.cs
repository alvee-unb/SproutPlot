using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Infrastructure.Persistence.Configurations;

/// <summary>EF Core mapping for <see cref="WateringEvent"/>.</summary>
public sealed class WateringEventConfiguration : IEntityTypeConfiguration<WateringEvent>
{
    public void Configure(EntityTypeBuilder<WateringEvent> builder)
    {
        builder.ToTable("WateringEvents", "app");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Notes).HasMaxLength(1000);

        // History queries filter by garden and order by time.
        builder.HasIndex(w => new { w.GardenId, w.WateredAtUtc });

        builder.HasOne(w => w.Garden)
            .WithMany()
            .HasForeignKey(w => w.GardenId)
            .OnDelete(DeleteBehavior.Cascade);

        // Keep the watering record if its plant is deleted; just drop the link.
        builder.HasOne(w => w.Plant)
            .WithMany()
            .HasForeignKey(w => w.PlantId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
