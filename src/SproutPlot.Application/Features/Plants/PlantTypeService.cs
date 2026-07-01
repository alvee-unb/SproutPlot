using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Features.Plants.Dtos;

namespace SproutPlot.Application.Features.Plants;

/// <summary>Reads seeded plant type reference data.</summary>
public sealed class PlantTypeService : IPlantTypeService
{
    private readonly IPlantTypeRepository _repository;

    public PlantTypeService(IPlantTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<PlantTypeResponse>> GetPlantTypesAsync(
        string? search,
        CancellationToken cancellationToken = default)
    {
        var types = await _repository.GetAllAsync(search, cancellationToken);

        return types
            .Select(t => new PlantTypeResponse { Id = t.Id, Name = t.Name, Category = t.Category })
            .ToList();
    }
}
