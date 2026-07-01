using SproutPlot.Domain.Enums;

namespace SproutPlot.Application.Features.Plants;

/// <summary>Sort fields supported by the plants list endpoint.</summary>
public enum PlantSortBy
{
    Name = 0,
    DatePlanted,
    CreatedAt,
}

/// <summary>Paging, filtering, and sorting options for listing plants in a garden.</summary>
public sealed record PlantQueryParameters
{
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 20;

    private readonly int _page = 1;
    private readonly int _pageSize = DefaultPageSize;

    public int Page
    {
        get => _page;
        init => _page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value < 1 ? DefaultPageSize : Math.Min(value, MaxPageSize);
    }

    /// <summary>Optional case-insensitive filter matched against the plant name.</summary>
    public string? Search { get; init; }

    /// <summary>Optional status filter.</summary>
    public PlantStatus? Status { get; init; }

    public PlantSortBy SortBy { get; init; } = PlantSortBy.Name;

    public bool Descending { get; init; }
}
