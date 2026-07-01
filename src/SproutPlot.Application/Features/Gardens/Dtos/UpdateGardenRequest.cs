namespace SproutPlot.Application.Features.Gardens.Dtos;

/// <summary>Payload for updating an existing garden.</summary>
public sealed record UpdateGardenRequest
{
    public required string Name { get; init; }

    public string? Location { get; init; }

    public string? Size { get; init; }

    public double? Latitude { get; init; }

    public double? Longitude { get; init; }

    public string? Notes { get; init; }
}
