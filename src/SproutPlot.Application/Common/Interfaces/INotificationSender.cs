using SproutPlot.Application.Features.Notifications.Dtos;

namespace SproutPlot.Application.Common.Interfaces;

/// <summary>
/// Seam for delivering notifications over a channel. The current implementation
/// logs messages; real providers (e.g. an email service such as SendGrid, or
/// push via Azure Notification Hubs) plug in here later without touching callers.
/// </summary>
public interface INotificationSender
{
    Task SendAsync(NotificationMessage message, CancellationToken cancellationToken = default);
}
