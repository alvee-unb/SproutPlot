using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SproutPlot.Domain.Common;
using SproutPlot.Domain.Entities;
using SproutPlot.Infrastructure.Identity;

namespace SproutPlot.Infrastructure.Persistence;

/// <summary>
/// Primary EF Core database context. Extends the Identity context so users,
/// roles and their related tables are managed here. Feature tables live under
/// the "app" schema; Identity tables under "identity".
/// </summary>
public sealed class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Garden> Gardens => Set<Garden>();

    public DbSet<Plant> Plants => Set<Plant>();

    public DbSet<PlantType> PlantTypes => Set<PlantType>();

    public DbSet<WeatherCacheEntry> WeatherCacheEntries => Set<WeatherCacheEntry>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Keep Identity tables in their own schema.
        builder.HasDefaultSchema("identity");

        // Apply IEntityTypeConfiguration<T> classes in this assembly.
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyAuditTimestamps();
        return base.SaveChanges();
    }

    /// <summary>Stamps created/updated timestamps on tracked <see cref="BaseEntity"/> instances.</summary>
    private void ApplyAuditTimestamps()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAtUtc = now;
                    break;
            }
        }
    }
}
