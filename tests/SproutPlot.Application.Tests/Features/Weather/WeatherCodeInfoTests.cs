using SproutPlot.Application.Features.Weather;
using Xunit;

namespace SproutPlot.Application.Tests.Features.Weather;

public sealed class WeatherCodeInfoTests
{
    [Theory]
    [InlineData(0, "Clear sky")]
    [InlineData(3, "Overcast")]
    [InlineData(61, "Slight rain")]
    [InlineData(95, "Thunderstorm")]
    [InlineData(1234, "Unknown")]
    public void Describe_maps_codes(int code, string expected)
    {
        Assert.Equal(expected, WeatherCodeInfo.Describe(code));
    }
}
