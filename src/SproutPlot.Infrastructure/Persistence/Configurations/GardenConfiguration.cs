using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Infrastructure.Persistence.Configurations;

/// <summary>EF Core mapping for <see cref="Garden"/>.</summary>
public sealed class GardenConfiguration : IEntityTypeConfiguration<Garden>
{
    public void Configure(EntityTypeBuilder<Garden> builder)
    {
        builder.ToTable("Gardens", "app");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name).IsRequired().HasMaxLength(120);
        builder.Property(g => g.Location).HasMaxLength(200);
        builder.Property(g => g.Size).HasMaxLength(100);
        builder.Property(g => g.Notes).HasMaxLength(2000);

        // A user has many gardens; deleting the user removes their gardens.
        builder.HasIndex(g => g.OwnerId);
        builder.HasOne<Identity.ApplicationUser>()
            .WithMany()
            .HasForeignKey(g => g.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
