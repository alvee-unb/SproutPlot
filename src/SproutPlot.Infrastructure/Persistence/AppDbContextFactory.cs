using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SproutPlot.Infrastructure.Persistence;

/// <summary>
/// Design-time factory used by the EF Core CLI ("dotnet ef") to create the
/// context for migrations without starting the API host.
///
/// The connection string is read from the SPROUTPLOT_CONNECTION environment
/// variable; if unset it falls back to the local development database. This is
/// a local dev convenience only — the running application always resolves its
/// connection string from configuration / user-secrets, never from here.
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string LocalDevConnection =
        "Host=localhost;Port=5432;Database=sproutplot_dev;Username=sproutplot;Password=sproutplot_dev";

    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("SPROUTPLOT_CONNECTION") ?? LocalDevConnection;

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
            .Options;

        return new AppDbContext(options);
    }
}
