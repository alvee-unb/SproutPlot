using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Features.Plants;
using SproutPlot.Application.Tests.Features.Gardens;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Application.Tests.Features.Plants;

/// <summary>In-memory <see cref="IPlantRepository"/> that resolves ownership via a fake garden repo.</summary>
internal sealed class FakePlantRepository : IPlantRepository
{
    private readonly List<Plant> _plants = new();
    private readonly FakeGardenRepository _gardens;

    public FakePlantRepository(FakeGardenRepository gardens)
    {
        _gardens = gardens;
    }

    public IReadOnlyList<Plant> All => _plants;

    public Task<PagedResult<Plant>> GetPagedByGardenAsync(
        Guid gardenId,
        PlantQueryParameters query,
        CancellationToken cancellationToken = default)
    {
        var list = _plants.Where(p => p.GardenId == gardenId).OrderBy(p => p.Name).ToList();
        return Task.FromResult(new PagedResult<Plant>
        {
            Items = list,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = list.Count,
        });
    }

    public Task<Plant?> GetByIdForOwnerAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var plant = _plants.FirstOrDefault(p => p.Id == id);
        if (plant is null) return Task.FromResult<Plant?>(null);

        var owns = _gardens.All.Any(g => g.Id == plant.GardenId && g.OwnerId == ownerId);
        return Task.FromResult(owns ? plant : null);
    }

    public Task AddAsync(Plant plant, CancellationToken cancellationToken = default)
    {
        if (plant.Id == Guid.Empty) plant.Id = Guid.NewGuid();
        _plants.Add(plant);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Plant plant, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task DeleteAsync(Plant plant, CancellationToken cancellationToken = default)
    {
        _plants.Remove(plant);
        return Task.CompletedTask;
    }
}

/// <summary>In-memory <see cref="IPlantTypeRepository"/> seeded with a fixed set of ids.</summary>
internal sealed class FakePlantTypeRepository : IPlantTypeRepository
{
    private readonly HashSet<Guid> _ids;

    public FakePlantTypeRepository(params Guid[] ids)
    {
        _ids = ids.ToHashSet();
    }

    public Task<IReadOnlyList<PlantType>> GetAllAsync(string? search, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<PlantType>>(Array.Empty<PlantType>());

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_ids.Contains(id));
}
