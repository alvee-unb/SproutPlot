namespace SproutPlot.Application.Features.Auth.Dtos;

/// <summary>Payload for registering a new account.</summary>
public sealed record RegisterRequest
{
    public required string Email { get; init; }

    public required string Password { get; init; }

    /// <summary>Optional friendly name shown in the UI.</summary>
    public string? DisplayName { get; init; }
}
