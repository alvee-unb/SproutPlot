using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Features.Gardens;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Application.Common.Interfaces;

/// <summary>Persistence operations for <see cref="Garden"/>, scoped to an owner.</summary>
public interface IGardenRepository
{
    Task<PagedResult<Garden>> GetPagedByOwnerAsync(
        Guid ownerId,
        GardenQueryParameters query,
        CancellationToken cancellationToken = default);

    Task<Garden?> GetByIdForOwnerAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default);

    Task AddAsync(Garden garden, CancellationToken cancellationToken = default);

    Task UpdateAsync(Garden garden, CancellationToken cancellationToken = default);

    Task DeleteAsync(Garden garden, CancellationToken cancellationToken = default);
}
