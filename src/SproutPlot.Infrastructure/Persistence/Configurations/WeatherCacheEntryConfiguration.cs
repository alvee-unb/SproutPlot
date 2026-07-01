using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Infrastructure.Persistence.Configurations;

/// <summary>EF Core mapping for <see cref="WeatherCacheEntry"/>.</summary>
public sealed class WeatherCacheEntryConfiguration : IEntityTypeConfiguration<WeatherCacheEntry>
{
    public void Configure(EntityTypeBuilder<WeatherCacheEntry> builder)
    {
        builder.ToTable("WeatherCache", "app");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.PayloadJson).IsRequired();

        // One cache row per rounded coordinate.
        builder.HasIndex(w => new { w.LatitudeKey, w.LongitudeKey }).IsUnique();
    }
}
