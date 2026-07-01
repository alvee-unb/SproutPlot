using SproutPlot.Domain.Enums;

namespace SproutPlot.Application.Features.Notifications.Dtos;

/// <summary>Outcome of a reminder run: what would be (or was) sent.</summary>
public sealed record ReminderSummaryResponse
{
    /// <summary>Number of pending tasks included in the reminder window.</summary>
    public required int TaskCount { get; init; }

    /// <summary>Channels a reminder was dispatched to (empty if nothing to send).</summary>
    public required IReadOnlyList<NotificationChannel> ChannelsNotified { get; init; }

    /// <summary>Human-readable explanation of the outcome.</summary>
    public required string Message { get; init; }
}
