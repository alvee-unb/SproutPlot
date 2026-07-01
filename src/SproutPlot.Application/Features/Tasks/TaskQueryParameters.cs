using SproutPlot.Domain.Enums;

namespace SproutPlot.Application.Features.Tasks;

/// <summary>Sort fields for the tasks list.</summary>
public enum TaskSortBy
{
    DueOn = 0,
    CreatedAt,
}

/// <summary>Filtering, paging and sorting options for listing tasks.</summary>
public sealed record TaskQueryParameters
{
    public const int MaxPageSize = 200;
    public const int DefaultPageSize = 50;

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

    /// <summary>Restrict to a single garden (owner-checked separately for nested routes).</summary>
    public Guid? GardenId { get; init; }

    public GardenTaskStatus? Status { get; init; }

    /// <summary>Only tasks due on or before this date (used for "today/upcoming").</summary>
    public DateOnly? DueOnOrBefore { get; init; }

    public TaskSortBy SortBy { get; init; } = TaskSortBy.DueOn;

    public bool Descending { get; init; }
}
