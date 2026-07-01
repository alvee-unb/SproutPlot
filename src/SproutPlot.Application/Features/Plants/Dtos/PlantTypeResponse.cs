using SproutPlot.Domain.Enums;

namespace SproutPlot.Application.Features.Plants.Dtos;

/// <summary>A seeded plant type, used to populate selection lists.</summary>
public sealed record PlantTypeResponse
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required PlantTypeCategory Category { get; init; }
}
