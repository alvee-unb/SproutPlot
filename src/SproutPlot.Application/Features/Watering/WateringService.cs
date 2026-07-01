using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Common.Results;
using SproutPlot.Application.Features.Watering.Dtos;
using SproutPlot.Domain.Entities;

namespace SproutPlot.Application.Features.Watering;

/// <summary>
/// Records waterings and produces the deterministic watering recommendation by
/// assembling history, plant types, season and (when the garden has coordinates)
/// the rain forecast, then delegating the decision to <see cref="WateringCalculator"/>.
/// </summary>
public sealed class WateringService : IWateringService
{
    private readonly IWateringRepository _waterings;
    private readonly IGardenRepository _gardens;
    private readonly IPlantRepository _plants;
    private readonly IWeatherService _weather;
    private readonly TimeProvider _timeProvider;

    public WateringService(
        IWateringRepository waterings,
        IGardenRepository gardens,
        IPlantRepository plants,
        IWeatherService weather,
        TimeProvider timeProvider)
    {
        _waterings = waterings;
        _gardens = gardens;
        _plants = plants;
        _weather = weather;
        _timeProvider = timeProvider;
    }

    public async Task<Result<WateringEventResponse>> RecordWateringAsync(
        Guid gardenId,
        Guid ownerId,
        RecordWateringRequest request,
        CancellationToken cancellationToken = default)
    {
        var garden = await _gardens.GetByIdForOwnerAsync(gardenId, ownerId, cancellationToken);
        if (garden is null)
        {
            return Result<WateringEventResponse>.NotFound("Garden not found.");
        }

        string? plantName = null;
        if (request.PlantId is { } plantId)
        {
            var plant = await _plants.GetByIdForOwnerAsync(plantId, ownerId, cancellationToken);
            if (plant is null || plant.GardenId != gardenId)
            {
                return Result<WateringEventResponse>.Failure("The specified plant is not in this garden.");
            }

            plantName = plant.Name;
        }

        var wateringEvent = new WateringEvent
        {
            GardenId = gardenId,
            PlantId = request.PlantId,
            WateredAtUtc = request.WateredAtUtc ?? _timeProvider.GetUtcNow().UtcDateTime,
            AmountLiters = request.AmountLiters,
            Notes = request.Notes?.Trim(),
        };

        await _waterings.AddAsync(wateringEvent, cancellationToken);
        return Result<WateringEventResponse>.Success(MapToResponse(wateringEvent, plantName));
    }

    public async Task<Result<PagedResult<WateringEventResponse>>> GetHistoryAsync(
        Guid gardenId,
        Guid ownerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var garden = await _gardens.GetByIdForOwnerAsync(gardenId, ownerId, cancellationToken);
        if (garden is null)
        {
            return Result<PagedResult<WateringEventResponse>>.NotFound("Garden not found.");
        }

        var normalizedPage = page < 1 ? 1 : page;
        var normalizedSize = pageSize is < 1 or > 100 ? 20 : pageSize;

        var events = await _waterings.GetPagedByGardenAsync(gardenId, normalizedPage, normalizedSize, cancellationToken);

        return Result<PagedResult<WateringEventResponse>>.Success(new PagedResult<WateringEventResponse>
        {
            Items = events.Items.Select(e => MapToResponse(e, e.Plant?.Name)).ToList(),
            Page = events.Page,
            PageSize = events.PageSize,
            TotalCount = events.TotalCount,
        });
    }

    public async Task<Result<WateringRecommendationResponse>> GetRecommendationAsync(
        Guid gardenId,
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        var garden = await _gardens.GetByIdForOwnerAsync(gardenId, ownerId, cancellationToken);
        if (garden is null)
        {
            return Result<WateringRecommendationResponse>.NotFound("Garden not found.");
        }

        var categories = await _plants.GetDistinctCategoriesInGardenAsync(gardenId, cancellationToken);
        var baseInterval = categories.Count > 0
            ? categories.Min(PlantWateringIntervals.BaseIntervalDays)
            : PlantWateringIntervals.DefaultIntervalDays;

        var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
        var today = DateOnly.FromDateTime(nowUtc);

        var lastWateredUtc = await _waterings.GetLatestWateredAtUtcAsync(gardenId, cancellationToken);
        int? daysSince = lastWateredUtc is { } last
            ? Math.Max(0, today.DayNumber - DateOnly.FromDateTime(last).DayNumber)
            : null;

        var (maxRainProbability, rainSum, rainConsidered) =
            await TryGetRainAsync(garden, cancellationToken);

        var input = new WateringCalculatorInput
        {
            BaseIntervalDays = baseInterval,
            Season = SeasonCalculator.Determine(today, garden.Latitude),
            DaysSinceLastWatering = daysSince,
            MaxRainProbabilityNext2Days = maxRainProbability,
            RainSumNext2DaysMm = rainSum,
            Today = today,
            RainConsidered = rainConsidered,
        };

        return Result<WateringRecommendationResponse>.Success(WateringCalculator.Recommend(input));
    }

    public async Task<Result> DeleteWateringAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var wateringEvent = await _waterings.GetByIdForOwnerAsync(id, ownerId, cancellationToken);
        if (wateringEvent is null)
        {
            return Result.NotFound("Watering record not found.");
        }

        await _waterings.DeleteAsync(wateringEvent, cancellationToken);
        return Result.Success();
    }

    /// <summary>Fetches rain over the next two days; degrades gracefully if unavailable.</summary>
    private async Task<(double? MaxProbability, double? SumMm, bool Considered)> TryGetRainAsync(
        Garden garden,
        CancellationToken cancellationToken)
    {
        if (garden.Latitude is not { } lat || garden.Longitude is not { } lon)
        {
            return (null, null, false);
        }

        try
        {
            var weather = await _weather.GetWeatherAsync(lat, lon, cancellationToken);
            var nextTwo = weather.Daily.Take(2).ToList();
            if (nextTwo.Count == 0)
            {
                return (null, null, false);
            }

            double? maxProbability = nextTwo
                .Select(d => d.PrecipitationProbabilityMaxPercent)
                .Where(p => p.HasValue)
                .Select(p => (double)p!.Value)
                .DefaultIfEmpty()
                .Max();

            var sum = nextTwo.Sum(d => d.PrecipitationSumMm);
            return (maxProbability, sum, true);
        }
        catch
        {
            // Weather provider unavailable — fall back to history/season only.
            return (null, null, false);
        }
    }

    private static WateringEventResponse MapToResponse(WateringEvent e, string? plantName) => new()
    {
        Id = e.Id,
        GardenId = e.GardenId,
        PlantId = e.PlantId,
        PlantName = plantName,
        WateredAtUtc = e.WateredAtUtc,
        AmountLiters = e.AmountLiters,
        Notes = e.Notes,
    };
}
