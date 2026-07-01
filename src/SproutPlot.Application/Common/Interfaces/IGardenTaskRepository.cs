using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Features.Tasks;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Application.Common.Interfaces;

/// <summary>Persistence operations for <see cref="GardenTask"/>, scoped to an owner.</summary>
public interface IGardenTaskRepository
{
    Task<PagedResult<GardenTask>> GetPagedForOwnerAsync(
        Guid ownerId,
        TaskQueryParameters query,
        CancellationToken cancellationToken = default);

    Task<GardenTask?> GetByIdForOwnerAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default);

    Task AddAsync(GardenTask task, CancellationToken cancellationToken = default);

    Task UpdateAsync(GardenTask task, CancellationToken cancellationToken = default);

    Task DeleteAsync(GardenTask task, CancellationToken cancellationToken = default);
}
