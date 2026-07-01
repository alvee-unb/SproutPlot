using Microsoft.Extensions.Logging;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Features.Notifications.Dtos;

namespace SproutPlot.Infrastructure.Notifications;

/// <summary>
/// Default <see cref="INotificationSender"/> that logs the notification instead
/// of delivering it. This keeps the feature end-to-end usable while real email
/// and push providers are added later (they simply replace or augment this).
/// </summary>
public sealed class LoggingNotificationSender : INotificationSender
{
    private readonly ILogger<LoggingNotificationSender> _logger;

    public LoggingNotificationSender(ILogger<LoggingNotificationSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Notification [{Channel}] to {Recipient}: {Subject}\n{Body}",
            message.Channel,
            message.Recipient,
            message.Subject,
            message.Body);

        return Task.CompletedTask;
    }
}
