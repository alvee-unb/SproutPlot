using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Common.Results;
using SproutPlot.Application.Features.Gardens.Dtos;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Application.Features.Gardens;

/// <summary>
/// Orchestrates garden use cases. All reads and writes are scoped to the
/// supplied owner id so a user can only ever see or change their own gardens.
/// </summary>
public sealed class GardenService : IGardenService
{
    private readonly IGardenRepository _repository;

    public GardenService(IGardenRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<GardenResponse>> GetGardensAsync(
        Guid ownerId,
        GardenQueryParameters query,
        CancellationToken cancellationToken = default)
    {
        var page = await _repository.GetPagedByOwnerAsync(ownerId, query, cancellationToken);

        return new PagedResult<GardenResponse>
        {
            Items = page.Items.Select(MapToResponse).ToList(),
            Page = page.Page,
            PageSize = page.PageSize,
            TotalCount = page.TotalCount,
        };
    }

    public async Task<Result<GardenResponse>> GetGardenAsync(
        Guid id,
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        var garden = await _repository.GetByIdForOwnerAsync(id, ownerId, cancellationToken);
        return garden is null
            ? Result<GardenResponse>.NotFound("Garden not found.")
            : Result<GardenResponse>.Success(MapToResponse(garden));
    }

    public async Task<Result<GardenResponse>> CreateGardenAsync(
        Guid ownerId,
        CreateGardenRequest request,
        CancellationToken cancellationToken = default)
    {
        var garden = new Garden
        {
            OwnerId = ownerId,
            Name = request.Name.Trim(),
            Location = request.Location?.Trim(),
            Size = request.Size?.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Notes = request.Notes?.Trim(),
        };

        await _repository.AddAsync(garden, cancellationToken);
        return Result<GardenResponse>.Success(MapToResponse(garden));
    }

    public async Task<Result<GardenResponse>> UpdateGardenAsync(
        Guid id,
        Guid ownerId,
        UpdateGardenRequest request,
        CancellationToken cancellationToken = default)
    {
        var garden = await _repository.GetByIdForOwnerAsync(id, ownerId, cancellationToken);
        if (garden is null)
        {
            return Result<GardenResponse>.NotFound("Garden not found.");
        }

        garden.Name = request.Name.Trim();
        garden.Location = request.Location?.Trim();
        garden.Size = request.Size?.Trim();
        garden.Latitude = request.Latitude;
        garden.Longitude = request.Longitude;
        garden.Notes = request.Notes?.Trim();

        await _repository.UpdateAsync(garden, cancellationToken);
        return Result<GardenResponse>.Success(MapToResponse(garden));
    }

    public async Task<Result> DeleteGardenAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var garden = await _repository.GetByIdForOwnerAsync(id, ownerId, cancellationToken);
        if (garden is null)
        {
            return Result.NotFound("Garden not found.");
        }

        await _repository.DeleteAsync(garden, cancellationToken);
        return Result.Success();
    }

    private static GardenResponse MapToResponse(Garden garden) => new()
    {
        Id = garden.Id,
        Name = garden.Name,
        Location = garden.Location,
        Size = garden.Size,
        Latitude = garden.Latitude,
        Longitude = garden.Longitude,
        Notes = garden.Notes,
        CreatedAtUtc = garden.CreatedAtUtc,
        UpdatedAtUtc = garden.UpdatedAtUtc,
    };
}
