using Microsoft.EntityFrameworkCore;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Infrastructure.Persistence.Repositories;

/// <summary>EF Core implementation of <see cref="IWeatherCacheRepository"/>.</summary>
public sealed class WeatherCacheRepository : IWeatherCacheRepository
{
    private readonly AppDbContext _db;

    public WeatherCacheRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<string?> GetFreshPayloadAsync(
        double latitudeKey,
        double longitudeKey,
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        return await _db.WeatherCacheEntries
            .AsNoTracking()
            .Where(w => w.LatitudeKey == latitudeKey && w.LongitudeKey == longitudeKey && w.ExpiresAtUtc > nowUtc)
            .Select(w => w.PayloadJson)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpsertAsync(
        double latitudeKey,
        double longitudeKey,
        string payloadJson,
        DateTime fetchedAtUtc,
        DateTime expiresAtUtc,
        CancellationToken cancellationToken = default)
    {
        var existing = await _db.WeatherCacheEntries
            .FirstOrDefaultAsync(w => w.LatitudeKey == latitudeKey && w.LongitudeKey == longitudeKey, cancellationToken);

        if (existing is null)
        {
            _db.WeatherCacheEntries.Add(new WeatherCacheEntry
            {
                LatitudeKey = latitudeKey,
                LongitudeKey = longitudeKey,
                PayloadJson = payloadJson,
                FetchedAtUtc = fetchedAtUtc,
                ExpiresAtUtc = expiresAtUtc,
            });
        }
        else
        {
            existing.PayloadJson = payloadJson;
            existing.FetchedAtUtc = fetchedAtUtc;
            existing.ExpiresAtUtc = expiresAtUtc;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
