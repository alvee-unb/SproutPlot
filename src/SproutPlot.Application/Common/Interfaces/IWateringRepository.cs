using SproutPlot.Application.Common.Models;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Application.Common.Interfaces;

/// <summary>Persistence operations for <see cref="WateringEvent"/>.</summary>
public interface IWateringRepository
{
    Task AddAsync(WateringEvent wateringEvent, CancellationToken cancellationToken = default);

    /// <summary>Watering history for a garden, newest first. Ownership is checked by the caller.</summary>
    Task<PagedResult<WateringEvent>> GetPagedByGardenAsync(
        Guid gardenId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>Timestamp of the most recent watering in the garden, or null if none.</summary>
    Task<DateTime?> GetLatestWateredAtUtcAsync(Guid gardenId, CancellationToken cancellationToken = default);

    Task<WateringEvent?> GetByIdForOwnerAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default);

    Task DeleteAsync(WateringEvent wateringEvent, CancellationToken cancellationToken = default);
}
