namespace SproutPlot.Application.Features.Notifications.Dtos;

/// <summary>Payload to update a user's notification settings.</summary>
public sealed record UpdateNotificationPreferencesRequest
{
    public bool EmailRemindersEnabled { get; init; }

    public bool PushRemindersEnabled { get; init; }

    public int ReminderLeadDays { get; init; } = 1;
}
