namespace SproutPlot.Application.Features.Gardens;

/// <summary>Sort fields supported by the gardens list endpoint.</summary>
public enum GardenSortBy
{
    Name = 0,
    CreatedAt,
}

/// <summary>Paging, filtering, and sorting options for listing gardens.</summary>
public sealed record GardenQueryParameters
{
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 20;

    private readonly int _page = 1;
    private readonly int _pageSize = DefaultPageSize;

    /// <summary>1-based page number (clamped to a minimum of 1).</summary>
    public int Page
    {
        get => _page;
        init => _page = value < 1 ? 1 : value;
    }

    /// <summary>Page size (clamped to 1..<see cref="MaxPageSize"/>).</summary>
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value < 1 ? DefaultPageSize : Math.Min(value, MaxPageSize);
    }

    /// <summary>Optional case-insensitive filter matched against the garden name.</summary>
    public string? Search { get; init; }

    public GardenSortBy SortBy { get; init; } = GardenSortBy.Name;

    public bool Descending { get; init; }
}
