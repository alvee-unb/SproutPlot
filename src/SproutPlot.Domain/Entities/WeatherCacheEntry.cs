namespace SproutPlot.Domain.Entities;

/// <summary>
/// A cached weather forecast for a rounded coordinate, used to avoid repeated
/// calls to the external weather provider and to keep the dashboard fast.
/// </summary>
public sealed class WeatherCacheEntry
{
    public Guid Id { get; set; }

    /// <summary>Latitude rounded to the cache grid (used as part of the lookup key).</summary>
    public double LatitudeKey { get; set; }

    /// <summary>Longitude rounded to the cache grid (used as part of the lookup key).</summary>
    public double LongitudeKey { get; set; }

    /// <summary>Serialized weather response payload.</summary>
    public required string PayloadJson { get; set; }

    public DateTime FetchedAtUtc { get; set; }

    /// <summary>When this cache entry should be considered stale.</summary>
    public DateTime ExpiresAtUtc { get; set; }
}
