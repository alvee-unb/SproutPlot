namespace SproutPlot.Domain.Enums;

/// <summary>Lifecycle status of a plant in a garden.</summary>
public enum PlantStatus
{
    /// <summary>Intended but not yet in the ground.</summary>
    Planned = 0,

    /// <summary>Currently growing.</summary>
    Growing = 1,

    /// <summary>Harvested (may still be producing or finished).</summary>
    Harvested = 2,

    /// <summary>Removed or no longer present.</summary>
    Removed = 3,

    /// <summary>Died / failed.</summary>
    Dead = 4,
}
