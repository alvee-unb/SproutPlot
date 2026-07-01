using SproutPlot.Domain.Common;
using SproutPlot.Domain.Enums;

namespace SproutPlot.Domain.Entities;

/// <summary>
/// Shared reference data describing a kind of plant (e.g. "Tomato", "Basil").
/// Not owned by any user; seeded and read-only from the application's point of view.
/// </summary>
public sealed class PlantType : BaseEntity
{
    public required string Name { get; set; }

    public PlantTypeCategory Category { get; set; }
}
