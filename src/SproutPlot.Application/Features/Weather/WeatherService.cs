using System.Text.Json;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Features.Weather.Dtos;

namespace SproutPlot.Application.Features.Weather;

/// <summary>
/// Caching layer over <see cref="IWeatherProvider"/>. Coordinates are rounded to
/// a coarse grid so nearby lookups share a cache entry; entries live for a short
/// TTL to balance freshness against provider fair-use.
/// </summary>
public sealed class WeatherService : IWeatherService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(30);
    private const int CoordinateKeyDecimals = 2;

    private readonly IWeatherProvider _provider;
    private readonly IWeatherCacheRepository _cache;
    private readonly TimeProvider _timeProvider;

    public WeatherService(IWeatherProvider provider, IWeatherCacheRepository cache, TimeProvider timeProvider)
    {
        _provider = provider;
        _cache = cache;
        _timeProvider = timeProvider;
    }

    public async Task<WeatherResponse> GetWeatherAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
        var latKey = Math.Round(latitude, CoordinateKeyDecimals);
        var lonKey = Math.Round(longitude, CoordinateKeyDecimals);

        var cached = await _cache.GetFreshPayloadAsync(latKey, lonKey, nowUtc, cancellationToken);
        if (cached is not null)
        {
            var fromCache = JsonSerializer.Deserialize<WeatherResponse>(cached);
            if (fromCache is not null)
            {
                return fromCache;
            }
        }

        var fresh = await _provider.FetchForecastAsync(latitude, longitude, cancellationToken);

        var payload = JsonSerializer.Serialize(fresh);
        await _cache.UpsertAsync(latKey, lonKey, payload, nowUtc, nowUtc.Add(CacheTtl), cancellationToken);

        return fresh;
    }

    public Task<IReadOnlyList<GeocodeResult>> SearchLocationsAsync(string name, CancellationToken cancellationToken = default) =>
        _provider.SearchLocationsAsync(name, cancellationToken);
}
