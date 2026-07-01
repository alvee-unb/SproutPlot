using Microsoft.AspNetCore.Identity;

namespace SproutPlot.Infrastructure.Identity;

/// <summary>
/// Identity user for the application. Kept in the Infrastructure layer because
/// it is an ASP.NET Core Identity concern; the Domain and Application layers
/// reference users only by <see cref="System.Guid"/> id and DTOs.
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>Optional friendly name shown in the UI.</summary>
    public string? DisplayName { get; set; }

    /// <summary>UTC timestamp of account creation.</summary>
    public DateTime CreatedAtUtc { get; set; }
}
