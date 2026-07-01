using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SproutPlot.Infrastructure.Identity;

namespace SproutPlot.Infrastructure.Persistence;

/// <summary>
/// Primary EF Core database context. Extends the Identity context so users,
/// roles and their related tables are managed here. Feature tables (gardens,
/// plants, tasks, ...) will be added as DbSets in later slices.
/// </summary>
public sealed class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Give Identity tables friendlier, snake-free names under a dedicated schema.
        builder.HasDefaultSchema("identity");
    }
}
