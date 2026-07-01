using SproutPlot.Domain.Enums;

namespace SproutPlot.Application.Features.Plants.Dtos;

/// <summary>Payload for updating an existing plant (does not move it between gardens).</summary>
public sealed record UpdatePlantRequest
{
    public required string Name { get; init; }

    public Guid? PlantTypeId { get; init; }

    public string? Variety { get; init; }

    public DateOnly? DatePlanted { get; init; }

    public int Quantity { get; init; } = 1;

    public PlantStatus Status { get; init; } = PlantStatus.Growing;

    public string? Notes { get; init; }
}
