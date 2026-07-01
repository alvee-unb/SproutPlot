using FluentValidation;
using SproutPlot.Application.Features.Plants.Dtos;

namespace SproutPlot.Application.Features.Plants.Validators;

/// <summary>Validation rules for <see cref="CreatePlantRequest"/>.</summary>
public sealed class CreatePlantRequestValidator : AbstractValidator<CreatePlantRequest>
{
    public CreatePlantRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Variety).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(2000);
        RuleFor(x => x.Quantity).InclusiveBetween(1, 100_000);
        RuleFor(x => x.Status).IsInEnum();
    }
}
