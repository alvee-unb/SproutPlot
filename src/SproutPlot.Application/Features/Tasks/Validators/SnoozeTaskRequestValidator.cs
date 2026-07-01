using FluentValidation;
using SproutPlot.Application.Features.Tasks.Dtos;

namespace SproutPlot.Application.Features.Tasks.Validators;

/// <summary>Validation rules for <see cref="SnoozeTaskRequest"/>.</summary>
public sealed class SnoozeTaskRequestValidator : AbstractValidator<SnoozeTaskRequest>
{
    public SnoozeTaskRequestValidator()
    {
        RuleFor(x => x.Days).InclusiveBetween(1, 365);
    }
}
