using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Features.Watering.Dtos;

namespace SproutPlot.Api.Controllers;

/// <summary>Recording waterings, watering history, and the watering recommendation.</summary>
[Authorize]
[Produces("application/json")]
public sealed class WateringController : ApiControllerBase
{
    private readonly IWateringService _wateringService;
    private readonly IValidator<RecordWateringRequest> _recordValidator;

    public WateringController(IWateringService wateringService, IValidator<RecordWateringRequest> recordValidator)
    {
        _wateringService = wateringService;
        _recordValidator = recordValidator;
    }

    /// <summary>Logs a watering against a garden (optionally a specific plant).</summary>
    [HttpPost("api/gardens/{gardenId:guid}/waterings")]
    [ProducesResponseType(typeof(WateringEventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Record(Guid gardenId, RecordWateringRequest request, CancellationToken cancellationToken)
    {
        var validation = await _recordValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(validation);
        }

        var result = await _wateringService.RecordWateringAsync(gardenId, CurrentUserId, request, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : FromError(result);
    }

    /// <summary>Watering history for a garden, newest first.</summary>
    [HttpGet("api/gardens/{gardenId:guid}/waterings")]
    [ProducesResponseType(typeof(PagedResult<WateringEventResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHistory(
        Guid gardenId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _wateringService.GetHistoryAsync(gardenId, CurrentUserId, page, pageSize, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : FromError(result);
    }

    /// <summary>Deterministic recommendation on whether to water the garden now.</summary>
    [HttpGet("api/gardens/{gardenId:guid}/watering-recommendation")]
    [ProducesResponseType(typeof(WateringRecommendationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRecommendation(Guid gardenId, CancellationToken cancellationToken)
    {
        var result = await _wateringService.GetRecommendationAsync(gardenId, CurrentUserId, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : FromError(result);
    }

    /// <summary>Deletes a watering record.</summary>
    [HttpDelete("api/waterings/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _wateringService.DeleteWateringAsync(id, CurrentUserId, cancellationToken);
        return result.Succeeded ? NoContent() : FromError(result);
    }
}
