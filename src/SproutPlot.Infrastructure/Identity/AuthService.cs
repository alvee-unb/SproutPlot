using Microsoft.AspNetCore.Identity;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Common.Results;
using SproutPlot.Application.Features.Auth.Dtos;

namespace SproutPlot.Infrastructure.Identity;

/// <summary>ASP.NET Core Identity implementation of <see cref="IAuthService"/>.</summary>
public sealed class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthService(UserManager<ApplicationUser> userManager, IJwtTokenGenerator tokenGenerator)
    {
        _userManager = userManager;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
        {
            return Result<AuthResponse>.Failure("An account with this email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName,
            CreatedAtUtc = DateTime.UtcNow,
        };

        var created = await _userManager.CreateAsync(user, request.Password);
        if (!created.Succeeded)
        {
            return Result<AuthResponse>.Failure(created.Errors.Select(e => e.Description).ToArray());
        }

        return BuildAuthResponse(user);
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            // Deliberately generic to avoid leaking which accounts exist.
            return Result<AuthResponse>.Failure("Invalid email or password.");
        }

        return BuildAuthResponse(user);
    }

    private Result<AuthResponse> BuildAuthResponse(ApplicationUser user)
    {
        var (token, expiresAtUtc) = _tokenGenerator.GenerateToken(
            user.Id,
            user.Email!,
            roles: Array.Empty<string>());

        return Result<AuthResponse>.Success(new AuthResponse
        {
            AccessToken = token,
            ExpiresAtUtc = expiresAtUtc,
            Email = user.Email!,
            DisplayName = user.DisplayName,
        });
    }
}
