using SproutPlot.Application.Common.Results;
using SproutPlot.Application.Features.Tasks;
using SproutPlot.Application.Features.Tasks.Dtos;
using SproutPlot.Application.Tests.Features.Gardens;
using SproutPlot.Application.Tests.Features.Plants;
using SproutPlot.Application.Tests.Features.Weather;
using SproutPlot.Domain.Entities;
using SproutPlot.Domain.Enums;
using Xunit;

namespace SproutPlot.Application.Tests.Features.Tasks;

public sealed class GardenTaskServiceTests
{
    private static readonly Guid OwnerA = Guid.NewGuid();
    private static readonly Guid OwnerB = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 6, 15, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateOnly Today = new(2026, 6, 15);

    private readonly FakeGardenRepository _gardens = new();
    private readonly FakeGardenTaskRepository _tasks;
    private readonly FakePlantRepository _plants;
    private readonly GardenTaskService _service;

    public GardenTaskServiceTests()
    {
        _tasks = new FakeGardenTaskRepository(_gardens);
        _plants = new FakePlantRepository(_gardens);
        _service = new GardenTaskService(_tasks, _gardens, _plants, new StubTimeProvider(Now));
    }

    private async Task<Guid> SeedGardenAsync(Guid ownerId)
    {
        var garden = new Garden { Id = Guid.NewGuid(), OwnerId = ownerId, Name = "G" };
        await _gardens.AddAsync(garden);
        return garden.Id;
    }

    private CreateTaskRequest NewTask(DateOnly? due = null) => new()
    {
        Type = GardenTaskType.Fertilize,
        DueOn = due ?? Today,
    };

    [Fact]
    public async Task Create_in_owned_garden_is_pending()
    {
        var gardenId = await SeedGardenAsync(OwnerA);

        var result = await _service.CreateTaskAsync(gardenId, OwnerA, NewTask());

        Assert.True(result.Succeeded);
        Assert.Equal(GardenTaskStatus.Pending, result.Value!.Status);
        Assert.Single(_tasks.All);
    }

    [Fact]
    public async Task Create_in_unowned_garden_returns_not_found()
    {
        var gardenId = await SeedGardenAsync(OwnerA);

        var result = await _service.CreateTaskAsync(gardenId, OwnerB, NewTask());

        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
        Assert.Empty(_tasks.All);
    }

    [Fact]
    public async Task Complete_sets_status_and_timestamp()
    {
        var gardenId = await SeedGardenAsync(OwnerA);
        var created = await _service.CreateTaskAsync(gardenId, OwnerA, NewTask());

        var result = await _service.CompleteTaskAsync(created.Value!.Id, OwnerA);

        Assert.True(result.Succeeded);
        Assert.Equal(GardenTaskStatus.Completed, result.Value!.Status);
        Assert.Equal(Now.UtcDateTime, result.Value.CompletedAtUtc);
    }

    [Fact]
    public async Task Snooze_pushes_overdue_task_from_today()
    {
        var gardenId = await SeedGardenAsync(OwnerA);
        var created = await _service.CreateTaskAsync(gardenId, OwnerA, NewTask(Today.AddDays(-2))); // overdue

        var result = await _service.SnoozeTaskAsync(created.Value!.Id, OwnerA, days: 3);

        Assert.True(result.Succeeded);
        Assert.Equal(Today.AddDays(3), result.Value!.DueOn); // from today, not the old due date
        Assert.Equal(GardenTaskStatus.Pending, result.Value.Status);
    }

    [Fact]
    public async Task Get_task_of_other_user_returns_not_found()
    {
        var gardenId = await SeedGardenAsync(OwnerA);
        var created = await _service.CreateTaskAsync(gardenId, OwnerA, NewTask());

        var result = await _service.GetTaskAsync(created.Value!.Id, OwnerB);

        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Upcoming_filter_returns_only_due_or_before()
    {
        var gardenId = await SeedGardenAsync(OwnerA);
        await _service.CreateTaskAsync(gardenId, OwnerA, NewTask(Today));
        await _service.CreateTaskAsync(gardenId, OwnerA, NewTask(Today.AddDays(30)));

        var page = await _service.GetTasksAsync(OwnerA, new TaskQueryParameters { DueOnOrBefore = Today.AddDays(7) });

        Assert.Equal(1, page.TotalCount);
    }
}
