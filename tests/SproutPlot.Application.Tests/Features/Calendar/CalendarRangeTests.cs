using SproutPlot.Application.Features.Calendar;
using Xunit;

namespace SproutPlot.Application.Tests.Features.Calendar;

public sealed class CalendarRangeTests
{
    [Fact]
    public void ForMonth_returns_first_and_last_day()
    {
        var (from, to) = CalendarRange.ForMonth(new DateOnly(2026, 7, 15));

        Assert.Equal(new DateOnly(2026, 7, 1), from);
        Assert.Equal(new DateOnly(2026, 7, 31), to);
    }

    [Fact]
    public void ForMonth_handles_february()
    {
        var (from, to) = CalendarRange.ForMonth(new DateOnly(2026, 2, 10));

        Assert.Equal(new DateOnly(2026, 2, 1), from);
        Assert.Equal(new DateOnly(2026, 2, 28), to);
    }

    [Theory]
    [InlineData("2026-07-01", "2026-07-31", true)]
    [InlineData("2026-07-31", "2026-07-01", false)] // reversed
    [InlineData("2026-01-01", "2027-06-01", false)] // > 366 days
    public void IsValid_checks_order_and_span(string from, string to, bool expected)
    {
        Assert.Equal(expected, CalendarRange.IsValid(DateOnly.Parse(from), DateOnly.Parse(to)));
    }
}
