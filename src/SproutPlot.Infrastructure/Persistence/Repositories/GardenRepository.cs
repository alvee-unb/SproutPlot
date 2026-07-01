using Microsoft.EntityFrameworkCore;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Features.Gardens;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Infrastructure.Persistence.Repositories;

/// <summary>EF Core implementation of <see cref="IGardenRepository"/>.</summary>
public sealed class GardenRepository : IGardenRepository
{
    private readonly AppDbContext _db;

    public GardenRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<Garden>> GetPagedByOwnerAsync(
        Guid ownerId,
        GardenQueryParameters query,
        CancellationToken cancellationToken = default)
    {
        var gardens = _db.Gardens
            .AsNoTracking()
            .Where(g => g.OwnerId == ownerId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            gardens = gardens.Where(g => EF.Functions.ILike(g.Name, pattern));
        }

        gardens = (query.SortBy, query.Descending) switch
        {
            (GardenSortBy.CreatedAt, false) => gardens.OrderBy(g => g.CreatedAtUtc),
            (GardenSortBy.CreatedAt, true) => gardens.OrderByDescending(g => g.CreatedAtUtc),
            (GardenSortBy.Name, true) => gardens.OrderByDescending(g => g.Name),
            _ => gardens.OrderBy(g => g.Name),
        };

        var totalCount = await gardens.CountAsync(cancellationToken);

        var items = await gardens
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Garden>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
        };
    }

    public Task<Garden?> GetByIdForOwnerAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default) =>
        _db.Gardens.FirstOrDefaultAsync(g => g.Id == id && g.OwnerId == ownerId, cancellationToken);

    public async Task AddAsync(Garden garden, CancellationToken cancellationToken = default)
    {
        _db.Gardens.Add(garden);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Garden garden, CancellationToken cancellationToken = default)
    {
        _db.Gardens.Update(garden);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Garden garden, CancellationToken cancellationToken = default)
    {
        _db.Gardens.Remove(garden);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
