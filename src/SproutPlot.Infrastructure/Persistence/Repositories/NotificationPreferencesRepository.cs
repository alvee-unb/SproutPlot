using Microsoft.EntityFrameworkCore;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Infrastructure.Persistence.Repositories;

/// <summary>EF Core implementation of <see cref="INotificationPreferencesRepository"/>.</summary>
public sealed class NotificationPreferencesRepository : INotificationPreferencesRepository
{
    private readonly AppDbContext _db;

    public NotificationPreferencesRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<NotificationPreferences?> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
        _db.NotificationPreferences.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

    public async Task UpsertAsync(NotificationPreferences preferences, CancellationToken cancellationToken = default)
    {
        var existing = await _db.NotificationPreferences
            .FirstOrDefaultAsync(p => p.UserId == preferences.UserId, cancellationToken);

        if (existing is null)
        {
            _db.NotificationPreferences.Add(preferences);
        }
        else
        {
            existing.EmailRemindersEnabled = preferences.EmailRemindersEnabled;
            existing.PushRemindersEnabled = preferences.PushRemindersEnabled;
            existing.ReminderLeadDays = preferences.ReminderLeadDays;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
