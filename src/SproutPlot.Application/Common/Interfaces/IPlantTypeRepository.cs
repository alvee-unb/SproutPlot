using SproutPlot.Domain.Entities;

namespace SproutPlot.Application.Common.Interfaces;

/// <summary>Read access to the seeded <see cref="PlantType"/> reference data.</summary>
public interface IPlantTypeRepository
{
    Task<IReadOnlyList<PlantType>> GetAllAsync(string? search, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
