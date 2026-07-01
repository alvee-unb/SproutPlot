using FluentValidation;
using SproutPlot.Application.Features.Gardens.Dtos;

namespace SproutPlot.Application.Features.Gardens.Validators;

/// <summary>Validation rules for <see cref="UpdateGardenRequest"/>.</summary>
public sealed class UpdateGardenRequestValidator : AbstractValidator<UpdateGardenRequest>
{
    public UpdateGardenRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(x => x.Location).MaximumLength(200);
        RuleFor(x => x.Size).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}
