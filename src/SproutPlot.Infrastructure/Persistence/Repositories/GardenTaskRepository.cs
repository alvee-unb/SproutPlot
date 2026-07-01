using Microsoft.EntityFrameworkCore;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Features.Tasks;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Infrastructure.Persistence.Repositories;

/// <summary>EF Core implementation of <see cref="IGardenTaskRepository"/>.</summary>
public sealed class GardenTaskRepository : IGardenTaskRepository
{
    private readonly AppDbContext _db;

    public GardenTaskRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<GardenTask>> GetPagedForOwnerAsync(
        Guid ownerId,
        TaskQueryParameters query,
        CancellationToken cancellationToken = default)
    {
        var tasks = _db.GardenTasks
            .AsNoTracking()
            .Include(t => t.Garden)
            .Include(t => t.Plant)
            .Where(t => t.Garden!.OwnerId == ownerId);

        if (query.GardenId is { } gardenId)
        {
            tasks = tasks.Where(t => t.GardenId == gardenId);
        }

        if (query.Status is { } status)
        {
            tasks = tasks.Where(t => t.Status == status);
        }

        if (query.DueOnOrBefore is { } dueBefore)
        {
            tasks = tasks.Where(t => t.DueOn <= dueBefore);
        }

        tasks = (query.SortBy, query.Descending) switch
        {
            (TaskSortBy.CreatedAt, false) => tasks.OrderBy(t => t.CreatedAtUtc),
            (TaskSortBy.CreatedAt, true) => tasks.OrderByDescending(t => t.CreatedAtUtc),
            (TaskSortBy.DueOn, true) => tasks.OrderByDescending(t => t.DueOn),
            _ => tasks.OrderBy(t => t.DueOn),
        };

        var totalCount = await tasks.CountAsync(cancellationToken);

        var items = await tasks
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<GardenTask>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
        };
    }

    public Task<GardenTask?> GetByIdForOwnerAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default) =>
        _db.GardenTasks
            .Include(t => t.Garden)
            .Include(t => t.Plant)
            .FirstOrDefaultAsync(t => t.Id == id && t.Garden!.OwnerId == ownerId, cancellationToken);

    public async Task AddAsync(GardenTask task, CancellationToken cancellationToken = default)
    {
        _db.GardenTasks.Add(task);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(GardenTask task, CancellationToken cancellationToken = default)
    {
        _db.GardenTasks.Update(task);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(GardenTask task, CancellationToken cancellationToken = default)
    {
        _db.GardenTasks.Remove(task);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
