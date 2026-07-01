using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Common.Results;
using SproutPlot.Application.Features.Gardens;
using SproutPlot.Application.Features.Gardens.Dtos;

namespace SproutPlot.Application.Common.Interfaces;

/// <summary>Use cases for managing a user's gardens. Ownership is enforced here.</summary>
public interface IGardenService
{
    Task<PagedResult<GardenResponse>> GetGardensAsync(
        Guid ownerId,
        GardenQueryParameters query,
        CancellationToken cancellationToken = default);

    Task<Result<GardenResponse>> GetGardenAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default);

    Task<Result<GardenResponse>> CreateGardenAsync(
        Guid ownerId,
        CreateGardenRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<GardenResponse>> UpdateGardenAsync(
        Guid id,
        Guid ownerId,
        UpdateGardenRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteGardenAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default);
}
