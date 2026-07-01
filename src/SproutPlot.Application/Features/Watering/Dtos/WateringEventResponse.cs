namespace SproutPlot.Application.Features.Watering.Dtos;

/// <summary>A recorded watering as returned to clients.</summary>
public sealed record WateringEventResponse
{
    public required Guid Id { get; init; }

    public required Guid GardenId { get; init; }

    public Guid? PlantId { get; init; }

    /// <summary>Name of the watered plant, when the event targets a specific plant.</summary>
    public string? PlantName { get; init; }

    public required DateTime WateredAtUtc { get; init; }

    public double? AmountLiters { get; init; }

    public string? Notes { get; init; }
}
