using SproutPlot.Domain.Entities;

namespace SproutPlot.Application.Common.Interfaces;

/// <summary>Persistence for per-user <see cref="NotificationPreferences"/>.</summary>
public interface INotificationPreferencesRepository
{
    Task<NotificationPreferences?> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task UpsertAsync(NotificationPreferences preferences, CancellationToken cancellationToken = default);
}
