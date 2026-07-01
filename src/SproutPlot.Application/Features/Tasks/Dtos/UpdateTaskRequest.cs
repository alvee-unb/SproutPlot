using SproutPlot.Domain.Enums;

namespace SproutPlot.Application.Features.Tasks.Dtos;

/// <summary>Payload for editing a task's details (not its completion state).</summary>
public sealed record UpdateTaskRequest
{
    public required GardenTaskType Type { get; init; }

    public string? Title { get; init; }

    public required DateOnly DueOn { get; init; }

    public string? Notes { get; init; }
}
