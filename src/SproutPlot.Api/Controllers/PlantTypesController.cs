using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Features.Plants.Dtos;

namespace SproutPlot.Api.Controllers;

/// <summary>Read-only access to seeded plant type reference data.</summary>
[Authorize]
[Route("api/plant-types")]
[Produces("application/json")]
public sealed class PlantTypesController : ApiControllerBase
{
    private readonly IPlantTypeService _plantTypeService;

    public PlantTypesController(IPlantTypeService plantTypeService)
    {
        _plantTypeService = plantTypeService;
    }

    /// <summary>Lists plant types, optionally filtered by a name search.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PlantTypeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null, CancellationToken cancellationToken = default)
    {
        var types = await _plantTypeService.GetPlantTypesAsync(search, cancellationToken);
        return Ok(types);
    }
}
