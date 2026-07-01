using FluentValidation.TestHelper;
using SproutPlot.Application.Features.Gardens.Dtos;
using SproutPlot.Application.Features.Gardens.Validators;
using Xunit;

namespace SproutPlot.Application.Tests.Features.Gardens;

public sealed class CreateGardenRequestValidatorTests
{
    private readonly CreateGardenRequestValidator _validator = new();

    [Fact]
    public void Valid_request_passes()
    {
        var request = new CreateGardenRequest { Name = "Back yard", Location = "Dhaka", Size = "3 beds" };

        _validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_name_fails()
    {
        var request = new CreateGardenRequest { Name = "" };

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Overlong_name_fails()
    {
        var request = new CreateGardenRequest { Name = new string('x', 121) };

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.Name);
    }
}
