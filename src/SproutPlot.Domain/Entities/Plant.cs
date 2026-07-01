using SproutPlot.Domain.Common;
using SproutPlot.Domain.Enums;

namespace SproutPlot.Domain.Entities;

/// <summary>A specific planting within a garden.</summary>
public sealed class Plant : BaseEntity
{
    /// <summary>Owning garden.</summary>
    public Guid GardenId { get; set; }

    public Garden? Garden { get; set; }

    /// <summary>Optional reference to a shared plant type.</summary>
    public Guid? PlantTypeId { get; set; }

    public PlantType? PlantType { get; set; }

    /// <summary>Display name, e.g. "Cherry tomatoes by the fence".</summary>
    public required string Name { get; set; }

    /// <summary>Optional cultivar/variety, e.g. "Sungold".</summary>
    public string? Variety { get; set; }

    /// <summary>Date the plant was put in the ground, if known.</summary>
    public DateOnly? DatePlanted { get; set; }

    /// <summary>How many were planted.</summary>
    public int Quantity { get; set; } = 1;

    public PlantStatus Status { get; set; } = PlantStatus.Growing;

    public string? Notes { get; set; }
}
