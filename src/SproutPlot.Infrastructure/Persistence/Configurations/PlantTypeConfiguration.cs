using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SproutPlot.Domain.Entities;
using SproutPlot.Domain.Enums;

namespace SproutPlot.Infrastructure.Persistence.Configurations;

/// <summary>EF Core mapping and seed data for <see cref="PlantType"/>.</summary>
public sealed class PlantTypeConfiguration : IEntityTypeConfiguration<PlantType>
{
    // Fixed timestamp for deterministic seed data (audit stamping does not run for HasData).
    private static readonly DateTime SeedTimestamp = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<PlantType> builder)
    {
        builder.ToTable("PlantTypes", "app");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Category).HasConversion<int>();

        builder.HasIndex(t => t.Name).IsUnique();

        builder.HasData(
            Seed(1, "Tomato", PlantTypeCategory.Vegetable),
            Seed(2, "Pepper", PlantTypeCategory.Vegetable),
            Seed(3, "Lettuce", PlantTypeCategory.Vegetable),
            Seed(4, "Carrot", PlantTypeCategory.Vegetable),
            Seed(5, "Cucumber", PlantTypeCategory.Vegetable),
            Seed(6, "Zucchini", PlantTypeCategory.Vegetable),
            Seed(7, "Onion", PlantTypeCategory.Vegetable),
            Seed(8, "Potato", PlantTypeCategory.Vegetable),
            Seed(9, "Basil", PlantTypeCategory.Herb),
            Seed(10, "Mint", PlantTypeCategory.Herb),
            Seed(11, "Rosemary", PlantTypeCategory.Herb),
            Seed(12, "Thyme", PlantTypeCategory.Herb),
            Seed(13, "Cilantro", PlantTypeCategory.Herb),
            Seed(14, "Strawberry", PlantTypeCategory.Fruit),
            Seed(15, "Blueberry", PlantTypeCategory.Fruit),
            Seed(16, "Rose", PlantTypeCategory.Flower),
            Seed(17, "Marigold", PlantTypeCategory.Flower),
            Seed(18, "Sunflower", PlantTypeCategory.Flower),
            Seed(19, "Tulip", PlantTypeCategory.Flower));
    }

    private static PlantType Seed(int n, string name, PlantTypeCategory category) => new()
    {
        Id = new Guid($"a0000000-0000-0000-0000-{n:D12}"),
        Name = name,
        Category = category,
        CreatedAtUtc = SeedTimestamp,
    };
}
