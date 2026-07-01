namespace SproutPlot.Domain.Entities;

/// <summary>Per-user notification settings (one row per user).</summary>
public sealed class NotificationPreferences
{
    /// <summary>Owning user id (primary key and FK to the Identity user).</summary>
    public Guid UserId { get; set; }

    public bool EmailRemindersEnabled { get; set; }

    public bool PushRemindersEnabled { get; set; }

    /// <summary>How many days ahead to include upcoming tasks in reminders.</summary>
    public int ReminderLeadDays { get; set; } = 1;
}
