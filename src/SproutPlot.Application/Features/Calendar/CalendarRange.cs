namespace SproutPlot.Application.Features.Calendar;

/// <summary>Helpers for resolving and validating a calendar date range.</summary>
public static class CalendarRange
{
    /// <summary>Maximum span a single query may cover.</summary>
    public const int MaxSpanDays = 366;

    /// <summary>The first and last day of the month containing <paramref name="date"/>.</summary>
    public static (DateOnly From, DateOnly To) ForMonth(DateOnly date)
    {
        var from = new DateOnly(date.Year, date.Month, 1);
        var to = from.AddMonths(1).AddDays(-1);
        return (from, to);
    }

    /// <summary>True if the range is ordered and within <see cref="MaxSpanDays"/>.</summary>
    public static bool IsValid(DateOnly from, DateOnly to) =>
        from <= to && to.DayNumber - from.DayNumber <= MaxSpanDays;
}
