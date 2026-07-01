using FluentValidation;
using SproutPlot.Application.Features.Tasks.Dtos;

namespace SproutPlot.Application.Features.Tasks.Validators;

/// <summary>Validation rules for <see cref="UpdateTaskRequest"/>.</summary>
public sealed class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Title).MaximumLength(150);
        RuleFor(x => x.Notes).MaximumLength(1000);
        RuleFor(x => x.DueOn).NotEqual(default(DateOnly)).WithMessage("A due date is required.");
    }
}
