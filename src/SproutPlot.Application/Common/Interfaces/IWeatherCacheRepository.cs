namespace SproutPlot.Application.Common.Interfaces;

/// <summary>Stores and retrieves cached weather payloads keyed by rounded coordinate.</summary>
public interface IWeatherCacheRepository
{
    /// <summary>Returns the cached payload for the key if it exists and has not expired.</summary>
    Task<string?> GetFreshPayloadAsync(
        double latitudeKey,
        double longitudeKey,
        DateTime nowUtc,
        CancellationToken cancellationToken = default);

    /// <summary>Inserts or replaces the cached payload for the key.</summary>
    Task UpsertAsync(
        double latitudeKey,
        double longitudeKey,
        string payloadJson,
        DateTime fetchedAtUtc,
        DateTime expiresAtUtc,
        CancellationToken cancellationToken = default);
}
