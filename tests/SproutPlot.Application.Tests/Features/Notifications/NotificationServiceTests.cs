using SproutPlot.Application.Features.Notifications;
using SproutPlot.Application.Features.Notifications.Dtos;
using SproutPlot.Application.Features.Tasks;
using SproutPlot.Application.Features.Tasks.Dtos;
using SproutPlot.Application.Tests.Features.Gardens;
using SproutPlot.Application.Tests.Features.Plants;
using SproutPlot.Application.Tests.Features.Tasks;
using SproutPlot.Application.Tests.Features.Weather;
using SproutPlot.Domain.Entities;
using SproutPlot.Domain.Enums;
using Xunit;

namespace SproutPlot.Application.Tests.Features.Notifications;

public sealed class NotificationServiceTests
{
    private static readonly Guid Owner = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 7, 1, 8, 0, 0, TimeSpan.Zero);
    private static readonly DateOnly Today = new(2026, 7, 1);

    private readonly FakeGardenRepository _gardens = new();
    private readonly FakeGardenTaskRepository _taskRepo;
    private readonly FakeNotificationPreferencesRepository _prefs = new();
    private readonly FakeNotificationSender _sender = new();
    private readonly NotificationService _service;

    public NotificationServiceTests()
    {
        _taskRepo = new FakeGardenTaskRepository(_gardens);
        var taskService = new GardenTaskService(_taskRepo, _gardens, new FakePlantRepository(_gardens), new StubTimeProvider(Now));
        _service = new NotificationService(_prefs, taskService, _sender, new StubTimeProvider(Now));
    }

    private async Task<Guid> SeedGardenWithDueTaskAsync()
    {
        var garden = new Garden { Id = Guid.NewGuid(), OwnerId = Owner, Name = "G" };
        await _gardens.AddAsync(garden);
        await _taskRepo.AddAsync(new GardenTask
        {
            Id = Guid.NewGuid(),
            GardenId = garden.Id,
            Type = GardenTaskType.Water,
            DueOn = Today,
            Status = GardenTaskStatus.Pending,
        });
        return garden.Id;
    }

    [Fact]
    public async Task No_channels_enabled_sends_nothing()
    {
        await SeedGardenWithDueTaskAsync();

        var summary = await _service.SendRemindersAsync(Owner, "g@example.com");

        Assert.Empty(summary.ChannelsNotified);
        Assert.Empty(_sender.Sent);
    }

    [Fact]
    public async Task Email_enabled_with_due_task_sends_one_email()
    {
        await SeedGardenWithDueTaskAsync();
        await _service.UpdatePreferencesAsync(Owner, new UpdateNotificationPreferencesRequest
        {
            EmailRemindersEnabled = true,
            ReminderLeadDays = 1,
        });

        var summary = await _service.SendRemindersAsync(Owner, "g@example.com");

        Assert.Equal(1, summary.TaskCount);
        Assert.Contains(NotificationChannel.Email, summary.ChannelsNotified);
        Assert.Single(_sender.Sent);
        Assert.Equal("g@example.com", _sender.Sent[0].Recipient);
    }

    [Fact]
    public async Task Enabled_but_no_tasks_sends_nothing()
    {
        var garden = new Garden { Id = Guid.NewGuid(), OwnerId = Owner, Name = "Empty" };
        await _gardens.AddAsync(garden);
        await _service.UpdatePreferencesAsync(Owner, new UpdateNotificationPreferencesRequest { EmailRemindersEnabled = true });

        var summary = await _service.SendRemindersAsync(Owner, "g@example.com");

        Assert.Equal(0, summary.TaskCount);
        Assert.Empty(_sender.Sent);
    }

    [Fact]
    public async Task Update_clamps_lead_days_and_get_returns_it()
    {
        await _service.UpdatePreferencesAsync(Owner, new UpdateNotificationPreferencesRequest
        {
            EmailRemindersEnabled = true,
            ReminderLeadDays = 999,
        });

        var prefs = await _service.GetPreferencesAsync(Owner);

        Assert.Equal(30, prefs.ReminderLeadDays); // clamped to max
        Assert.True(prefs.EmailRemindersEnabled);
    }

    [Fact]
    public async Task Get_returns_defaults_when_unset()
    {
        var prefs = await _service.GetPreferencesAsync(Guid.NewGuid());

        Assert.False(prefs.EmailRemindersEnabled);
        Assert.False(prefs.PushRemindersEnabled);
        Assert.Equal(1, prefs.ReminderLeadDays);
    }
}
