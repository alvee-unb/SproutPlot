using SproutPlot.Application.Features.Weather.Dtos;

namespace SproutPlot.Application.Common.Interfaces;

/// <summary>Talks to the external weather API. Implemented in Infrastructure.</summary>
public interface IWeatherProvider
{
    Task<WeatherResponse> FetchForecastAsync(double latitude, double longitude, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GeocodeResult>> SearchLocationsAsync(string name, CancellationToken cancellationToken = default);
}
