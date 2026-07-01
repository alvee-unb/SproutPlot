using Microsoft.EntityFrameworkCore;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Common.Models;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Infrastructure.Persistence.Repositories;

/// <summary>EF Core implementation of <see cref="IWateringRepository"/>.</summary>
public sealed class WateringRepository : IWateringRepository
{
    private readonly AppDbContext _db;

    public WateringRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(WateringEvent wateringEvent, CancellationToken cancellationToken = default)
    {
        _db.WateringEvents.Add(wateringEvent);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<WateringEvent>> GetPagedByGardenAsync(
        Guid gardenId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _db.WateringEvents
            .AsNoTracking()
            .Include(w => w.Plant)
            .Where(w => w.GardenId == gardenId)
            .OrderByDescending(w => w.WateredAtUtc);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<WateringEvent>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
        };
    }

    public Task<DateTime?> GetLatestWateredAtUtcAsync(Guid gardenId, CancellationToken cancellationToken = default) =>
        _db.WateringEvents
            .Where(w => w.GardenId == gardenId)
            .MaxAsync(w => (DateTime?)w.WateredAtUtc, cancellationToken);

    public Task<WateringEvent?> GetByIdForOwnerAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default) =>
        _db.WateringEvents
            .Include(w => w.Garden)
            .FirstOrDefaultAsync(w => w.Id == id && w.Garden!.OwnerId == ownerId, cancellationToken);

    public async Task DeleteAsync(WateringEvent wateringEvent, CancellationToken cancellationToken = default)
    {
        _db.WateringEvents.Remove(wateringEvent);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
