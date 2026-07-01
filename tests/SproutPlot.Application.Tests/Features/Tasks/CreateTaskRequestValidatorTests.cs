using FluentValidation.TestHelper;
using SproutPlot.Application.Features.Tasks.Dtos;
using SproutPlot.Application.Features.Tasks.Validators;
using SproutPlot.Domain.Enums;
using Xunit;

namespace SproutPlot.Application.Tests.Features.Tasks;

public sealed class CreateTaskRequestValidatorTests
{
    private readonly CreateTaskRequestValidator _validator = new();

    [Fact]
    public void Valid_request_passes()
    {
        var request = new CreateTaskRequest
        {
            Type = GardenTaskType.Prune,
            DueOn = new DateOnly(2026, 7, 1),
            Title = "Prune the roses",
        };

        _validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Missing_due_date_fails()
    {
        var request = new CreateTaskRequest { Type = GardenTaskType.Water, DueOn = default };

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.DueOn);
    }

    [Fact]
    public void Overlong_title_fails()
    {
        var request = new CreateTaskRequest
        {
            Type = GardenTaskType.Water,
            DueOn = new DateOnly(2026, 7, 1),
            Title = new string('x', 151),
        };

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.Title);
    }
}
