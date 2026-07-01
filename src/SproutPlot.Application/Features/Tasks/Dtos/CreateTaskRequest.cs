using SproutPlot.Domain.Enums;

namespace SproutPlot.Application.Features.Tasks.Dtos;

/// <summary>Payload for creating a task. The garden comes from the route.</summary>
public sealed record CreateTaskRequest
{
    public required GardenTaskType Type { get; init; }

    /// <summary>Optional custom label; defaults to the task type when omitted.</summary>
    public string? Title { get; init; }

    /// <summary>Optional specific plant the task targets (must be in the garden).</summary>
    public Guid? PlantId { get; init; }

    public required DateOnly DueOn { get; init; }

    public string? Notes { get; init; }
}
