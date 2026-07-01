using SproutPlot.Domain.Enums;

namespace SproutPlot.Application.Features.Plants.Dtos;

/// <summary>Payload for adding a plant to a garden. The garden comes from the route.</summary>
public sealed record CreatePlantRequest
{
    public required string Name { get; init; }

    public Guid? PlantTypeId { get; init; }

    public string? Variety { get; init; }

    public DateOnly? DatePlanted { get; init; }

    public int Quantity { get; init; } = 1;

    public PlantStatus Status { get; init; } = PlantStatus.Growing;

    public string? Notes { get; init; }
}
