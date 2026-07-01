using SproutPlot.Application.Features.Calendar.Dtos;

namespace SproutPlot.Application.Common.Interfaces;

/// <summary>Aggregates dated entries across a user's gardens for the calendar.</summary>
public interface ICalendarRepository
{
    Task<IReadOnlyList<CalendarEvent>> GetEventsAsync(
        Guid ownerId,
        DateOnly from,
        DateOnly to,
        Guid? gardenId,
        CancellationToken cancellationToken = default);
}
