namespace SproutPlot.Application.Features.Watering.Dtos;

/// <summary>Payload for logging a watering. The garden comes from the route.</summary>
public sealed record RecordWateringRequest
{
    /// <summary>Optional specific plant that was watered (must be in the garden).</summary>
    public Guid? PlantId { get; init; }

    /// <summary>When it was watered; defaults to now if omitted.</summary>
    public DateTime? WateredAtUtc { get; init; }

    public double? AmountLiters { get; init; }

    public string? Notes { get; init; }
}
