namespace SproutPlot.Application.Features.Gardens.Dtos;

/// <summary>Garden as returned to clients.</summary>
public sealed record GardenResponse
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public string? Location { get; init; }

    public string? Size { get; init; }

    public string? Notes { get; init; }

    public required DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; init; }
}
