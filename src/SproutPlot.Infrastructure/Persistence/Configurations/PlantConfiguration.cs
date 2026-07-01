using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Infrastructure.Persistence.Configurations;

/// <summary>EF Core mapping for <see cref="Plant"/>.</summary>
public sealed class PlantConfiguration : IEntityTypeConfiguration<Plant>
{
    public void Configure(EntityTypeBuilder<Plant> builder)
    {
        builder.ToTable("Plants", "app");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(120);
        builder.Property(p => p.Variety).HasMaxLength(100);
        builder.Property(p => p.Notes).HasMaxLength(2000);
        builder.Property(p => p.Quantity).HasDefaultValue(1);
        builder.Property(p => p.Status).HasConversion<int>();

        builder.HasIndex(p => p.GardenId);

        // Deleting a garden removes its plants.
        builder.HasOne(p => p.Garden)
            .WithMany()
            .HasForeignKey(p => p.GardenId)
            .OnDelete(DeleteBehavior.Cascade);

        // Plant type is optional reference data; block deleting a type still in use.
        builder.HasOne(p => p.PlantType)
            .WithMany()
            .HasForeignKey(p => p.PlantTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
