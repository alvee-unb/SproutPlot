using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Features.Gardens;
using SproutPlot.Application.Features.Gardens.Dtos;

namespace SproutPlot.Api.Controllers;

/// <summary>CRUD endpoints for the authenticated user's gardens.</summary>
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class GardensController : ApiControllerBase
{
    private readonly IGardenService _gardenService;
    private readonly IValidator<CreateGardenRequest> _createValidator;
    private readonly IValidator<UpdateGardenRequest> _updateValidator;

    public GardensController(
        IGardenService gardenService,
        IValidator<CreateGardenRequest> createValidator,
        IValidator<UpdateGardenRequest> updateValidator)
    {
        _gardenService = gardenService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>Lists the current user's gardens (paged, filterable, sortable).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GardenResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = GardenQueryParameters.DefaultPageSize,
        [FromQuery] string? search = null,
        [FromQuery] GardenSortBy sortBy = GardenSortBy.Name,
        [FromQuery] bool descending = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GardenQueryParameters
        {
            Page = page,
            PageSize = pageSize,
            Search = search,
            SortBy = sortBy,
            Descending = descending,
        };

        var result = await _gardenService.GetGardensAsync(CurrentUserId, query, cancellationToken);
        return Ok(result);
    }

    /// <summary>Gets a single garden by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GardenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await _gardenService.GetGardenAsync(id, CurrentUserId, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : FromError(result);
    }

    /// <summary>Creates a new garden.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(GardenResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateGardenRequest request, CancellationToken cancellationToken)
    {
        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(validation);
        }

        var result = await _gardenService.CreateGardenAsync(CurrentUserId, request, cancellationToken);
        if (!result.Succeeded)
        {
            return FromError(result);
        }

        return CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>Updates an existing garden.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(GardenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateGardenRequest request, CancellationToken cancellationToken)
    {
        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(validation);
        }

        var result = await _gardenService.UpdateGardenAsync(id, CurrentUserId, request, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : FromError(result);
    }

    /// <summary>Deletes a garden.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _gardenService.DeleteGardenAsync(id, CurrentUserId, cancellationToken);
        return result.Succeeded ? NoContent() : FromError(result);
    }
}
