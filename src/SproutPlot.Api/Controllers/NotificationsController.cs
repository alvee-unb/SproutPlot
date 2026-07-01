using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Features.Notifications.Dtos;

namespace SproutPlot.Api.Controllers;

/// <summary>Notification preferences and on-demand reminder dispatch.</summary>
[Authorize]
[Route("api/notifications")]
[Produces("application/json")]
public sealed class NotificationsController : ApiControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IValidator<UpdateNotificationPreferencesRequest> _preferencesValidator;

    public NotificationsController(
        INotificationService notificationService,
        IValidator<UpdateNotificationPreferencesRequest> preferencesValidator)
    {
        _notificationService = notificationService;
        _preferencesValidator = preferencesValidator;
    }

    /// <summary>Gets the current user's notification preferences.</summary>
    [HttpGet("preferences")]
    [ProducesResponseType(typeof(NotificationPreferencesResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPreferences(CancellationToken cancellationToken)
    {
        var prefs = await _notificationService.GetPreferencesAsync(CurrentUserId, cancellationToken);
        return Ok(prefs);
    }

    /// <summary>Updates the current user's notification preferences.</summary>
    [HttpPut("preferences")]
    [ProducesResponseType(typeof(NotificationPreferencesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePreferences(
        UpdateNotificationPreferencesRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await _preferencesValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(validation);
        }

        var prefs = await _notificationService.UpdatePreferencesAsync(CurrentUserId, request, cancellationToken);
        return Ok(prefs);
    }

    /// <summary>
    /// Builds and dispatches the user's task reminder digest over the enabled
    /// channels. A scheduled job can call this per user later.
    /// </summary>
    [HttpPost("send-reminders")]
    [ProducesResponseType(typeof(ReminderSummaryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendReminders(CancellationToken cancellationToken)
    {
        var summary = await _notificationService.SendRemindersAsync(CurrentUserId, CurrentUserEmail, cancellationToken);
        return Ok(summary);
    }
}
