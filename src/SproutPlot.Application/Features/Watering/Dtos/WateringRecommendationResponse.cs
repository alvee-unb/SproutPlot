using SproutPlot.Domain.Enums;

namespace SproutPlot.Application.Features.Watering.Dtos;

/// <summary>Deterministic guidance on whether a garden needs watering now.</summary>
public sealed record WateringRecommendationResponse
{
    public required bool ShouldWaterNow { get; init; }

    public required string Reason { get; init; }

    /// <summary>Season-adjusted days between waterings used for the decision.</summary>
    public required int EffectiveIntervalDays { get; init; }

    public int? DaysSinceLastWatering { get; init; }

    public DateOnly? NextDueDate { get; init; }

    public required Season Season { get; init; }

    /// <summary>True when the rain forecast was available and factored in.</summary>
    public required bool RainConsidered { get; init; }
}
