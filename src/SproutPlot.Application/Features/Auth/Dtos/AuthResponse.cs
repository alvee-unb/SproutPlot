namespace SproutPlot.Application.Features.Auth.Dtos;

/// <summary>Returned to the client after a successful register/login.</summary>
public sealed record AuthResponse
{
    public required string AccessToken { get; init; }

    public required DateTime ExpiresAtUtc { get; init; }

    public required string Email { get; init; }

    public string? DisplayName { get; init; }
}
