using SproutPlot.Application.Features.Notifications.Dtos;

namespace SproutPlot.Application.Common.Interfaces;

/// <summary>Notification preferences and reminder dispatch.</summary>
public interface INotificationService
{
    Task<NotificationPreferencesResponse> GetPreferencesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<NotificationPreferencesResponse> UpdatePreferencesAsync(
        Guid userId,
        UpdateNotificationPreferencesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Builds the user's task reminder digest and dispatches it over the enabled
    /// channels. Intended to be called on demand now and by a scheduled job later.
    /// </summary>
    Task<ReminderSummaryResponse> SendRemindersAsync(
        Guid userId,
        string email,
        CancellationToken cancellationToken = default);
}
