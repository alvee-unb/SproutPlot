using SproutPlot.Domain.Common;

namespace SproutPlot.Domain.Entities;

/// <summary>A recorded watering, against a garden and optionally a specific plant.</summary>
public sealed class WateringEvent : BaseEntity
{
    /// <summary>Garden that was watered.</summary>
    public Guid GardenId { get; set; }

    public Garden? Garden { get; set; }

    /// <summary>Optional specific plant that was watered.</summary>
    public Guid? PlantId { get; set; }

    public Plant? Plant { get; set; }

    /// <summary>When the watering happened (UTC).</summary>
    public DateTime WateredAtUtc { get; set; }

    /// <summary>Optional amount of water applied, in litres.</summary>
    public double? AmountLiters { get; set; }

    public string? Notes { get; set; }
}
