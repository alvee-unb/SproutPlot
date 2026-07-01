using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Features.Gardens;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Application.Tests.Features.Gardens;

/// <summary>In-memory <see cref="IGardenRepository"/> for testing the service in isolation.</summary>
internal sealed class FakeGardenRepository : IGardenRepository
{
    private readonly List<Garden> _gardens = new();

    public IReadOnlyList<Garden> All => _gardens;

    public Task<PagedResult<Garden>> GetPagedByOwnerAsync(
        Guid ownerId,
        GardenQueryParameters query,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Garden> owned = _gardens.Where(g => g.OwnerId == ownerId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            owned = owned.Where(g => g.Name.Contains(query.Search, StringComparison.OrdinalIgnoreCase));
        }

        var ordered = query.SortBy == GardenSortBy.CreatedAt
            ? (query.Descending ? owned.OrderByDescending(g => g.CreatedAtUtc) : owned.OrderBy(g => g.CreatedAtUtc))
            : (query.Descending ? owned.OrderByDescending(g => g.Name) : owned.OrderBy(g => g.Name));

        var list = ordered.ToList();
        var items = list.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToList();

        return Task.FromResult(new PagedResult<Garden>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = list.Count,
        });
    }

    public Task<Garden?> GetByIdForOwnerAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_gardens.FirstOrDefault(g => g.Id == id && g.OwnerId == ownerId));

    public Task AddAsync(Garden garden, CancellationToken cancellationToken = default)
    {
        if (garden.Id == Guid.Empty) garden.Id = Guid.NewGuid();
        _gardens.Add(garden);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Garden garden, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task DeleteAsync(Garden garden, CancellationToken cancellationToken = default)
    {
        _gardens.Remove(garden);
        return Task.CompletedTask;
    }
}
