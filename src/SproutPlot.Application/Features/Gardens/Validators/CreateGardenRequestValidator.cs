using FluentValidation;
using SproutPlot.Application.Features.Gardens.Dtos;

namespace SproutPlot.Application.Features.Gardens.Validators;

/// <summary>Validation rules for <see cref="CreateGardenRequest"/>.</summary>
public sealed class CreateGardenRequestValidator : AbstractValidator<CreateGardenRequest>
{
    public CreateGardenRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(x => x.Location).MaximumLength(200);
        RuleFor(x => x.Size).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(2000);
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue);
    }
}
