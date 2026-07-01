using SproutPlot.Application.Features.Watering.Dtos;
using SproutPlot.Domain.Enums;

namespace SproutPlot.Application.Features.Watering;

/// <summary>Inputs for the deterministic watering recommendation.</summary>
public sealed record WateringCalculatorInput
{
    /// <summary>Base days between waterings for the thirstiest plant in the garden.</summary>
    public required int BaseIntervalDays { get; init; }

    public required Season Season { get; init; }

    /// <summary>Whole days since the last recorded watering, or null if never watered.</summary>
    public int? DaysSinceLastWatering { get; init; }

    /// <summary>Highest daily rain probability (%) over the next two days, if weather was available.</summary>
    public double? MaxRainProbabilityNext2Days { get; init; }

    /// <summary>Total forecast rainfall (mm) over the next two days, if weather was available.</summary>
    public double? RainSumNext2DaysMm { get; init; }

    public required DateOnly Today { get; init; }

    /// <summary>True when weather data was available and factored in.</summary>
    public required bool RainConsidered { get; init; }
}

/// <summary>
/// Deterministic watering guidance. Given a base interval (from plant type), the
/// season, the watering history and — when available — the rain forecast, it
/// decides whether to water now. This is pure business logic: no AI, no I/O.
/// </summary>
public static class WateringCalculator
{
    // Significant rain either likely (probability) or substantial (amount).
    private const double RainProbabilityThreshold = 60;
    private const double RainSumThresholdMm = 5;

    public static WateringRecommendationResponse Recommend(WateringCalculatorInput input)
    {
        var interval = SeasonAdjustedInterval(input.BaseIntervalDays, input.Season);

        var rainLikely =
            (input.MaxRainProbabilityNext2Days is { } p && p >= RainProbabilityThreshold) ||
            (input.RainSumNext2DaysMm is { } mm && mm >= RainSumThresholdMm);

        if (rainLikely)
        {
            return new WateringRecommendationResponse
            {
                ShouldWaterNow = false,
                Reason = "Significant rain is forecast in the next two days — hold off watering and let nature do it.",
                EffectiveIntervalDays = interval,
                DaysSinceLastWatering = input.DaysSinceLastWatering,
                NextDueDate = input.Today.AddDays(2),
                Season = input.Season,
                RainConsidered = input.RainConsidered,
            };
        }

        if (input.DaysSinceLastWatering is null)
        {
            return new WateringRecommendationResponse
            {
                ShouldWaterNow = true,
                Reason = "No watering has been recorded yet — give the garden a drink and log it.",
                EffectiveIntervalDays = interval,
                DaysSinceLastWatering = null,
                NextDueDate = input.Today,
                Season = input.Season,
                RainConsidered = input.RainConsidered,
            };
        }

        var days = input.DaysSinceLastWatering.Value;
        if (days >= interval)
        {
            return new WateringRecommendationResponse
            {
                ShouldWaterNow = true,
                Reason = $"Last watered {days} day(s) ago; in {input.Season.ToString().ToLowerInvariant()} this garden wants water about every {interval} days.",
                EffectiveIntervalDays = interval,
                DaysSinceLastWatering = days,
                NextDueDate = input.Today,
                Season = input.Season,
                RainConsidered = input.RainConsidered,
            };
        }

        var dueIn = interval - days;
        return new WateringRecommendationResponse
        {
            ShouldWaterNow = false,
            Reason = $"Watered {days} day(s) ago; next watering due in about {dueIn} day(s).",
            EffectiveIntervalDays = interval,
            DaysSinceLastWatering = days,
            NextDueDate = input.Today.AddDays(dueIn),
            Season = input.Season,
            RainConsidered = input.RainConsidered,
        };
    }

    private static int SeasonAdjustedInterval(int baseInterval, Season season)
    {
        var factor = season switch
        {
            Season.Summer => 0.7,
            Season.Autumn => 1.2,
            Season.Winter => 1.6,
            _ => 1.0, // Spring
        };

        return Math.Max(1, (int)Math.Round(baseInterval * factor));
    }
}
