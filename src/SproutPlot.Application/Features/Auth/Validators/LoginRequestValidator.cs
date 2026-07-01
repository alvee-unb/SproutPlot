using FluentValidation;
using SproutPlot.Application.Features.Auth.Dtos;

namespace SproutPlot.Application.Features.Auth.Validators;

/// <summary>Validation rules for <see cref="LoginRequest"/>.</summary>
public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
