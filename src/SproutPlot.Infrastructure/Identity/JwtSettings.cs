namespace SproutPlot.Infrastructure.Identity;

/// <summary>Strongly-typed JWT configuration bound from the "Jwt" section.</summary>
public sealed class JwtSettings
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    /// <summary>Symmetric signing key. Provided via user-secrets / Key Vault, never source control.</summary>
    public string Secret { get; set; } = string.Empty;

    public int AccessTokenExpirationMinutes { get; set; } = 60;
}
