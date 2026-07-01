using SproutPlot.Application.Common.Results;
using SproutPlot.Application.Features.Calendar.Dtos;

namespace SproutPlot.Application.Common.Interfaces;

/// <summary>Read access to the aggregated garden calendar.</summary>
public interface ICalendarService
{
    /// <summary>
    /// Calendar events in the given range (defaults to the current month) across
    /// the user's gardens, optionally scoped to one garden.
    /// </summary>
    Task<Result<IReadOnlyList<CalendarEvent>>> GetEventsAsync(
        Guid ownerId,
        DateOnly? from,
        DateOnly? to,
        Guid? gardenId,
        CancellationToken cancellationToken = default);
}
