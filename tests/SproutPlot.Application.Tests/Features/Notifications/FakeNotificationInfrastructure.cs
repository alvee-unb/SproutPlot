using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Features.Notifications.Dtos;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Application.Tests.Features.Notifications;

internal sealed class FakeNotificationPreferencesRepository : INotificationPreferencesRepository
{
    private readonly Dictionary<Guid, NotificationPreferences> _store = new();

    public Task<NotificationPreferences?> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_store.TryGetValue(userId, out var p) ? p : null);

    public Task UpsertAsync(NotificationPreferences preferences, CancellationToken cancellationToken = default)
    {
        _store[preferences.UserId] = preferences;
        return Task.CompletedTask;
    }
}

internal sealed class FakeNotificationSender : INotificationSender
{
    public List<NotificationMessage> Sent { get; } = new();

    public Task SendAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        Sent.Add(message);
        return Task.CompletedTask;
    }
}
