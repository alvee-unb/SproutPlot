namespace SproutPlot.Application.Common.Interfaces;

/// <summary>Generates signed JWT access tokens for authenticated users.</summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Builds a signed access token for the given user.
    /// </summary>
    /// <returns>The encoded token and its UTC expiry.</returns>
    (string Token, DateTime ExpiresAtUtc) GenerateToken(Guid userId, string email, IEnumerable<string> roles);
}
