using SproutPlot.Domain.Enums;

namespace SproutPlot.Application.Features.Watering;

/// <summary>Resolves the meteorological season for a date and hemisphere.</summary>
public static class SeasonCalculator
{
    /// <summary>
    /// Returns the season. A negative latitude is treated as the southern
    /// hemisphere (seasons shifted by six months); null defaults to northern.
    /// </summary>
    public static Season Determine(DateOnly date, double? latitude)
    {
        var northern = NorthernSeason(date.Month);

        var isSouthern = latitude is < 0;
        return isSouthern ? Opposite(northern) : northern;
    }

    private static Season NorthernSeason(int month) => month switch
    {
        3 or 4 or 5 => Season.Spring,
        6 or 7 or 8 => Season.Summer,
        9 or 10 or 11 => Season.Autumn,
        _ => Season.Winter,
    };

    private static Season Opposite(Season season) => season switch
    {
        Season.Spring => Season.Autumn,
        Season.Summer => Season.Winter,
        Season.Autumn => Season.Spring,
        _ => Season.Summer,
    };
}
