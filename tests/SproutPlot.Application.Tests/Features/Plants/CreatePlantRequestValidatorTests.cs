using FluentValidation.TestHelper;
using SproutPlot.Application.Features.Plants.Dtos;
using SproutPlot.Application.Features.Plants.Validators;
using SproutPlot.Domain.Enums;
using Xunit;

namespace SproutPlot.Application.Tests.Features.Plants;

public sealed class CreatePlantRequestValidatorTests
{
    private readonly CreatePlantRequestValidator _validator = new();

    [Fact]
    public void Valid_request_passes()
    {
        var request = new CreatePlantRequest { Name = "Tomato", Quantity = 3, Status = PlantStatus.Growing };

        _validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_name_fails()
    {
        _validator.TestValidate(new CreatePlantRequest { Name = "" })
            .ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Non_positive_quantity_fails(int quantity)
    {
        _validator.TestValidate(new CreatePlantRequest { Name = "Tomato", Quantity = quantity })
            .ShouldHaveValidationErrorFor(x => x.Quantity);
    }
}
