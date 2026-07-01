using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Features.Weather.Dtos;

namespace SproutPlot.Api.Controllers;

/// <summary>Weather lookups for the dashboard and (later) watering reminders.</summary>
[Authorize]
[Route("api/weather")]
[Produces("application/json")]
public sealed class WeatherController : ApiControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    /// <summary>Current conditions and a 7-day forecast for a coordinate.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(WeatherResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        CancellationToken cancellationToken)
    {
        if (latitude is < -90 or > 90 || longitude is < -180 or > 180)
        {
            return Problem(detail: "Latitude must be -90..90 and longitude -180..180.", statusCode: StatusCodes.Status400BadRequest);
        }

        var weather = await _weatherService.GetWeatherAsync(latitude, longitude, cancellationToken);
        return Ok(weather);
    }

    /// <summary>Searches for places by name, returning their coordinates.</summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IReadOnlyList<GeocodeResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search([FromQuery] string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Problem(detail: "A search name is required.", statusCode: StatusCodes.Status400BadRequest);
        }

        var results = await _weatherService.SearchLocationsAsync(name.Trim(), cancellationToken);
        return Ok(results);
    }
}
