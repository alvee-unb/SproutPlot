namespace SproutPlot.Domain.Common;

/// <summary>
/// Base type for persistent domain entities. Provides a surrogate key and
/// audit timestamps that all aggregates share.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }

    /// <summary>UTC timestamp set when the entity is first persisted.</summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>UTC timestamp set on the most recent update, if any.</summary>
    public DateTime? UpdatedAtUtc { get; set; }
}
