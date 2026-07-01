using SproutPlot.Domain.Common;

namespace SproutPlot.Domain.Entities;

/// <summary>
/// A physical garden owned by a user. Acts as the container for plants and
/// their tasks (added in later slices).
/// </summary>
public sealed class Garden : BaseEntity
{
    /// <summary>Id of the owning user (ASP.NET Identity user).</summary>
    public Guid OwnerId { get; set; }

    /// <summary>Display name, e.g. "Back yard beds".</summary>
    public required string Name { get; set; }

    /// <summary>Free-text location, e.g. a city or a spot within the property.</summary>
    public string? Location { get; set; }

    /// <summary>Free-text size, e.g. "10 x 4 m" or "3 raised beds".</summary>
    public string? Size { get; set; }

    /// <summary>Optional latitude, enabling weather-aware watering guidance.</summary>
    public double? Latitude { get; set; }

    /// <summary>Optional longitude, enabling weather-aware watering guidance.</summary>
    public double? Longitude { get; set; }

    /// <summary>Optional notes about the garden.</summary>
    public string? Notes { get; set; }
}
