using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Common.Results;
using SproutPlot.Application.Features.Calendar.Dtos;

namespace SproutPlot.Application.Features.Calendar;

/// <summary>Resolves the date range, validates it, and returns ordered calendar events.</summary>
public sealed class CalendarService : ICalendarService
{
    private readonly ICalendarRepository _repository;
    private readonly TimeProvider _timeProvider;

    public CalendarService(ICalendarRepository repository, TimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task<Result<IReadOnlyList<CalendarEvent>>> GetEventsAsync(
        Guid ownerId,
        DateOnly? from,
        DateOnly? to,
        Guid? gardenId,
        CancellationToken cancellationToken = default)
    {
        // Default to the current month when either bound is missing.
        DateOnly resolvedFrom;
        DateOnly resolvedTo;
        if (from is { } f && to is { } t)
        {
            (resolvedFrom, resolvedTo) = (f, t);
        }
        else
        {
            var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
            (resolvedFrom, resolvedTo) = CalendarRange.ForMonth(today);
        }

        if (!CalendarRange.IsValid(resolvedFrom, resolvedTo))
        {
            return Result<IReadOnlyList<CalendarEvent>>.Failure(
                $"Invalid date range: 'from' must be on or before 'to' and span at most {CalendarRange.MaxSpanDays} days.");
        }

        var events = await _repository.GetEventsAsync(ownerId, resolvedFrom, resolvedTo, gardenId, cancellationToken);

        var ordered = events
            .OrderBy(e => e.Date)
            .ThenBy(e => e.Kind)
            .ToList();

        return Result<IReadOnlyList<CalendarEvent>>.Success(ordered);
    }
}
