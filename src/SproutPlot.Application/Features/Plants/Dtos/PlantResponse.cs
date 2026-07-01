using SproutPlot.Domain.Enums;

namespace SproutPlot.Application.Features.Plants.Dtos;

/// <summary>Plant as returned to clients.</summary>
public sealed record PlantResponse
{
    public required Guid Id { get; init; }

    public required Guid GardenId { get; init; }

    public Guid? PlantTypeId { get; init; }

    /// <summary>Denormalised name of the referenced plant type, for display.</summary>
    public string? PlantTypeName { get; init; }

    public required string Name { get; init; }

    public string? Variety { get; init; }

    public DateOnly? DatePlanted { get; init; }

    public required int Quantity { get; init; }

    public required PlantStatus Status { get; init; }

    public string? Notes { get; init; }

    public required DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; init; }
}
