using SproutPlot.Application.Common.Results;
using SproutPlot.Application.Features.Plants;
using SproutPlot.Application.Features.Plants.Dtos;
using SproutPlot.Application.Tests.Features.Gardens;
using SproutPlot.Domain.Entities;
using Xunit;

namespace SproutPlot.Application.Tests.Features.Plants;

public sealed class PlantServiceTests
{
    private static readonly Guid OwnerA = Guid.NewGuid();
    private static readonly Guid OwnerB = Guid.NewGuid();
    private static readonly Guid KnownTypeId = Guid.NewGuid();

    private readonly FakeGardenRepository _gardens = new();
    private readonly FakePlantRepository _plants;
    private readonly PlantService _service;

    public PlantServiceTests()
    {
        _plants = new FakePlantRepository(_gardens);
        _service = new PlantService(_plants, _gardens, new FakePlantTypeRepository(KnownTypeId));
    }

    private async Task<Guid> SeedGardenAsync(Guid ownerId)
    {
        var garden = new Garden { Id = Guid.NewGuid(), OwnerId = ownerId, Name = "G" };
        await _gardens.AddAsync(garden);
        return garden.Id;
    }

    [Fact]
    public async Task Create_in_owned_garden_succeeds()
    {
        var gardenId = await SeedGardenAsync(OwnerA);

        var result = await _service.CreatePlantAsync(gardenId, OwnerA, new CreatePlantRequest { Name = "Tomato" });

        Assert.True(result.Succeeded);
        Assert.Equal(gardenId, result.Value!.GardenId);
        Assert.Single(_plants.All);
    }

    [Fact]
    public async Task Create_in_unowned_garden_returns_not_found()
    {
        var gardenId = await SeedGardenAsync(OwnerA);

        var result = await _service.CreatePlantAsync(gardenId, OwnerB, new CreatePlantRequest { Name = "Sneaky" });

        Assert.False(result.Succeeded);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
        Assert.Empty(_plants.All);
    }

    [Fact]
    public async Task Create_with_unknown_plant_type_fails_validation()
    {
        var gardenId = await SeedGardenAsync(OwnerA);

        var result = await _service.CreatePlantAsync(
            gardenId,
            OwnerA,
            new CreatePlantRequest { Name = "Mystery", PlantTypeId = Guid.NewGuid() });

        Assert.False(result.Succeeded);
        Assert.Equal(ResultErrorType.Validation, result.ErrorType);
    }

    [Fact]
    public async Task Create_with_known_plant_type_succeeds()
    {
        var gardenId = await SeedGardenAsync(OwnerA);

        var result = await _service.CreatePlantAsync(
            gardenId,
            OwnerA,
            new CreatePlantRequest { Name = "Basil", PlantTypeId = KnownTypeId });

        Assert.True(result.Succeeded);
        Assert.Equal(KnownTypeId, result.Value!.PlantTypeId);
    }

    [Fact]
    public async Task Get_plant_in_other_users_garden_returns_not_found()
    {
        var gardenId = await SeedGardenAsync(OwnerA);
        var created = await _service.CreatePlantAsync(gardenId, OwnerA, new CreatePlantRequest { Name = "Private" });

        var result = await _service.GetPlantAsync(created.Value!.Id, OwnerB);

        Assert.False(result.Succeeded);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task List_for_unowned_garden_returns_not_found()
    {
        var gardenId = await SeedGardenAsync(OwnerA);

        var result = await _service.GetPlantsAsync(gardenId, OwnerB, new PlantQueryParameters());

        Assert.False(result.Succeeded);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Delete_owned_plant_succeeds()
    {
        var gardenId = await SeedGardenAsync(OwnerA);
        var created = await _service.CreatePlantAsync(gardenId, OwnerA, new CreatePlantRequest { Name = "Temp" });

        var result = await _service.DeletePlantAsync(created.Value!.Id, OwnerA);

        Assert.True(result.Succeeded);
        Assert.Empty(_plants.All);
    }
}
