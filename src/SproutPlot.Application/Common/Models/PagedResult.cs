namespace SproutPlot.Application.Common.Models;

/// <summary>A single page of results plus the totals needed for pagination UIs.</summary>
/// <typeparam name="T">Item type.</typeparam>
public sealed class PagedResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }

    public required int Page { get; init; }

    public required int PageSize { get; init; }

    public required int TotalCount { get; init; }

    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
