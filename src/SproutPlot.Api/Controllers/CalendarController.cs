using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Features.Calendar.Dtos;

namespace SproutPlot.Api.Controllers;

/// <summary>Aggregated calendar of plantings, task due dates and waterings.</summary>
[Authorize]
[Route("api/calendar")]
[Produces("application/json")]
public sealed class CalendarController : ApiControllerBase
{
    private readonly ICalendarService _calendarService;

    public CalendarController(ICalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    /// <summary>
    /// Calendar events in the given range (defaults to the current month),
    /// optionally scoped to a single garden.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CalendarEvent>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(
        [FromQuery] DateOnly? from = null,
        [FromQuery] DateOnly? to = null,
        [FromQuery] Guid? gardenId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _calendarService.GetEventsAsync(CurrentUserId, from, to, gardenId, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : FromError(result);
    }
}
