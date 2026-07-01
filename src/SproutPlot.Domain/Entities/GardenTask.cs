using SproutPlot.Domain.Common;
using SproutPlot.Domain.Enums;

namespace SproutPlot.Domain.Entities;

/// <summary>A maintenance task for a garden, optionally targeting a specific plant.</summary>
public sealed class GardenTask : BaseEntity
{
    public Guid GardenId { get; set; }

    public Garden? Garden { get; set; }

    public Guid? PlantId { get; set; }

    public Plant? Plant { get; set; }

    public GardenTaskType Type { get; set; }

    /// <summary>Optional custom label; when null the UI shows the task type.</summary>
    public string? Title { get; set; }

    /// <summary>Date the task is due.</summary>
    public DateOnly DueOn { get; set; }

    public GardenTaskStatus Status { get; set; } = GardenTaskStatus.Pending;

    public DateTime? CompletedAtUtc { get; set; }

    public string? Notes { get; set; }
}
