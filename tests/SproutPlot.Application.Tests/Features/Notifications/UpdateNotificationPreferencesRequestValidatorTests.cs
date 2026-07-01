using FluentValidation.TestHelper;
using SproutPlot.Application.Features.Notifications.Dtos;
using SproutPlot.Application.Features.Notifications.Validators;
using Xunit;

namespace SproutPlot.Application.Tests.Features.Notifications;

public sealed class UpdateNotificationPreferencesRequestValidatorTests
{
    private readonly UpdateNotificationPreferencesRequestValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(30)]
    public void Valid_lead_days_pass(int days)
    {
        var request = new UpdateNotificationPreferencesRequest { ReminderLeadDays = days };
        _validator.TestValidate(request).ShouldNotHaveValidationErrorFor(x => x.ReminderLeadDays);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(31)]
    public void Out_of_range_lead_days_fail(int days)
    {
        var request = new UpdateNotificationPreferencesRequest { ReminderLeadDays = days }
        ;
        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.ReminderLeadDays)
        ;
    }
}
