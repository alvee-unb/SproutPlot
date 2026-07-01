using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Features.Plants;
using SproutPlot.Domain.Entities;
using SproutPlot.Domain.Enums;

namespace SproutPlot.Application.Common.Interfaces;

/// <summary>Persistence operations for <see cref="Plant"/>.</summary>
public interface IPlantRepository
{
    /// <summary>Lists plants in a garden. Ownership of the garden is checked by the caller.</summary>
    Task<PagedResult<Plant>> GetPagedByGardenAsync(
        Guid gardenId,
        PlantQueryParameters query,
        CancellationToken cancellationToken = default);

    /// <summary>Gets a plant by id, but only if its garden belongs to the given owner.</summary>
    Task<Plant?> GetByIdForOwnerAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default);

    Task AddAsync(Plant plant, CancellationToken cancellationToken = default);

    Task UpdateAsync(Plant plant, CancellationToken cancellationToken = default);

    Task DeleteAsync(Plant plant, CancellationToken cancellationToken = default);

    /// <summary>Distinct plant-type categories present among a garden's typed plants.</summary>
    Task<IReadOnlyList<PlantTypeCategory>> GetDistinctCategoriesInGardenAsync(
        Guid gardenId,
        CancellationToken cancellationToken = default);
}
