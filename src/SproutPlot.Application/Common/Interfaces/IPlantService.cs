using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Common.Results;
using SproutPlot.Application.Features.Plants;
using SproutPlot.Application.Features.Plants.Dtos;

namespace SproutPlot.Application.Common.Interfaces;

/// <summary>Use cases for managing plants within a user's gardens.</summary>
public interface IPlantService
{
    /// <summary>Lists plants in a garden the caller owns. NotFound if the garden isn't theirs.</summary>
    Task<Result<PagedResult<PlantResponse>>> GetPlantsAsync(
        Guid gardenId,
        Guid ownerId,
        PlantQueryParameters query,
        CancellationToken cancellationToken = default);

    Task<Result<PlantResponse>> GetPlantAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default);

    Task<Result<PlantResponse>> CreatePlantAsync(
        Guid gardenId,
        Guid ownerId,
        CreatePlantRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<PlantResponse>> UpdatePlantAsync(
        Guid id,
        Guid ownerId,
        UpdatePlantRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DeletePlantAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default);
}
