using Microsoft.EntityFrameworkCore;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Features.Plants;
using SproutPlot.Domain.Entities;
using SproutPlot.Domain.Enums;

namespace SproutPlot.Infrastructure.Persistence.Repositories;

/// <summary>EF Core implementation of <see cref="IPlantRepository"/>.</summary>
public sealed class PlantRepository : IPlantRepository
{
    private readonly AppDbContext _db;

    public PlantRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<Plant>> GetPagedByGardenAsync(
        Guid gardenId,
        PlantQueryParameters query,
        CancellationToken cancellationToken = default)
    {
        var plants = _db.Plants
            .AsNoTracking()
            .Include(p => p.PlantType)
            .Where(p => p.GardenId == gardenId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            plants = plants.Where(p => EF.Functions.ILike(p.Name, pattern));
        }

        if (query.Status is { } status)
        {
            plants = plants.Where(p => p.Status == status);
        }

        plants = (query.SortBy, query.Descending) switch
        {
            (PlantSortBy.DatePlanted, false) => plants.OrderBy(p => p.DatePlanted),
            (PlantSortBy.DatePlanted, true) => plants.OrderByDescending(p => p.DatePlanted),
            (PlantSortBy.CreatedAt, false) => plants.OrderBy(p => p.CreatedAtUtc),
            (PlantSortBy.CreatedAt, true) => plants.OrderByDescending(p => p.CreatedAtUtc),
            (PlantSortBy.Name, true) => plants.OrderByDescending(p => p.Name),
            _ => plants.OrderBy(p => p.Name),
        };

        var totalCount = await plants.CountAsync(cancellationToken);

        var items = await plants
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Plant>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
        };
    }

    public Task<Plant?> GetByIdForOwnerAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default) =>
        _db.Plants
            .Include(p => p.PlantType)
            .Include(p => p.Garden)
            .FirstOrDefaultAsync(p => p.Id == id && p.Garden!.OwnerId == ownerId, cancellationToken);

    public async Task AddAsync(Plant plant, CancellationToken cancellationToken = default)
    {
        _db.Plants.Add(plant);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Plant plant, CancellationToken cancellationToken = default)
    {
        _db.Plants.Update(plant);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Plant plant, CancellationToken cancellationToken = default)
    {
        _db.Plants.Remove(plant);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PlantTypeCategory>> GetDistinctCategoriesInGardenAsync(
        Guid gardenId,
        CancellationToken cancellationToken = default)
    {
        return await _db.Plants
            .AsNoTracking()
            .Where(p => p.GardenId == gardenId && p.PlantType != null)
            .Select(p => p.PlantType!.Category)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}
