using System.Text.Json;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Features.Weather;
using SproutPlot.Application.Features.Weather.Dtos;
using Xunit;

namespace SproutPlot.Application.Tests.Features.Weather;

public sealed class WeatherServiceTests
{
    private static WeatherResponse SampleResponse(double temp) => new()
    {
        Latitude = 23.81,
        Longitude = 90.41,
        Current = new CurrentWeather
        {
            TemperatureC = temp,
            Humidity = 70,
            WindSpeedKmh = 5,
            PrecipitationMm = 0,
            WeatherCode = 0,
            Description = "Clear sky",
            IsDay = true,
        },
        Daily = Array.Empty<DailyForecast>(),
        FetchedAtUtc = new DateTime(2026, 6, 30, 12, 0, 0, DateTimeKind.Utc),
    };

    [Fact]
    public async Task Cache_miss_fetches_from_provider_and_caches()
    {
        var provider = new FakeWeatherProvider { Response = SampleResponse(25) };
        var cache = new FakeWeatherCacheRepository();
        var service = new WeatherService(provider, cache, new StubTimeProvider(DateTimeOffset.UtcNow));

        var result = await service.GetWeatherAsync(23.81, 90.41);

        Assert.Equal(25, result.Current.TemperatureC);
        Assert.Equal(1, provider.FetchCount);
        Assert.Equal(1, cache.UpsertCount);
    }

    [Fact]
    public async Task Cache_hit_returns_cached_and_skips_provider()
    {
        var cache = new FakeWeatherCacheRepository { Payload = JsonSerializer.Serialize(SampleResponse(18)) };
        var provider = new FakeWeatherProvider { Response = SampleResponse(99) };
        var service = new WeatherService(provider, cache, new StubTimeProvider(DateTimeOffset.UtcNow));

        var result = await service.GetWeatherAsync(23.81, 90.41);

        Assert.Equal(18, result.Current.TemperatureC); // from cache, not the provider's 99
        Assert.Equal(0, provider.FetchCount);
        Assert.Equal(0, cache.UpsertCount);
    }
}

internal sealed class StubTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _now;
    public StubTimeProvider(DateTimeOffset now) => _now = now;
    public override DateTimeOffset GetUtcNow() => _now;
}

internal sealed class FakeWeatherProvider : IWeatherProvider
{
    public int FetchCount;
    public WeatherResponse Response { get; set; } = null!;

    public Task<WeatherResponse> FetchForecastAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        FetchCount++;
        return Task.FromResult(Response);
    }

    public Task<IReadOnlyList<GeocodeResult>> SearchLocationsAsync(string name, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<GeocodeResult>>(Array.Empty<GeocodeResult>());
}

internal sealed class FakeWeatherCacheRepository : IWeatherCacheRepository
{
    public string? Payload { get; set; }
    public int UpsertCount;

    public Task<string?> GetFreshPayloadAsync(double latitudeKey, double longitudeKey, DateTime nowUtc, CancellationToken cancellationToken = default) =>
        Task.FromResult(Payload);

    public Task UpsertAsync(double latitudeKey, double longitudeKey, string payloadJson, DateTime fetchedAtUtc, DateTime expiresAtUtc, CancellationToken cancellationToken = default)
    {
        UpsertCount++;
        Payload = payloadJson;
        return Task.CompletedTask;
    }
}
