using Microsoft.EntityFrameworkCore;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Infrastructure.Persistence.Repositories;

/// <summary>EF Core implementation of <see cref="IPlantTypeRepository"/>.</summary>
public sealed class PlantTypeRepository : IPlantTypeRepository
{
    private readonly AppDbContext _db;

    public PlantTypeRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<PlantType>> GetAllAsync(string? search, CancellationToken cancellationToken = default)
    {
        var types = _db.PlantTypes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            types = types.Where(t => EF.Functions.ILike(t.Name, pattern));
        }

        return await types.OrderBy(t => t.Name).ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.PlantTypes.AnyAsync(t => t.Id == id, cancellationToken);
}
