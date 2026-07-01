namespace SproutPlot.Application.Features.Weather.Dtos;

/// <summary>A place matched by a location name search.</summary>
public sealed record GeocodeResult
{
    public required string Name { get; init; }

    public string? Country { get; init; }

    public string? Admin1 { get; init; }

    public required double Latitude { get; init; }

    public required double Longitude { get; init; }
}
