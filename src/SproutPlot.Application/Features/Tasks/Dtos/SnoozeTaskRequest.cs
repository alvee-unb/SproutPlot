namespace SproutPlot.Application.Features.Tasks.Dtos;

/// <summary>Payload for snoozing a task by pushing its due date forward.</summary>
public sealed record SnoozeTaskRequest
{
    /// <summary>Number of days to push the due date (from today, or the due date if later).</summary>
    public int Days { get; init; } = 1;
}
