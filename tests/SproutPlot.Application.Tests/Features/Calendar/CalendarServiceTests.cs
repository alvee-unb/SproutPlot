using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Common.Results;
using SproutPlot.Application.Features.Calendar;
using SproutPlot.Application.Features.Calendar.Dtos;
using SproutPlot.Application.Tests.Features.Weather;
using Xunit;

namespace SproutPlot.Application.Tests.Features.Calendar;

public sealed class CalendarServiceTests
{
    private static readonly Guid Owner = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 7, 15, 12, 0, 0, TimeSpan.Zero);

    private readonly FakeCalendarRepository _repository = new();
    private readonly CalendarService _service;

    public CalendarServiceTests()
    {
        _service = new CalendarService(_repository, new StubTimeProvider(Now));
    }

    [Fact]
    public async Task Defaults_to_current_month_when_range_omitted()
    {
        await _service.GetEventsAsync(Owner, from: null, to: null, gardenId: null);

        Assert.Equal(new DateOnly(2026, 7, 1), _repository.LastFrom);
        Assert.Equal(new DateOnly(2026, 7, 31), _repository.LastTo);
    }

    [Fact]
    public async Task Reversed_range_is_a_validation_failure()
    {
        var result = await _service.GetEventsAsync(
            Owner,
            from: new DateOnly(2026, 7, 31),
            to: new DateOnly(2026, 7, 1),
            gardenId: null);

        Assert.False(result.Succeeded);
        Assert.Equal(ResultErrorType.Validation, result.ErrorType);
    }

    [Fact]
    public async Task Events_are_ordered_by_date_then_kind()
    {
        _repository.Events = new List<CalendarEvent>
        {
            Event(new DateOnly(2026, 7, 10), CalendarEventKind.Watering),
            Event(new DateOnly(2026, 7, 5), CalendarEventKind.Task),
            Event(new DateOnly(2026, 7, 5), CalendarEventKind.Planting),
        };

        var result = await _service.GetEventsAsync(Owner, new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 31), null);

        Assert.True(result.Succeeded);
        var ordered = result.Value!;
        Assert.Equal(new DateOnly(2026, 7, 5), ordered[0].Date);
        Assert.Equal(CalendarEventKind.Planting, ordered[0].Kind); // Planting sorts before Task on same day
        Assert.Equal(CalendarEventKind.Task, ordered[1].Kind);
        Assert.Equal(new DateOnly(2026, 7, 10), ordered[2].Date);
    }

    private static CalendarEvent Event(DateOnly date, CalendarEventKind kind) => new()
    {
        Date = date,
        Kind = kind,
        Title = kind.ToString(),
        GardenId = Guid.NewGuid(),
        SourceId = Guid.NewGuid(),
    };
}

internal sealed class FakeCalendarRepository : ICalendarRepository
{
    public DateOnly LastFrom { get; private set; }
    public DateOnly LastTo { get; private set; }
    public List<CalendarEvent> Events { get; set; } = new();

    public Task<IReadOnlyList<CalendarEvent>> GetEventsAsync(
        Guid ownerId,
        DateOnly from,
        DateOnly to,
        Guid? gardenId,
        CancellationToken cancellationToken = default)
    {
        LastFrom = from;
        LastTo = to;
        return Task.FromResult<IReadOnlyList<CalendarEvent>>(Events);
    }
}
