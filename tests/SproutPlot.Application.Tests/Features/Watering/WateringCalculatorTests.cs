using SproutPlot.Application.Features.Watering;
using SproutPlot.Domain.Enums;
using Xunit;

namespace SproutPlot.Application.Tests.Features.Watering;

public sealed class WateringCalculatorTests
{
    private static readonly DateOnly Today = new(2026, 6, 15);

    private static WateringCalculatorInput BaseInput(WateringCalculatorInput? overrides = null) => new()
    {
        BaseIntervalDays = 3,
        Season = Season.Spring,
        DaysSinceLastWatering = 0,
        MaxRainProbabilityNext2Days = null,
        RainSumNext2DaysMm = null,
        Today = Today,
        RainConsidered = false,
    };

    [Fact]
    public void Rain_forecast_defers_watering()
    {
        var input = BaseInput() with
        {
            DaysSinceLastWatering = 10, // well overdue
            MaxRainProbabilityNext2Days = 85,
            RainConsidered = true,
        };

        var result = WateringCalculator.Recommend(input);

        Assert.False(result.ShouldWaterNow);
        Assert.Contains("rain", result.Reason, System.StringComparison.OrdinalIgnoreCase);
        Assert.True(result.RainConsidered);
    }

    [Fact]
    public void Never_watered_recommends_watering()
    {
        var result = WateringCalculator.Recommend(BaseInput() with { DaysSinceLastWatering = null });

        Assert.True(result.ShouldWaterNow);
    }

    [Fact]
    public void Overdue_without_rain_recommends_watering()
    {
        var result = WateringCalculator.Recommend(BaseInput() with { DaysSinceLastWatering = 5, BaseIntervalDays = 3 });

        Assert.True(result.ShouldWaterNow);
    }

    [Fact]
    public void Recently_watered_defers_with_due_date()
    {
        var result = WateringCalculator.Recommend(BaseInput() with { DaysSinceLastWatering = 1, BaseIntervalDays = 5 });

        Assert.False(result.ShouldWaterNow);
        Assert.NotNull(result.NextDueDate);
        Assert.True(result.NextDueDate > Today);
    }

    [Fact]
    public void Summer_shortens_the_interval()
    {
        var spring = WateringCalculator.Recommend(BaseInput() with { Season = Season.Spring, BaseIntervalDays = 4 });
        var summer = WateringCalculator.Recommend(BaseInput() with { Season = Season.Summer, BaseIntervalDays = 4 });

        Assert.Equal(4, spring.EffectiveIntervalDays);
        Assert.True(summer.EffectiveIntervalDays < spring.EffectiveIntervalDays); // 4 * 0.7 ≈ 3
    }

    [Fact]
    public void Heavy_rain_by_amount_also_defers()
    {
        var input = BaseInput() with { DaysSinceLastWatering = 10, RainSumNext2DaysMm = 12, RainConsidered = true };

        Assert.False(WateringCalculator.Recommend(input).ShouldWaterNow);
    }
}
