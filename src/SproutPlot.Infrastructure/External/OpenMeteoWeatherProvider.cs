using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Features.Weather;
using SproutPlot.Application.Features.Weather.Dtos;

namespace SproutPlot.Infrastructure.External;

/// <summary>
/// <see cref="IWeatherProvider"/> backed by the free Open-Meteo API (no key required).
/// </summary>
public sealed class OpenMeteoWeatherProvider : IWeatherProvider
{
    private const string ForecastBaseUrl = "https://api.open-meteo.com/v1/forecast";
    private const string GeocodeBaseUrl = "https://geocoding-api.open-meteo.com/v1/search";

    private readonly HttpClient _httpClient;

    public OpenMeteoWeatherProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherResponse> FetchForecastAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        var lat = latitude.ToString(CultureInfo.InvariantCulture);
        var lon = longitude.ToString(CultureInfo.InvariantCulture);

        var url = $"{ForecastBaseUrl}?latitude={lat}&longitude={lon}" +
                  "&current=temperature_2m,relative_humidity_2m,apparent_temperature,is_day,precipitation,weather_code,wind_speed_10m" +
                  "&daily=weather_code,temperature_2m_max,temperature_2m_min,precipitation_sum,precipitation_probability_max" +
                  "&timezone=auto&forecast_days=7";

        var forecast = await _httpClient.GetFromJsonOrThrowAsync<OmForecast>(url, cancellationToken);

        var current = forecast.Current
            ?? throw new InvalidOperationException("Weather provider returned no current conditions.");
        var daily = forecast.Daily ?? new OmDaily();

        var days = new List<DailyForecast>();
        for (var i = 0; i < daily.Time.Count; i++)
        {
            days.Add(new DailyForecast
            {
                Date = DateOnly.ParseExact(daily.Time[i], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                WeatherCode = ValueAt(daily.WeatherCode, i),
                TempMaxC = ValueAt(daily.TempMax, i),
                TempMinC = ValueAt(daily.TempMin, i),
                PrecipitationSumMm = ValueAt(daily.PrecipSum, i),
                PrecipitationProbabilityMaxPercent = ValueAt(daily.PrecipProb, i),
                Description = WeatherCodeInfo.Describe(ValueAt(daily.WeatherCode, i)),
            });
        }

        return new WeatherResponse
        {
            Latitude = forecast.Latitude,
            Longitude = forecast.Longitude,
            TimeZone = forecast.Timezone,
            Current = new CurrentWeather
            {
                TemperatureC = current.Temperature,
                ApparentTemperatureC = current.ApparentTemperature,
                Humidity = current.Humidity,
                WindSpeedKmh = current.WindSpeed,
                PrecipitationMm = current.Precipitation,
                WeatherCode = current.WeatherCode,
                Description = WeatherCodeInfo.Describe(current.WeatherCode),
                IsDay = current.IsDay == 1,
            },
            Daily = days,
            FetchedAtUtc = DateTime.UtcNow,
        };
    }

    public async Task<IReadOnlyList<GeocodeResult>> SearchLocationsAsync(string name, CancellationToken cancellationToken = default)
    {
        var url = $"{GeocodeBaseUrl}?name={Uri.EscapeDataString(name)}&count=5&language=en&format=json";

        var response = await _httpClient.GetFromJsonOrThrowAsync<OmGeocode>(url, cancellationToken);

        return (response.Results ?? new List<OmGeoResult>())
            .Select(r => new GeocodeResult
            {
                Name = r.Name,
                Country = r.Country,
                Admin1 = r.Admin1,
                Latitude = r.Latitude,
                Longitude = r.Longitude,
            })
            .ToList();
    }

    private static T ValueAt<T>(IReadOnlyList<T> list, int index) => index < list.Count ? list[index] : default!;

    // --- Open-Meteo response shapes ---

    private sealed class OmForecast
    {
        [JsonPropertyName("latitude")] public double Latitude { get; set; }
        [JsonPropertyName("longitude")] public double Longitude { get; set; }
        [JsonPropertyName("timezone")] public string? Timezone { get; set; }
        [JsonPropertyName("current")] public OmCurrent? Current { get; set; }
        [JsonPropertyName("daily")] public OmDaily? Daily { get; set; }
    }

    private sealed class OmCurrent
    {
        [JsonPropertyName("temperature_2m")] public double Temperature { get; set; }
        [JsonPropertyName("relative_humidity_2m")] public int Humidity { get; set; }
        [JsonPropertyName("apparent_temperature")] public double? ApparentTemperature { get; set; }
        [JsonPropertyName("is_day")] public int IsDay { get; set; }
        [JsonPropertyName("precipitation")] public double Precipitation { get; set; }
        [JsonPropertyName("weather_code")] public int WeatherCode { get; set; }
        [JsonPropertyName("wind_speed_10m")] public double WindSpeed { get; set; }
    }

    private sealed class OmDaily
    {
        [JsonPropertyName("time")] public List<string> Time { get; set; } = new();
        [JsonPropertyName("weather_code")] public List<int> WeatherCode { get; set; } = new();
        [JsonPropertyName("temperature_2m_max")] public List<double> TempMax { get; set; } = new();
        [JsonPropertyName("temperature_2m_min")] public List<double> TempMin { get; set; } = new();
        [JsonPropertyName("precipitation_sum")] public List<double> PrecipSum { get; set; } = new();
        [JsonPropertyName("precipitation_probability_max")] public List<int?> PrecipProb { get; set; } = new();
    }

    private sealed class OmGeocode
    {
        [JsonPropertyName("results")] public List<OmGeoResult>? Results { get; set; }
    }

    private sealed class OmGeoResult
    {
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("country")] public string? Country { get; set; }
        [JsonPropertyName("admin1")] public string? Admin1 { get; set; }
        [JsonPropertyName("latitude")] public double Latitude { get; set; }
        [JsonPropertyName("longitude")] public double Longitude { get; set; }
    }
}

/// <summary>Small helper to deserialize JSON and fail clearly on error responses.</summary>
internal static class HttpClientJsonExtensions
{
    public static async Task<T> GetFromJsonOrThrowAsync<T>(this HttpClient client, string url, CancellationToken cancellationToken)
    {
        using var response = await client.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var value = await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);

        return value ?? throw new InvalidOperationException($"Weather provider returned an empty response for {url}.");
    }
}
