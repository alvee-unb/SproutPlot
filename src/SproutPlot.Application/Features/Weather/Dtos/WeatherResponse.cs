namespace SproutPlot.Application.Features.Weather.Dtos;

/// <summary>Current conditions plus a short daily forecast for a coordinate.</summary>
public sealed record WeatherResponse
{
    public required double Latitude { get; init; }

    public required double Longitude { get; init; }

    public string? TimeZone { get; init; }

    public required CurrentWeather Current { get; init; }

    public required IReadOnlyList<DailyForecast> Daily { get; init; }

    public required DateTime FetchedAtUtc { get; init; }
}

/// <summary>Present-moment conditions.</summary>
public sealed record CurrentWeather
{
    public required double TemperatureC { get; init; }

    public double? ApparentTemperatureC { get; init; }

    public required int Humidity { get; init; }

    public required double WindSpeedKmh { get; init; }

    public required double PrecipitationMm { get; init; }

    public required int WeatherCode { get; init; }

    public required string Description { get; init; }

    public required bool IsDay { get; init; }
}

/// <summary>A single day's forecast.</summary>
public sealed record DailyForecast
{
    public required DateOnly Date { get; init; }

    public required double TempMinC { get; init; }

    public required double TempMaxC { get; init; }

    public required double PrecipitationSumMm { get; init; }

    public int? PrecipitationProbabilityMaxPercent { get; init; }

    public required int WeatherCode { get; init; }

    public required string Description { get; init; }
}
