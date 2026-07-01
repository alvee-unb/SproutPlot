using SproutPlot.Domain.Enums;

namespace SproutPlot.Application.Features.Notifications.Dtos;

/// <summary>A single notification to deliver over one channel.</summary>
public sealed record NotificationMessage
{
    public required NotificationChannel Channel { get; init; }

    /// <summary>Email address, device token, or other channel-specific address.</summary>
    public required string Recipient { get; init; }

    public required string Subject { get; init; }

    public required string Body { get; init; }
}
