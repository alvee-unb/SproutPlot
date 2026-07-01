using FluentValidation;
using SproutPlot.Application.Features.Notifications.Dtos;

namespace SproutPlot.Application.Features.Notifications.Validators;

/// <summary>Validation rules for <see cref="UpdateNotificationPreferencesRequest"/>.</summary>
public sealed class UpdateNotificationPreferencesRequestValidator : AbstractValidator<UpdateNotificationPreferencesRequest>
{
    public UpdateNotificationPreferencesRequestValidator()
    {
        RuleFor(x => x.ReminderLeadDays).InclusiveBetween(0, 30);
    }
}
