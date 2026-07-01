namespace SproutPlot.Application.Features.Gardens.Dtos;

/// <summary>Payload for creating a garden.</summary>
public sealed record CreateGardenRequest
{
    public required string Name { get; init; }

    public string? Location { get; init; }

    public string? Size { get; init; }

    public string? Notes { get; init; }
}
