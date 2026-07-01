using FluentValidation.TestHelper;
using SproutPlot.Application.Features.Auth.Dtos;
using SproutPlot.Application.Features.Auth.Validators;
using Xunit;

namespace SproutPlot.Application.Tests.Features.Auth;

public sealed class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Fact]
    public void Valid_request_passes()
    {
        var request = new RegisterRequest
        {
            Email = "gardener@example.com",
            Password = "Sprout123",
            DisplayName = "Gardener",
        };

        _validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Invalid_email_fails(string email)
    {
        var request = new RegisterRequest { Email = email, Password = "Sprout123" };

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("short1A")]      // too short
    [InlineData("alllowercase1")] // no uppercase
    [InlineData("ALLUPPERCASE1")] // no lowercase
    [InlineData("NoDigitsHere")]  // no digit
    public void Weak_password_fails(string password)
    {
        var request = new RegisterRequest { Email = "gardener@example.com", Password = password };

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.Password);
    }
}
