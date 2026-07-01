using SproutPlot.Domain.Enums;

namespace SproutPlot.Application.Features.Tasks.Dtos;

/// <summary>A garden task as returned to clients.</summary>
public sealed record TaskResponse
{
    public required Guid Id { get; init; }

    public required Guid GardenId { get; init; }

    public string? GardenName { get; init; }

    public Guid? PlantId { get; init; }

    public string? PlantName { get; init; }

    public required GardenTaskType Type { get; init; }

    public string? Title { get; init; }

    public required DateOnly DueOn { get; init; }

    public required GardenTaskStatus Status { get; init; }

    public DateTime? CompletedAtUtc { get; init; }

    public string? Notes { get; init; }

    public required DateTime CreatedAtUtc { get; init; }
}
