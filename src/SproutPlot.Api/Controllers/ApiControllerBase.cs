using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SproutPlot.Application.Common.Results;

namespace SproutPlot.Api.Controllers;

/// <summary>Shared controller helpers: current-user resolution and Result mapping.</summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>The authenticated user's id, taken from the JWT subject claim.</summary>
    protected Guid CurrentUserId
    {
        get
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            return Guid.TryParse(value, out var id)
                ? id
                : throw new InvalidOperationException("Authenticated user id claim is missing or invalid.");
        }
    }

    /// <summary>The authenticated user's email, taken from the JWT email claim.</summary>
    protected string CurrentUserEmail =>
        User.FindFirstValue(ClaimTypes.Email)
        ?? User.FindFirstValue("email")
        ?? throw new InvalidOperationException("Authenticated user email claim is missing.");

    /// <summary>Maps a failed <see cref="Result"/> to the appropriate HTTP response.</summary>
    protected IActionResult FromError(Result result)
    {
        var detail = string.Join(" ", result.Errors);
        return result.ErrorType switch
        {
            ResultErrorType.NotFound => Problem(detail: detail, statusCode: StatusCodes.Status404NotFound),
            ResultErrorType.Conflict => Problem(detail: detail, statusCode: StatusCodes.Status409Conflict),
            _ => Problem(detail: detail, statusCode: StatusCodes.Status400BadRequest),
        };
    }

    /// <summary>Builds a 400 ValidationProblem from a FluentValidation result.</summary>
    protected IActionResult ValidationProblem(FluentValidation.Results.ValidationResult validation)
    {
        var errors = validation.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return ValidationProblem(new ValidationProblemDetails(errors));
    }
}
