using SproutPlot.Application.Common.Results;
using SproutPlot.Application.Features.Gardens;
using SproutPlot.Application.Features.Gardens.Dtos;
using Xunit;

namespace SproutPlot.Application.Tests.Features.Gardens;

public sealed class GardenServiceTests
{
    private readonly FakeGardenRepository _repository = new();
    private readonly GardenService _service;

    private static readonly Guid OwnerA = Guid.NewGuid();
    private static readonly Guid OwnerB = Guid.NewGuid();

    public GardenServiceTests()
    {
        _service = new GardenService(_repository);
    }

    [Fact]
    public async Task Create_persists_and_returns_mapped_garden()
    {
        var result = await _service.CreateGardenAsync(OwnerA, new CreateGardenRequest { Name = "  Front bed  " });

        Assert.True(result.Succeeded);
        Assert.Equal("Front bed", result.Value!.Name); // trimmed
        Assert.Single(_repository.All);
        Assert.Equal(OwnerA, _repository.All[0].OwnerId);
    }

    [Fact]
    public async Task Get_returns_not_found_for_other_users_garden()
    {
        var created = await _service.CreateGardenAsync(OwnerA, new CreateGardenRequest { Name = "Secret" });

        var result = await _service.GetGardenAsync(created.Value!.Id, OwnerB);

        Assert.False(result.Succeeded);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Update_missing_garden_returns_not_found()
    {
        var result = await _service.UpdateGardenAsync(Guid.NewGuid(), OwnerA, new UpdateGardenRequest { Name = "X" });

        Assert.False(result.Succeeded);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Delete_removes_owned_garden()
    {
        var created = await _service.CreateGardenAsync(OwnerA, new CreateGardenRequest { Name = "Temp" });

        var result = await _service.DeleteGardenAsync(created.Value!.Id, OwnerA);

        Assert.True(result.Succeeded);
        Assert.Empty(_repository.All);
    }

    [Fact]
    public async Task List_only_returns_callers_gardens()
    {
        await _service.CreateGardenAsync(OwnerA, new CreateGardenRequest { Name = "A1" });
        await _service.CreateGardenAsync(OwnerA, new CreateGardenRequest { Name = "A2" });
        await _service.CreateGardenAsync(OwnerB, new CreateGardenRequest { Name = "B1" });

        var page = await _service.GetGardensAsync(OwnerA, new GardenQueryParameters());

        Assert.Equal(2, page.TotalCount);
        Assert.All(page.Items, g => Assert.StartsWith("A", g.Name));
    }
}
