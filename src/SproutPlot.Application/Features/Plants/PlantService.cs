using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Common.Results;
using SproutPlot.Application.Features.Plants.Dtos;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Application.Features.Plants;

/// <summary>
/// Orchestrates plant use cases. Ownership is enforced through the parent
/// garden: a plant is only accessible if its garden belongs to the caller.
/// </summary>
public sealed class PlantService : IPlantService
{
    private readonly IPlantRepository _plants;
    private readonly IGardenRepository _gardens;
    private readonly IPlantTypeRepository _plantTypes;

    public PlantService(
        IPlantRepository plants,
        IGardenRepository gardens,
        IPlantTypeRepository plantTypes)
    {
        _plants = plants;
        _gardens = gardens;
        _plantTypes = plantTypes;
    }

    public async Task<Result<PagedResult<PlantResponse>>> GetPlantsAsync(
        Guid gardenId,
        Guid ownerId,
        PlantQueryParameters query,
        CancellationToken cancellationToken = default)
    {
        var garden = await _gardens.GetByIdForOwnerAsync(gardenId, ownerId, cancellationToken);
        if (garden is null)
        {
            return Result<PagedResult<PlantResponse>>.NotFound("Garden not found.");
        }

        var page = await _plants.GetPagedByGardenAsync(gardenId, query, cancellationToken);

        return Result<PagedResult<PlantResponse>>.Success(new PagedResult<PlantResponse>
        {
            Items = page.Items.Select(MapToResponse).ToList(),
            Page = page.Page,
            PageSize = page.PageSize,
            TotalCount = page.TotalCount,
        });
    }

    public async Task<Result<PlantResponse>> GetPlantAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var plant = await _plants.GetByIdForOwnerAsync(id, ownerId, cancellationToken);
        return plant is null
            ? Result<PlantResponse>.NotFound("Plant not found.")
            : Result<PlantResponse>.Success(MapToResponse(plant));
    }

    public async Task<Result<PlantResponse>> CreatePlantAsync(
        Guid gardenId,
        Guid ownerId,
        CreatePlantRequest request,
        CancellationToken cancellationToken = default)
    {
        var garden = await _gardens.GetByIdForOwnerAsync(gardenId, ownerId, cancellationToken);
        if (garden is null)
        {
            return Result<PlantResponse>.NotFound("Garden not found.");
        }

        if (request.PlantTypeId is { } typeId && !await _plantTypes.ExistsAsync(typeId, cancellationToken))
        {
            return Result<PlantResponse>.Failure("The specified plant type does not exist.");
        }

        var plant = new Plant
        {
            GardenId = gardenId,
            PlantTypeId = request.PlantTypeId,
            Name = request.Name.Trim(),
            Variety = request.Variety?.Trim(),
            DatePlanted = request.DatePlanted,
            Quantity = request.Quantity,
            Status = request.Status,
            Notes = request.Notes?.Trim(),
        };

        await _plants.AddAsync(plant, cancellationToken);

        // Reload with PlantType included so the response carries the type name.
        var created = await _plants.GetByIdForOwnerAsync(plant.Id, ownerId, cancellationToken);
        return Result<PlantResponse>.Success(MapToResponse(created ?? plant));
    }

    public async Task<Result<PlantResponse>> UpdatePlantAsync(
        Guid id,
        Guid ownerId,
        UpdatePlantRequest request,
        CancellationToken cancellationToken = default)
    {
        var plant = await _plants.GetByIdForOwnerAsync(id, ownerId, cancellationToken);
        if (plant is null)
        {
            return Result<PlantResponse>.NotFound("Plant not found.");
        }

        if (request.PlantTypeId is { } typeId && !await _plantTypes.ExistsAsync(typeId, cancellationToken))
        {
            return Result<PlantResponse>.Failure("The specified plant type does not exist.");
        }

        plant.PlantTypeId = request.PlantTypeId;
        plant.Name = request.Name.Trim();
        plant.Variety = request.Variety?.Trim();
        plant.DatePlanted = request.DatePlanted;
        plant.Quantity = request.Quantity;
        plant.Status = request.Status;
        plant.Notes = request.Notes?.Trim();

        await _plants.UpdateAsync(plant, cancellationToken);

        var updated = await _plants.GetByIdForOwnerAsync(plant.Id, ownerId, cancellationToken);
        return Result<PlantResponse>.Success(MapToResponse(updated ?? plant));
    }

    public async Task<Result> DeletePlantAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var plant = await _plants.GetByIdForOwnerAsync(id, ownerId, cancellationToken);
        if (plant is null)
        {
            return Result.NotFound("Plant not found.");
        }

        await _plants.DeleteAsync(plant, cancellationToken);
        return Result.Success();
    }

    private static PlantResponse MapToResponse(Plant plant) => new()
    {
        Id = plant.Id,
        GardenId = plant.GardenId,
        PlantTypeId = plant.PlantTypeId,
        PlantTypeName = plant.PlantType?.Name,
        Name = plant.Name,
        Variety = plant.Variety,
        DatePlanted = plant.DatePlanted,
        Quantity = plant.Quantity,
        Status = plant.Status,
        Notes = plant.Notes,
        CreatedAtUtc = plant.CreatedAtUtc,
        UpdatedAtUtc = plant.UpdatedAtUtc,
    };
}
