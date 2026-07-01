using SproutPlot.Domain.Enums;

namespace SproutPlot.Application.Features.Watering;

/// <summary>Baseline days between waterings by plant category (deterministic defaults).</summary>
public static class PlantWateringIntervals
{
    /// <summary>Interval used when a garden has no typed plants.</summary>
    public const int DefaultIntervalDays = 3;

    public static int BaseIntervalDays(PlantTypeCategory category) => category switch
    {
        PlantTypeCategory.Vegetable => 2,
        PlantTypeCategory.Herb => 3,
        PlantTypeCategory.Flower => 3,
        PlantTypeCategory.Fruit => 4,
        PlantTypeCategory.Shrub => 5,
        PlantTypeCategory.Tree => 7,
        _ => DefaultIntervalDays,
    };
}
