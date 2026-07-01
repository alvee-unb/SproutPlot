using SproutPlot.Application.Features.Plants.Dtos;

namespace SproutPlot.Application.Common.Interfaces;

/// <summary>Read access to plant type reference data for selection lists.</summary>
public interface IPlantTypeService
{
    Task<IReadOnlyList<PlantTypeResponse>> GetPlantTypesAsync(
        string? search,
        CancellationToken cancellationToken = default);
}
