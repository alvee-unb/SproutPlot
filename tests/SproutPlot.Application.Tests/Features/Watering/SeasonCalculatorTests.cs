using SproutPlot.Application.Features.Watering;
using SproutPlot.Domain.Enums;
using Xunit;

namespace SproutPlot.Application.Tests.Features.Watering;

public sealed class SeasonCalculatorTests
{
    [Theory]
    [InlineData(6, Season.Summer)]
    [InlineData(12, Season.Winter)]
    [InlineData(4, Season.Spring)]
    [InlineData(10, Season.Autumn)]
    public void Northern_hemisphere_seasons(int month, Season expected)
    {
        var date = new DateOnly(2026, month, 15);
        Assert.Equal(expected, SeasonCalculator.Determine(date, latitude: 51.5)); // London
    }

    [Theory]
    [InlineData(6, Season.Winter)]
    [InlineData(12, Season.Summer)]
    public void Southern_hemisphere_seasons_are_opposite(int month, Season expected)
    {
        var date = new DateOnly(2026, month, 15);
        Assert.Equal(expected, SeasonCalculator.Determine(date, latitude: -33.9)); // Sydney
    }

    [Fact]
    public void Null_latitude_defaults_to_northern()
    {
        Assert.Equal(Season.Summer, SeasonCalculator.Determine(new DateOnly(2026, 7, 1), latitude: null));
    }
}
