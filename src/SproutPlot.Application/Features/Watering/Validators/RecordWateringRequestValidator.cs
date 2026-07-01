using FluentValidation;
using SproutPlot.Application.Features.Watering.Dtos;

namespace SproutPlot.Application.Features.Watering.Validators;

/// <summary>Validation rules for <see cref="RecordWateringRequest"/>.</summary>
public sealed class RecordWateringRequestValidator : AbstractValidator<RecordWateringRequest>
{
    public RecordWateringRequestValidator()
    {
        RuleFor(x => x.AmountLiters)
            .GreaterThanOrEqualTo(0).When(x => x.AmountLiters.HasValue);

        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
