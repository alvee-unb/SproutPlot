namespace SproutPlot.Application.Features.Weather;

/// <summary>Maps WMO weather interpretation codes (used by Open-Meteo) to text.</summary>
public static class WeatherCodeInfo
{
    /// <summary>Returns a human-readable description for a WMO weather code.</summary>
    public static string Describe(int code) => code switch
    {
        0 => "Clear sky",
        1 => "Mainly clear",
        2 => "Partly cloudy",
        3 => "Overcast",
        45 or 48 => "Fog",
        51 => "Light drizzle",
        53 => "Moderate drizzle",
        55 => "Dense drizzle",
        56 or 57 => "Freezing drizzle",
        61 => "Slight rain",
        63 => "Moderate rain",
        65 => "Heavy rain",
        66 or 67 => "Freezing rain",
        71 => "Slight snow",
        73 => "Moderate snow",
        75 => "Heavy snow",
        77 => "Snow grains",
        80 => "Slight rain showers",
        81 => "Moderate rain showers",
        82 => "Violent rain showers",
        85 or 86 => "Snow showers",
        95 => "Thunderstorm",
        96 or 99 => "Thunderstorm with hail",
        _ => "Unknown",
    };
}
