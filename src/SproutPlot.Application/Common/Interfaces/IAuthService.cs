using SproutPlot.Application.Common.Results;
using SproutPlot.Application.Features.Auth.Dtos;

namespace SproutPlot.Application.Common.Interfaces;

/// <summary>
/// Application-facing authentication operations. Implemented in the
/// Infrastructure layer against ASP.NET Core Identity.
/// </summary>
public interface IAuthService
{
    /// <summary>Creates a new account and returns an access token on success.</summary>
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>Validates credentials and returns an access token on success.</summary>
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
