namespace SproutPlot.Application.Features.Notifications.Dtos;

/// <summary>Current notification settings for a user.</summary>
public sealed record NotificationPreferencesResponse
{
    public required bool EmailRemindersEnabled { get; init; }

    public required bool PushRemindersEnabled { get; init; }

    public required int ReminderLeadDays { get; init; }
}
