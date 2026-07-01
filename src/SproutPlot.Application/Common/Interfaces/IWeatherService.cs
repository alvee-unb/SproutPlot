using SproutPlot.Application.Features.Weather.Dtos;

namespace SproutPlot.Application.Common.Interfaces;

/// <summary>
/// Application-facing weather operations. Serves cached data when fresh and
/// otherwise fetches from the provider and caches the result.
/// </summary>
public interface IWeatherService
{
    Task<WeatherResponse> GetWeatherAsync(double latitude, double longitude, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GeocodeResult>> SearchLocationsAsync(string name, CancellationToken cancellationToken = default);
}
