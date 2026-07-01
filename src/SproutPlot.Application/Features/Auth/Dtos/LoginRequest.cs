namespace SproutPlot.Application.Features.Auth.Dtos;

/// <summary>Payload for authenticating an existing account.</summary>
public sealed record LoginRequest
{
    public required string Email { get; init; }

    public required string Password { get; init; }
}
