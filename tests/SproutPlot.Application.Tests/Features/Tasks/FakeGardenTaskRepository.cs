using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Features.Tasks;
using SproutPlot.Application.Tests.Features.Gardens;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Application.Tests.Features.Tasks;

/// <summary>In-memory <see cref="IGardenTaskRepository"/> resolving ownership via a fake garden repo.</summary>
internal sealed class FakeGardenTaskRepository : IGardenTaskRepository
{
    private readonly List<GardenTask> _tasks = new();
    private readonly FakeGardenRepository _gardens;

    public FakeGardenTaskRepository(FakeGardenRepository gardens)
    {
        _gardens = gardens;
    }

    public IReadOnlyList<GardenTask> All => _tasks;

    private bool OwnedBy(GardenTask task, Guid ownerId) =>
        _gardens.All.Any(g => g.Id == task.GardenId && g.OwnerId == ownerId);

    public Task<PagedResult<GardenTask>> GetPagedForOwnerAsync(
        Guid ownerId,
        TaskQueryParameters query,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<GardenTask> tasks = _tasks.Where(t => OwnedBy(t, ownerId));

        if (query.GardenId is { } gardenId) tasks = tasks.Where(t => t.GardenId == gardenId);
        if (query.Status is { } status) tasks = tasks.Where(t => t.Status == status);
        if (query.DueOnOrBefore is { } due) tasks = tasks.Where(t => t.DueOn <= due);

        var list = tasks.OrderBy(t => t.DueOn).ToList();

        return Task.FromResult(new PagedResult<GardenTask>
        {
            Items = list,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = list.Count,
        });
    }

    public Task<GardenTask?> GetByIdForOwnerAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(task is not null && OwnedBy(task, ownerId) ? task : null);
    }

    public Task AddAsync(GardenTask task, CancellationToken cancellationToken = default)
    {
        if (task.Id == Guid.Empty) task.Id = Guid.NewGuid();
        _tasks.Add(task);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(GardenTask task, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task DeleteAsync(GardenTask task, CancellationToken cancellationToken = default)
    {
        _tasks.Remove(task);
        return Task.CompletedTask;
    }
}
