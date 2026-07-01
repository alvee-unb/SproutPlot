using System.Text;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Features.Notifications.Dtos;
using SproutPlot.Application.Features.Tasks;
using SproutPlot.Domain.Entities;
using SproutPlot.Domain.Enums;

namespace SproutPlot.Application.Features.Notifications;

/// <summary>
/// Manages notification preferences and builds/dispatches the task reminder
/// digest. Dispatch goes through <see cref="INotificationSender"/> so the
/// email/push providers can be added later without changing this logic.
/// </summary>
public sealed class NotificationService : INotificationService
{
    private const int MaxLeadDays = 30;

    private readonly INotificationPreferencesRepository _preferences;
    private readonly IGardenTaskService _tasks;
    private readonly INotificationSender _sender;
    private readonly TimeProvider _timeProvider;

    public NotificationService(
        INotificationPreferencesRepository preferences,
        IGardenTaskService tasks,
        INotificationSender sender,
        TimeProvider timeProvider)
    {
        _preferences = preferences;
        _tasks = tasks;
        _sender = sender;
        _timeProvider = timeProvider;
    }

    public async Task<NotificationPreferencesResponse> GetPreferencesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var prefs = await _preferences.GetByUserAsync(userId, cancellationToken);
        return ToResponse(prefs ?? Defaults(userId));
    }

    public async Task<NotificationPreferencesResponse> UpdatePreferencesAsync(
        Guid userId,
        UpdateNotificationPreferencesRequest request,
        CancellationToken cancellationToken = default)
    {
        var prefs = new NotificationPreferences
        {
            UserId = userId,
            EmailRemindersEnabled = request.EmailRemindersEnabled,
            PushRemindersEnabled = request.PushRemindersEnabled,
            ReminderLeadDays = Math.Clamp(request.ReminderLeadDays, 0, MaxLeadDays),
        };

        await _preferences.UpsertAsync(prefs, cancellationToken);
        return ToResponse(prefs);
    }

    public async Task<ReminderSummaryResponse> SendRemindersAsync(
        Guid userId,
        string email,
        CancellationToken cancellationToken = default)
    {
        var prefs = await _preferences.GetByUserAsync(userId, cancellationToken) ?? Defaults(userId);

        if (!prefs.EmailRemindersEnabled && !prefs.PushRemindersEnabled)
        {
            return new ReminderSummaryResponse
            {
                TaskCount = 0,
                ChannelsNotified = Array.Empty<NotificationChannel>(),
                Message = "No reminder channels are enabled.",
            };
        }

        var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        var tasks = await _tasks.GetTasksAsync(
            userId,
            new TaskQueryParameters
            {
                Status = GardenTaskStatus.Pending,
                DueOnOrBefore = today.AddDays(prefs.ReminderLeadDays),
                PageSize = 200,
            },
            cancellationToken);

        if (tasks.Items.Count == 0)
        {
            return new ReminderSummaryResponse
            {
                TaskCount = 0,
                ChannelsNotified = Array.Empty<NotificationChannel>(),
                Message = "No pending tasks in the reminder window.",
            };
        }

        var subject = $"SproutPlot: {tasks.Items.Count} garden task(s) need attention";
        var body = BuildBody(tasks.Items);

        var channels = new List<NotificationChannel>();
        if (prefs.EmailRemindersEnabled)
        {
            await _sender.SendAsync(
                new NotificationMessage { Channel = NotificationChannel.Email, Recipient = email, Subject = subject, Body = body },
                cancellationToken);
            channels.Add(NotificationChannel.Email);
        }

        if (prefs.PushRemindersEnabled)
        {
            await _sender.SendAsync(
                new NotificationMessage { Channel = NotificationChannel.Push, Recipient = userId.ToString(), Subject = subject, Body = body },
                cancellationToken);
            channels.Add(NotificationChannel.Push);
        }

        return new ReminderSummaryResponse
        {
            TaskCount = tasks.Items.Count,
            ChannelsNotified = channels,
            Message = $"Reminder for {tasks.Items.Count} task(s) sent via {string.Join(", ", channels)}.",
        };
    }

    private static string BuildBody(IReadOnlyList<Features.Tasks.Dtos.TaskResponse> tasks)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"You have {tasks.Count} task(s) due soon:");
        foreach (var task in tasks)
        {
            var label = string.IsNullOrWhiteSpace(task.Title) ? task.Type.ToString() : task.Title;
            var where = task.PlantName is null ? task.GardenName : $"{task.GardenName} / {task.PlantName}";
            sb.AppendLine($"- {label} ({task.Type}) in {where} — due {task.DueOn:yyyy-MM-dd}");
        }

        return sb.ToString();
    }

    private static NotificationPreferences Defaults(Guid userId) => new()
    {
        UserId = userId,
        EmailRemindersEnabled = false,
        PushRemindersEnabled = false,
        ReminderLeadDays = 1,
    };

    private static NotificationPreferencesResponse ToResponse(NotificationPreferences prefs) => new()
    {
        EmailRemindersEnabled = prefs.EmailRemindersEnabled,
        PushRemindersEnabled = prefs.PushRemindersEnabled,
        ReminderLeadDays = prefs.ReminderLeadDays,
    };
}
