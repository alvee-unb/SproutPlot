using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Features.Plants;
using SproutPlot.Application.Features.Plants.Dtos;
using SproutPlot.Domain.Enums;

namespace SproutPlot.Api.Controllers;

/// <summary>
/// Plant endpoints. Listing and creation are nested under a garden; fetching a
/// single plant and updating/deleting use the flat /api/plants/{id} route.
/// </summary>
[Authorize]
[Produces("application/json")]
public sealed class PlantsController : ApiControllerBase
{
    private readonly IPlantService _plantService;
    private readonly IValidator<CreatePlantRequest> _createValidator;
    private readonly IValidator<UpdatePlantRequest> _updateValidator;

    public PlantsController(
        IPlantService plantService,
        IValidator<CreatePlantRequest> createValidator,
        IValidator<UpdatePlantRequest> updateValidator)
    {
        _plantService = plantService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>Lists plants in a garden the caller owns.</summary>
    [HttpGet("api/gardens/{gardenId:guid}/plants")]
    [ProducesResponseType(typeof(PagedResult<PlantResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByGarden(
        Guid gardenId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PlantQueryParameters.DefaultPageSize,
        [FromQuery] string? search = null,
        [FromQuery] PlantStatus? status = null,
        [FromQuery] PlantSortBy sortBy = PlantSortBy.Name,
        [FromQuery] bool descending = false,
        CancellationToken cancellationToken = default)
    {
        var query = new PlantQueryParameters
        {
            Page = page,
            PageSize = pageSize,
            Search = search,
            Status = status,
            SortBy = sortBy,
            Descending = descending,
        };

        var result = await _plantService.GetPlantsAsync(gardenId, CurrentUserId, query, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : FromError(result);
    }

    /// <summary>Adds a plant to a garden the caller owns.</summary>
    [HttpPost("api/gardens/{gardenId:guid}/plants")]
    [ProducesResponseType(typeof(PlantResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(Guid gardenId, CreatePlantRequest request, CancellationToken cancellationToken)
    {
        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(validation);
        }

        var result = await _plantService.CreatePlantAsync(gardenId, CurrentUserId, request, cancellationToken);
        if (!result.Succeeded)
        {
            return FromError(result);
        }

        return CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>Gets a single plant.</summary>
    [HttpGet("api/plants/{id:guid}")]
    [ProducesResponseType(typeof(PlantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await _plantService.GetPlantAsync(id, CurrentUserId, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : FromError(result);
    }

    /// <summary>Updates a plant.</summary>
    [HttpPut("api/plants/{id:guid}")]
    [ProducesResponseType(typeof(PlantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdatePlantRequest request, CancellationToken cancellationToken)
    {
        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(validation);
        }

        var result = await _plantService.UpdatePlantAsync(id, CurrentUserId, request, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : FromError(result);
    }

    /// <summary>Deletes a plant.</summary>
    [HttpDelete("api/plants/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _plantService.DeletePlantAsync(id, CurrentUserId, cancellationToken);
        return result.Succeeded ? NoContent() : FromError(result);
    }
}
