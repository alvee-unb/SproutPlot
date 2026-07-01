using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Common.Results;
using SproutPlot.Application.Features.Watering.Dtos;

namespace SproutPlot.Application.Common.Interfaces;

/// <summary>Use cases for recording waterings and computing watering guidance.</summary>
public interface IWateringService
{
    Task<Result<WateringEventResponse>> RecordWateringAsync(
        Guid gardenId,
        Guid ownerId,
        RecordWateringRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<PagedResult<WateringEventResponse>>> GetHistoryAsync(
        Guid gardenId,
        Guid ownerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>Deterministic recommendation on whether the garden needs watering now.</summary>
    Task<Result<WateringRecommendationResponse>> GetRecommendationAsync(
        Guid gardenId,
        Guid ownerId,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteWateringAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default);
}
